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
    [SerializeField] float jumpSpeed;

    [Header("Gravity")]
    [SerializeField] float lowJumpMultiplier;
    [SerializeField] float fallMultiplier;
    
    [Header("Raycast")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Vector2 boxSize;
    [SerializeField] float castDistanceGround;
    [SerializeField] float castDistanceCeil;
    [SerializeField] float castDistanceWall;

    private float _walkInput;
    private bool _isJumpPressed;
    private bool _isGrounded;
    private bool _isTouchCeil;
    private bool _isTouchWall;
    private bool _isSwingPressed;
    
    
    private Rigidbody2D _compBotRigidBody;
    private Animator _compBotAnimator;

    private String _currentAnimationState;
    private const String COMPBOT_IDLE = "CompBotIdle";
    private const String COMPBOT_WALK = "CompBotWalk";
    private const String COMPBOT_JUMP = "CompBotJump";

    private float _gravityScale;
    private Vector2 _gravityForce;

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

        HandleIdle();
        HandleFlipSprite();
        HandleSpriteRotate();
        HandleWalk();
        HandleJump();
        HandleSwing();
        //HandleGravity();
    }

    private void FixedUpdate()
    {
        ChangeGravityDirection();
        _compBotRigidBody.AddForce(_gravityForce);
    }

    private void ChangeGravityDirection()
    {
        Debug.Log(transform.localEulerAngles.z);
        switch (_currentPlane)
        {
            case ClimbPlane.Ground:
                _gravityForce = new Vector2(0, -3 * _gravityScale);
                if (transform.localEulerAngles.z == 90) 
                {
                    _currentPlane = ClimbPlane.RightWall;
                }
                if (transform.localEulerAngles.z == 270)
                {
                    _currentPlane = ClimbPlane.LeftWall;
                    StartCoroutine(DisableInputCoroutine());
                }
                if (transform.localEulerAngles.z == 180)
                { 
                    _currentPlane = ClimbPlane.Ceiling;
                }
                break;
            case ClimbPlane.LeftWall:
                _gravityForce = new Vector2(-3 * _gravityScale, 0);
                if (transform.localEulerAngles.z == 90)
                {
                    _currentPlane = ClimbPlane.Ground;
                }
                if (transform.localEulerAngles.z == -90)
                {
                    _currentPlane = ClimbPlane.Ceiling;
                }
                if (transform.localEulerAngles.z == 180)
                {
                    _currentPlane = ClimbPlane.RightWall;
                }
                break;
            case ClimbPlane.RightWall:
                _gravityForce = new Vector2(3 * _gravityScale, 0);
                if (transform.localEulerAngles.z == 90)
                {
                    _currentPlane = ClimbPlane.Ceiling;
                }
                if (transform.localEulerAngles.z == -90)
                {
                    _currentPlane = ClimbPlane.Ground;
                }
                if (transform.localEulerAngles.z == 180)
                {
                    _currentPlane = ClimbPlane.LeftWall;
                }
                break;
            case ClimbPlane.Ceiling:
                _gravityForce = new Vector2(0, 3 * _gravityScale);
                if (transform.localEulerAngles.z == 90)
                {
                    _currentPlane = ClimbPlane.LeftWall;
                }
                if (transform.localEulerAngles.z == -90)
                {
                    _currentPlane = ClimbPlane.RightWall;
                }
                if (transform.localEulerAngles.z == 180)
                {
                    _currentPlane = ClimbPlane.Ground;
                }
                break;
        }
    }

    private void BoolStatusCheck()
    {
        _isGrounded = Physics2D.Raycast(
                        origin: transform.position,
                        direction: -transform.up, distance: castDistanceGround,
                        layerMask: groundLayer
                   );
        _isTouchCeil = Physics2D.Raycast(
                origin: transform.position,
                direction: transform.up, distance: castDistanceCeil,
                layerMask: groundLayer
            );
        _isTouchWall = Physics2D.Raycast(
                transform.position,
                new Vector2(
                        transform.localScale.x == 1 ?
                        castDistanceWall : -castDistanceWall, 0
                    ),
                castDistanceWall,
                groundLayer
            ); ;
        Debug.DrawRay(transform.position, new Vector2(castDistanceWall, 0), Color.blue);
        Debug.DrawRay(transform.position, new Vector2(0, castDistanceCeil), Color.blue);
        Debug.DrawRay(transform.position, new Vector2(0, castDistanceGround), Color.blue);

    }

    private void HandleSwing()
    {
        
    }

    private void HandleJump()
    {
        if (_isGrounded && _isJumpPressed)
        {
            _compBotRigidBody.AddForce(new Vector2(
                    0, jumpSpeed
                ), ForceMode2D.Impulse);
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
        if (_compBotRigidBody.velocity.x > Mathf.Epsilon)
        {
            transform.localScale = Vector3.one;
        }
        if (_compBotRigidBody.velocity.x < -Mathf.Epsilon)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void HandleSpriteRotate() 
    {

        if (_isTouchWall && transform.localScale.x == -1) 
        {
            transform.localRotation = Quaternion.Euler(0, 0, -90);
        }
        if (_isTouchWall && transform.localScale.x == 1) 
        {
            transform.localRotation = Quaternion.Euler(0, 0, 90);
        }
        if (_isTouchCeil && !_isTouchWall) 
        {
            transform.localRotation = Quaternion.Euler(0, 0, 180);
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
        Gizmos.DrawLine(transform.position, new Vector3(
                transform.position.x + castDistanceWall, 
                transform.position.y,
                transform.position.z
            ));
        Gizmos.DrawLine(transform.position, new Vector3(
                transform.position.x - castDistanceWall, 
                transform.position.y,
                transform.position.z
            ));
        Gizmos.DrawLine(transform.position, new Vector3(
                transform.position.x, 
                transform.position.y + castDistanceCeil,
                transform.position.z
            ));
        Gizmos.DrawLine(transform.position, new Vector3(
                transform.position.x,
                transform.position.y - castDistanceGround,
                transform.position.z
            ));
        Gizmos.DrawWireCube(transform.position - transform.up * castDistanceGround, boxSize);
        Gizmos.DrawWireCube(transform.position + transform.up * castDistanceCeil, boxSize);
    }

    IEnumerator DisableInputCoroutine()
    {
        _compBotRigidBody.velocity = Vector2.zero;
        yield return new WaitForSeconds(.4f);
    }
}
