using System;
using UnityEngine;

[RequireComponent (typeof(Collider2D))]
abstract public class Interactable : MonoBehaviour
{
    //TODO: Fix Interaction Switch Bug (Player Kept walking)

    public enum Interact { HoldToInteract, PressToInteract }

    [Header("Interaction")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private Interact typeOfInteract = Interact.PressToInteract;
    [SerializeField] private bool compBotInteractable = false;

    private const float HOLD_DOWN_TIME = 0.6f;

    protected private bool _isInteract;
    protected private bool _isReadyToInteract;
    protected private bool _isKeyLock;
    protected private float _holdTimer = HOLD_DOWN_TIME;

    public static Action<Transform, Collider2D, KeyCode> OnAnyStayInteractable;
    public static Action<KeyCode> OnAnyExitInteractable;
    public static Action<bool, float> OnAnyCountdownInteract;
    public static Action<bool, float, Interact, KeyCode> OnAnyInteraction;

    public Collider2D PlayerCD { get; private set; }

    public bool IsControllingCat => ControllingManager.Instance.IsControllingCat;
    public bool IsControllingCompBot => ControllingManager.Instance.IsControllingCompBot;
    public bool IsControllingClawMachine => ControllingManager.Instance.IsControllingClawMachine;

    protected virtual void Update()
    {
        switch (typeOfInteract)
        {
            case Interact.HoldToInteract:
                _HandleHoldToInteract();
                _CountDownToInteract();
                break;
            case Interact.PressToInteract:
                _HandlePressToInteract();
                _TriggerInteract();
                break;
        }
    }

    private void _HandleHoldToInteract() 
    {
        if (Input.GetKeyUp(interactKey)) _isKeyLock = false;

        if (_isKeyLock) return;

        if (PlayerCD && _IsControllableInteractable(PlayerCD))
        {
            _isInteract = Input.GetKey(interactKey);
        }
    }

    private void _HandlePressToInteract() 
    {
        if (PlayerCD && _IsControllableInteractable(PlayerCD)) 
        {
            _isInteract = Input.GetKeyDown(interactKey);
        }
    }
    private void _CountDownToInteract() 
    {
        if (!PlayerCD) return;

        _holdTimer = _isInteract ?
            _holdTimer - Time.deltaTime : HOLD_DOWN_TIME;

        _isReadyToInteract = _holdTimer <= 0;

        OnAnyCountdownInteract.Invoke(_isInteract, _holdTimer);

        if (_isReadyToInteract)
        {
            _isReadyToInteract = false;
            _LockInteractKey();
            HandleInteract();
        }
    }

    private void _TriggerInteract() 
    {
        if (!_isInteract) return;
        OnAnyInteraction?.Invoke(_isInteract, _holdTimer, typeOfInteract, interactKey);
        HandleInteract();
    }

    public abstract void HandleInteract();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_IsControllableInteractable(collision)) return;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (!_IsControllableInteractable(collision) || collision.GetComponent<Rigidbody2D>().velocity.magnitude > .5f) return;

        PlayerPushPull checkedPushPull = collision.gameObject.GetComponent<PlayerPushPull>();

        if (checkedPushPull.IsFoundMoveable)
        {
            PlayerCD = null;
            _isInteract = false;
            _isReadyToInteract = false;
            _isKeyLock = false;
            _holdTimer = HOLD_DOWN_TIME;
            OnAnyExitInteractable?.Invoke(interactKey);
            return;
        };

        PlayerCD = collision;

        OnAnyStayInteractable?.Invoke(transform, collision, interactKey);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
        if (!_IsControllableInteractable(collision)) return;

        PlayerCD = null;
        _isInteract = false;
        _isReadyToInteract = false;
        _isKeyLock = false;
        _holdTimer = HOLD_DOWN_TIME;

        try 
        {
            OnAnyExitInteractable?.Invoke(interactKey);
        }
        catch (Exception e) 
        {
            Debug.LogException(e);
        }
    }

    private bool _IsControllableInteractable(Collider2D collision) 
    {
        bool isCatInteract = (IsControllingCat || gameObject.CompareTag("ControlPanel")) && collision.CompareTag("PlayerCat");
        bool isCompBotInteract = compBotInteractable && IsControllingCompBot && collision.CompareTag("PlayerCompBot") && ControllingManager.Instance.IsMatchedCompBot(collision.GetComponent<CompBotController>());

        return isCatInteract || isCompBotInteract;
    }

    private void _LockInteractKey()
    {
        _isInteract = false;
        _isKeyLock = true;
    }

}
