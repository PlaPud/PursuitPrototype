using UnityEngine;

abstract public class Interactable : MonoBehaviour
{
    private const float HOLD_DOWN_TIME = 1f;

    protected private bool _isInteractHold;
    protected private bool _isReadyToInteract;
    protected private bool _isKeyLock;
    protected private float _holdTimer = HOLD_DOWN_TIME;

    private KeyCode _key = KeyCode.E;
    public Collider2D PlayerCD { get; private set; }

    protected virtual void Update()
    {
        OnInteract();

        CountDownToInteract();
    }

    private void OnInteract()
    {
        if (Input.GetKeyUp(_key)) _isKeyLock = false;

        if (_isKeyLock) return;

        if (PlayerCD && PlayerCD.CompareTag("PlayerCat"))
        {
            _isInteractHold = Input.GetKey(_key);
        }
    }
    private void CountDownToInteract() 
    {
        _holdTimer = _isInteractHold ?
            _holdTimer - Time.deltaTime : HOLD_DOWN_TIME;
        
        _isReadyToInteract = _holdTimer <= 0;

        if (_isReadyToInteract)
        {
            _isReadyToInteract = false;
            _LockInteractKey();
            HandleInteract();
        }
    }

    public abstract void HandleInteract();

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("PlayerCat")) return;
        PlayerCD = collision;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("PlayerCat")) return;
        PlayerCD = null;
    }

    private void _LockInteractKey()
    {
        _isInteractHold = false;
        _isKeyLock = true;
    }

}
