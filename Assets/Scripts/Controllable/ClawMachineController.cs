using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClawMachineController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float detectionRadius;
    [SerializeField] private GameObject clawBody;
    [SerializeField] Transform holdingPoint;
    [SerializeField] private LayerMask movableLayer;

    private float _moveX;
    private float _moveY;
    private bool _isHolding;

    private GameObject _holdingObject;

    private bool _toToggleHold;

    private Rigidbody2D _clawBodyRB;
    private Animator _clawBodyAnimator;
    private Collider2D _enteredMovable;

    string _currentAnimationState = CLAW_EMPTY;

    const string CLAW_EMPTY = "ClawEmpty";
    const string CLAW_HOLD = "ClawHold";

    private void Awake()
    {
        _clawBodyRB = clawBody.GetComponent<Rigidbody2D>();
        _clawBodyAnimator = clawBody.GetComponent<Animator>();
    }

    void Start()
    {

    }

    void Update()
    {
        RayCheck();
        OnMove();
        OnToggleHold();
    }

    private void FixedUpdate()
    {
        HandleIdle();
        HandleMove();
        HandleHolding();
    }

    private void RayCheck() 
    {
        _enteredMovable = Physics2D.OverlapCircle(
                point: holdingPoint.position,
                radius: detectionRadius,
                layerMask: movableLayer
            );
    }

    private void OnMove() 
    {
        _moveX = Input.GetAxisRaw("Horizontal");
        _moveY = Input.GetAxisRaw("Vertical");
    }

    private void OnToggleHold() 
    {
        bool _isToggleHold = Input.GetKeyDown(KeyCode.E);
        if (_isToggleHold) _toToggleHold = true;
    }

    private void HandleMove() 
    {
        _clawBodyRB.velocity = new Vector2(
                    _moveX * moveSpeed,
                    _moveY * moveSpeed
                );
    }

    private void HandleIdle() 
    {
        
    }

    private void HandleHolding() 
    {
        if (_toToggleHold && _enteredMovable && !_holdingObject) 
        {
            _toToggleHold = false;
            _isHolding = true;
            Debug.Log(_isHolding);
            ChangeAnimationState(CLAW_HOLD);
            _enteredMovable.transform.SetParent(holdingPoint);
            _enteredMovable.transform.position = holdingPoint.position;
            _holdingObject = _enteredMovable.gameObject;
            Rigidbody2D holdingRB = _enteredMovable.GetComponent<Rigidbody2D>();
            holdingRB.gravityScale = 0;
            return;
        }

        if (_toToggleHold && _holdingObject)
        {
            _toToggleHold = false;
            Rigidbody2D holdingRB = _holdingObject.GetComponent<Rigidbody2D>();
            holdingRB.gravityScale = 1;
            _holdingObject.transform.parent = null;
            ChangeAnimationState(CLAW_EMPTY);
            return;
        }

        if (_isHolding) 
        {
            _holdingObject.transform.localPosition = Vector3.zero;
        }
    }
        
    void ChangeAnimationState(string newState) 
    {
        if (_currentAnimationState == newState) return;
        _currentAnimationState = newState;
        _clawBodyAnimator.Play(newState);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(holdingPoint.position, detectionRadius);
    }
}
