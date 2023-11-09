using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPushPull : MonoBehaviour
{
    private const float HOLD_DOWN_TIME = 1f;
    private const KeyCode KEY_TO_HOLD = KeyCode.E;

    [Header("Check Moveable")]
    [SerializeField] private float grabDistance;
    [SerializeField] private LayerMask groundLayer;

    [Header("Controllables")]
    [SerializeField] private IControllableOnGround player;


    private RaycastHit2D _hitGroundForward;
    private RaycastHit2D _hitGroundBehind;

    private GameObject _moveableObject;
    private Rigidbody2D _playerRB;

    private KeyCode _key = KEY_TO_HOLD;
    private bool _isInteractHold;
    private bool _isReadyToInteract;
    private bool _isKeyLock;
    private float _holdTimer = HOLD_DOWN_TIME;

    private float _localScaleXSign => Mathf.Sign(transform.localScale.x);
    public bool IsGrabbing => _moveableObject != null;
    public bool IsFoundMoveable => _hitGroundForward && _hitGroundForward.collider.CompareTag("Moveable");
    // Need To Check Controlling Which one Main or CompBot
    private bool _isControllingMain => ControllingManager.instance.CurrentControl == ControllingManager.Control.PlayerMain;

    void Start()
    {
        _playerRB = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        OnGrab();
        RayCheck();

        if (_isControllingMain)
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
        _hitGroundForward = Physics2D.Raycast(
            origin: transform.position,
            direction: Vector2.right * _localScaleXSign,
            distance: grabDistance,
            layerMask: groundLayer
        );
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
        _moveableObject = _hitGroundForward.collider.gameObject;
        if (!_moveableObject) return;

        _moveableObject.GetComponent<FixedJoint2D>().enabled = true;
        Debug.Log("Test");
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
        Gizmos.DrawLine(transform.position, (Vector2) transform.position + Vector2.right * transform.localScale.x * grabDistance);
    }

    
}
