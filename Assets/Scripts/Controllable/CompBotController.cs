using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompBotController : MonoBehaviour
{
    [Header("Controlling")]
    [SerializeField] float walkSpeed;
    [SerializeField] float jumpSpeed;

    [Header("Gravity")]
    [SerializeField] float lowJumpMultiplier;
    [SerializeField] float fallMultiplier;
    
    [Header("Raycast Ground")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Vector2 boxSize;
    [SerializeField] float castDistance;

    private float _walkInput;
    private bool _isJumpPressed;
    private bool _isGrounded;
    private bool _isSwingPressed;
    
    
    private Rigidbody2D _compBotRigidBody;
    private Animator _compBotAnimator;

    private String _currentAnimationState;
    private const String COMPBOT_IDLE = "CompBotIdle";
    private const String COMPBOT_WALK = "CompBotWalk";
    private const String COMPBOT_JUMP = "CompBotJump";

    private void Awake()
    {
        _compBotRigidBody = GetComponent<Rigidbody2D>();
        _compBotAnimator = GetComponent<Animator>();
    }

    void Start()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        BoolStatusCheck();
        
        OnWalk();
        OnJump();

        HandleIdle();
        HandleWalk();
        HandleJump();
        HandleSwing();
    }

    private void BoolStatusCheck()
    {
        _isGrounded = Physics2D.BoxCast(
                        origin: transform.position, size: boxSize, angle: 0,
                        direction: -transform.up, distance: castDistance,
                        layerMask: groundLayer
                   );
    }

    private void HandleSwing()
    {
    
    }

    private void HandleJump()
    {
        if (_isGrounded && _isJumpPressed)
        {
            _compBotRigidBody.velocity = new Vector2(
                        _compBotRigidBody.velocity.x,
                        jumpSpeed
                    );
        }

        if (!_isGrounded)
        {
            ChangeAnimationState(COMPBOT_JUMP);
        }
    }

    private void HandleWalk()
    {
        //if (_disableX || _playerRopeJoint.enabled) return;
        _compBotRigidBody.velocity = new Vector2(
                        _walkInput * walkSpeed,
                        _compBotRigidBody.velocity.y
                    );
        if (_isGrounded && Mathf.Abs(_compBotRigidBody.velocity.x) > 0.5f) 
        {
            ChangeAnimationState(COMPBOT_WALK);
        }
    }

    private void HandleIdle()
    {
        bool isNotInAir = !(Mathf.Abs(_compBotRigidBody.velocity.y) > 0.5);

        if (_walkInput == 0 && isNotInAir)
        {
            ChangeAnimationState(COMPBOT_IDLE);
        }
    }

    private void OnWalk()
    {
        _walkInput = Input.GetAxis("Horizontal");
    }
    private void OnJump()
    {
        _isJumpPressed = Input.GetKeyDown(KeyCode.Space);
    }
    private void OnSwing()
    {
        _isSwingPressed = Input.GetKey(KeyCode.Mouse0);
    }

    private void ChangeAnimationState(string newAnimationState)
    {
        if (_currentAnimationState == newAnimationState) return;

        _compBotAnimator.Play(newAnimationState);

        _currentAnimationState = newAnimationState;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position - transform.up * castDistance, boxSize);
    }

}
