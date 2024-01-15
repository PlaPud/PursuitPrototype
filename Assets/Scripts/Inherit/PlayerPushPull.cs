using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPushPull : MonoBehaviour
{
    private const float HOLD_DOWN_TIME = 0.6f;
    private const KeyCode KEY_TO_HOLD = KeyCode.E;

    [Header("Check Moveable")]
    [SerializeField] private float grabDistance;
    [SerializeField] private LayerMask groundLayer;

    [Header("Controllables")]
    [SerializeField] private IControllableOnGround player;
    [SerializeField] ControllingManager.Control pusher;

    private RaycastHit2D _hitGroundForward;

    private GameObject _moveableObject;
    private Rigidbody2D _playerRB;

    private KeyCode _key = KEY_TO_HOLD;
    private bool _isInteractHold;
    private bool _isReadyToInteract;
    private bool _isKeyLock;
    private float _holdTimer = HOLD_DOWN_TIME;

    public bool IsGrabbing => _moveableObject != null;
    public bool IsFoundMoveable => _hitGroundForward && _hitGroundForward.collider.CompareTag("Moveable");
    private bool _isControllingMain => ControllingManager.Instance.IsControllingCat;
    private bool _isControllingCompBot => ControllingManager.Instance.IsControllingCompBot;

    public static Action<KeyCode, Collider2D, IControllableOnGround> OnAnyFoundMoveable;
    public static Action<KeyCode, IControllableOnGround> OnAnyNotFoundMoveable;
    public static Action<bool, bool, float> OnAnyGrabbing;

    void Start()
    {
        _playerRB = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (pusher != ControllingManager.Instance.CurrentControl) return;

        if (player is CompBotController && !ControllingManager.Instance.IsMatchedCompBot((CompBotController)player)) return;

        OnGrab();
        RayCheck();

        if (_isControllingMain || _isControllingCompBot)
        {
            EnableCountDownToInteract();
        }
        HandleNotOnGround();
    }
    private void OnGrab()
    {
        _CheckUnlockHoldingButton();

        if (_isKeyLock) return;

        if (!IsFoundMoveable || !player.IsGrounded) return;

        _isInteractHold = Input.GetKey(_key);
    }


    private void RayCheck()
    {
        if (player is PlayerController) 
        {
            _hitGroundForward = Physics2D.Raycast(
                origin: transform.position,
                direction: transform.right,
                distance: grabDistance,
                layerMask: groundLayer
            );
        }

        if (player is CompBotController) 
        {
            _hitGroundForward = Physics2D.Raycast(
                origin: transform.position,
                direction: Vector3.right * transform.localScale.x,
                distance: grabDistance,
                layerMask: groundLayer
            );
        }

        if (!IsFoundMoveable) 
        {
            OnAnyNotFoundMoveable?.Invoke(KEY_TO_HOLD, player);
            return;
        }

        OnAnyFoundMoveable?.Invoke(KEY_TO_HOLD, _hitGroundForward.collider, player);
    }

    private void HandleNotOnGround() 
    {
        if (IsFoundMoveable && player.IsGrounded) return;
        _ReleaseMoveable();

    } 

    private void HandleInteract() 
    {
        if (!IsGrabbing) { _GrabMoveable(); return; }

        _ReleaseMoveable();
    }

    private void EnableCountDownToInteract()
    {
        if (!IsFoundMoveable) return;

        _holdTimer = _isInteractHold ?
            _holdTimer - Time.deltaTime : HOLD_DOWN_TIME;

        _isReadyToInteract = _holdTimer <= 0;

        OnAnyGrabbing?.Invoke(IsGrabbing, _isInteractHold, _holdTimer);

        if (_isReadyToInteract)
        {
            _isReadyToInteract = false;
            _LockInteractKey();
            HandleInteract();
        }
    }

    private void _LockInteractKey()
    {
        _isInteractHold = false;
        _isKeyLock = true;
    }
    private void _CheckUnlockHoldingButton()
    {
        if (Input.GetKeyUp(_key)) _isKeyLock = false;
    }

    private void _GrabMoveable()
    {
        _moveableObject = IsFoundMoveable ? _hitGroundForward.collider.gameObject : null;
        if (!_moveableObject) return;

        _moveableObject.GetComponent<FixedJoint2D>().enabled = true;
        _moveableObject.GetComponent<FixedJoint2D>().connectedBody = _playerRB;
    }

    private void _ReleaseMoveable()
    {
        if (!IsGrabbing) return;

        _moveableObject.GetComponent<FixedJoint2D>().enabled = false;
        _moveableObject.GetComponent<FixedJoint2D>().connectedBody = _playerRB;
        _moveableObject = null;
    }

    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        if (player is PlayerController) 
        {
            Gizmos.DrawLine(transform.position, (Vector2) transform.position + Vector2.right * transform.right.x * grabDistance);
        }

        if (player is CompBotController)
        {
            Gizmos.DrawLine(transform.position, (Vector2) transform.position + Vector2.right * transform.localScale.x * grabDistance);
        }
    }

    
}
