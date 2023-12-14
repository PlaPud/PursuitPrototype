using System;
using UnityEngine;

[RequireComponent (typeof(Collider2D))]
abstract public class Interactable : MonoBehaviour
{
    //TODO: Fix Interaction Switch Bug (Player Kept walking)

    public enum Interact { HoldToInteract, PressToInteract }

    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private Interact typeOfInteract = Interact.PressToInteract;
    [SerializeField] private bool compBotInteractable = false;

    private const float HOLD_DOWN_TIME = 1f;

    protected private bool _isInteract;
    protected private bool _isReadyToInteract;
    protected private bool _isKeyLock;
    protected private float _holdTimer = HOLD_DOWN_TIME;

    public static event Action<KeyCode> OnAnyStayInteractable;
    public static event Action<KeyCode> OnAnyExitInteractable;
    public static event Action<bool, float, Interact, KeyCode> OnAnyInteraction;

    public Collider2D PlayerCD { get; private set; }

    public bool IsControllingCat => ControllingManager.Instance.IsControllingCat;
    public bool IsControllingCompBot => ControllingManager.Instance.IsControllingCompBot;

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

        bool isCompBotInteract = IsControllingCompBot && compBotInteractable && PlayerCD && PlayerCD.CompareTag("PlayerCompBot");
        bool isCatInteract = (IsControllingCat || gameObject.CompareTag("ControlPanel")) && PlayerCD && PlayerCD.CompareTag("PlayerCat");
        if (isCatInteract || isCompBotInteract)
        {
            _isInteract = Input.GetKey(interactKey);
        }
    }

    private void _HandlePressToInteract() 
    {
        bool isCompBotInteract = IsControllingCompBot && compBotInteractable && PlayerCD && PlayerCD.CompareTag("PlayerCompBot");
        bool isCatInteract = (IsControllingCat || gameObject.CompareTag("ControlPanel")) && PlayerCD && PlayerCD.CompareTag("PlayerCat");
        if (isCatInteract || isCompBotInteract) 
        {
            _isInteract = Input.GetKeyDown(interactKey);
        }
    }
    private void _CountDownToInteract() 
    {
        _holdTimer = _isInteract ?
            _holdTimer - Time.deltaTime : HOLD_DOWN_TIME;
        
        _isReadyToInteract = _holdTimer <= 0;

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
        bool isCompBotInteract = IsControllingCompBot && compBotInteractable && collision.CompareTag("PlayerCompBot");
        bool isCatInteract = (IsControllingCat || gameObject.CompareTag("ControlPanel")) && collision.CompareTag("PlayerCat");
        if (!isCatInteract && !isCompBotInteract) return;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        bool isCompBotInteract = IsControllingCompBot && compBotInteractable && collision.CompareTag("PlayerCompBot");
        bool isCatInteract = (IsControllingCat || gameObject.CompareTag("ControlPanel")) && collision.CompareTag("PlayerCat");

        if (!isCatInteract && !isCompBotInteract) return;

        PlayerPushPull checkedPushPull = collision.gameObject.GetComponent<PlayerPushPull>();

        if (checkedPushPull.IsFoundMoveable) return;

        PlayerCD = collision;
        OnAnyStayInteractable?.Invoke(interactKey);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        bool isCompBotInteract = IsControllingCompBot && compBotInteractable && collision.CompareTag("PlayerCompBot");
        bool isCatInteract = (IsControllingCat || gameObject.CompareTag("ControlPanel")) && collision.CompareTag("PlayerCat");
        if (!isCatInteract && !isCompBotInteract) return;
        PlayerCD = null;
        OnAnyExitInteractable?.Invoke(interactKey);
    }

    private void _LockInteractKey()
    {
        _isInteract = false;
        _isKeyLock = true;
    }

}
