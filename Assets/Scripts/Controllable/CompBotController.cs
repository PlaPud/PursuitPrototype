using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompBotController : MonoBehaviour
{
    enum ClimbPlane {
        Ceiling, Ground, LeftWall, RightWall
    }

    [Header("Controlling")]
    [SerializeField] float walkSpeed;
    [SerializeField] float maxAccelerate;
    [SerializeField] float maxDeccelerate;
    [SerializeField] float frictionAmount;
    [SerializeField] float velocityPower;
    [SerializeField] float jumpSpeed;

    [Header("Gravity")]
    [SerializeField] float lowJumpMultiplier;
    [SerializeField] float fallMultiplier;
    
    [Header("Raycast")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Vector2 boxSize;
    [SerializeField] float castDistance;

    private float _walkInput;
    private bool _isJumpPressed;
    private bool _isGrounded;
    
    private RaycastHit2D _groundHit;
    
    private Rigidbody2D _compBotRigidBody;
    private Animator _compBotAnimator;

    private String _currentAnimationState;
    private const String COMPBOT_IDLE = "CompBotIdle";
    private const String COMPBOT_WALK = "CompBotWalk";
    private const String COMPBOT_JUMP = "CompBotJump";

    private float _gravityScale;
    private Vector2 _gravityDirection = Vector2.down;

    private ClimbPlane _currentPlane = ClimbPlane.Ground;

    private void Awake()
    {
        _compBotRigidBody = GetComponent<Rigidbody2D>();
        _compBotAnimator = GetComponent<Animator>();
    }

    void Start()
    {
        //gameObject.SetActive(false);
        _gravityScale = _compBotRigidBody.gravityScale;
        _compBotRigidBody.gravityScale = 0;
        _currentPlane = ClimbPlane.Ground; 
    }

    void Update()
    {
        BoolStatusCheck();
        
        OnWalk();
        OnJump();

        CheckGravityDirection();
        HandleIdle();
        HandleWalk();
        HandleJump();
        HandleSpriteRotate();
        HandleFlipSprite();
        
        _compBotRigidBody.AddForce(_gravityDirection * _gravityScale);
    }

    private void FixedUpdate()
    {
    }

    private void CheckGravityDirection()
    {
        Vector2[] directions = new Vector2[4];
        directions[0] = Vector2.right;
        directions[1] = Vector2.up;
        directions[2] = Vector2.left;
        directions[3] = Vector2.down;

        foreach (Vector2 dir in directions) 
        {
            if (Physics2D.Raycast(transform.position, dir, castDistance, groundLayer));
        }
    }

    private void BoolStatusCheck()
    {
        _groundHit = Physics2D.Raycast(
                        origin: transform.position,
                        direction: -transform.up, distance: castDistance,
                        layerMask: groundLayer
                   );
        _isGrounded = _groundHit;
        
        Debug.DrawRay(transform.position, new Vector2(castDistance, 0), Color.blue);
        Debug.DrawRay(transform.position, new Vector2(0, castDistance), Color.red);
        Debug.DrawRay(transform.position, new Vector2(0, castDistance), Color.green);

    }

    private void HandleSwing()
    {
        
    }

    private void HandleJump()
    {
        if (_isGrounded && _isJumpPressed)
        {
            _compBotRigidBody.AddForce(
                    _groundHit.normal.normalized * jumpSpeed, 
                    ForceMode2D.Impulse
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
        float resultSpeed = _walkInput * walkSpeed;

        if (_currentPlane == ClimbPlane.Ground || _currentPlane == ClimbPlane.Ceiling)
        {
            float accel = (Mathf.Abs(resultSpeed) > .01f ? maxAccelerate : maxDeccelerate);
            float speedDif = resultSpeed - _compBotRigidBody.velocity.x;

            float movement = Mathf.Pow(
                    Mathf.Abs(speedDif) * accel, velocityPower
                ) * Mathf.Sign(speedDif);
            _compBotRigidBody.AddForce(movement * Vector2.right);
            if (_isGrounded && Mathf.Abs(_compBotRigidBody.velocity.x) > 0.5f)
            {
                ChangeAnimationState(COMPBOT_WALK);
            }
        }
        else 
        {
            float accel = (Mathf.Abs(resultSpeed) > .01f ? maxAccelerate : maxDeccelerate);
            float speedDif = resultSpeed - _compBotRigidBody.velocity.y;

            float movement = Mathf.Pow(
                    Mathf.Abs(speedDif) * accel, velocityPower
                ) * Mathf.Sign(speedDif);
            _compBotRigidBody.AddForce(movement * Vector2.up);
            if (_isGrounded && Mathf.Abs(_compBotRigidBody.velocity.y) > 0.5f)
            {
                ChangeAnimationState(COMPBOT_WALK);
            }
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
    private void HandleGravity()
    {
        if (_compBotRigidBody.velocity.y < 0)
        {
            _compBotRigidBody.velocity +=
                Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1)
                * Time.deltaTime;
        }
        else if (_compBotRigidBody.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            _compBotRigidBody.velocity +=
                Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1)
                * Time.deltaTime;
        }
    }
    private void HandleFlipSprite()
    {
        
    }

    private void HandleSpriteRotate() 
    {
        
    }

    private void OnWalk()
    {
        if (_currentPlane == ClimbPlane.LeftWall || _currentPlane == ClimbPlane.RightWall)
        {
            _walkInput = Input.GetAxis("Vertical");
            return;
        }
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
        //Gizmos.DrawLine(transform.position, new Vector3(
        //        transform.position.x + castDistanceWall, 
        //        transform.position.y,
        //        transform.position.z
        //    ));
        //Gizmos.DrawLine(transform.position, new Vector3(
        //        transform.position.x - castDistanceWall, 
        //        transform.position.y,
        //        transform.position.z
        //    ));
        //Gizmos.DrawLine(transform.position, new Vector3(
        //        transform.position.x, 
        //        transform.position.y + castDistanceCeil,
        //        transform.position.z
        //    ));
        //Gizmos.DrawLine(transform.position, new Vector3(
        //        transform.position.x,
        //        transform.position.y - castDistanceGround,
        //        transform.position.z
        //    ));
    }

    IEnumerator DisableInputCoroutine()
    {
        _compBotRigidBody.velocity = Vector2.zero;
        yield return new WaitForSeconds(.4f);
    }
}
