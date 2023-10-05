using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClawMachineController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private GameObject clawBody;

    private float _moveX;
    private float _moveY;
    private bool _isHolding;
    private GameObject _holdingObject;

    private bool _toHold;

    private Rigidbody2D _clawBodyRB;
    private Animator _clawBodyAnimator;

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
        OnMove();
        OnToggleHold();
    }

    private void FixedUpdate()
    {
        HandleIdle();
        HandleMove();
        HandleHolding();
    }

    private void OnMove() 
    {
        _moveX = Input.GetAxisRaw("Horizontal");
        _moveY = Input.GetAxisRaw("Vertical");
    }

    private void OnToggleHold() 
    {
        bool _isToggleHold = Input.GetKey(KeyCode.E);
        if (_isToggleHold) _toHold = true;
        //if (_isToggleHold && _isObjectNear) 
        //{
        //    _toHold = true;
        //}
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
        if (!_toHold) 
        {
            ChangeAnimationState(CLAW_EMPTY);
            return;
        }

        ChangeAnimationState(CLAW_HOLD);
    }

    void ChangeAnimationState(string newState) 
    {
        if (_currentAnimationState == newState) return;
        _currentAnimationState = newState;
        _clawBodyAnimator.Play(newState);
    }
}
