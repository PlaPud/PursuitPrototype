using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

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
    [SerializeField] float castDistance;

    [Header("Grappling Hook")]
    [SerializeField] AimingController _aimingController;
    [SerializeField] float maxDistance;
    [SerializeField] float grapplingSpeed;

    [Header("Rope Animation")]
    [SerializeField] int resolution;
    [SerializeField] int wobbleCounts;
    [SerializeField] float waveSize;
    [SerializeField] float animationSpeed;

    private bool _enableInput = true;

    private float _walkInput;
    private bool _isJumpPressed;
    private bool _isGrounded;
    private bool _isSwingPressed;
    private bool _isShootPressed;

    private bool _toJump;
    private bool _toShoot;

    private RaycastHit2D _groundHit;
    private RaycastHit2D _grapplerHit;

    private Vector2 _hookDirection;
    private Vector2 _hookPosition;
    private bool _isOnHook;
    private RaycastHit2D grapplerHitObject;
    private bool _isHit;
    private float _shootAngle;

    private Rigidbody2D _compBotRigidBody;
    private Animator _compBotAnimator;
    private LineRenderer _compBotLineRenderer;

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
        _compBotLineRenderer = GetComponent<LineRenderer>();
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

        if (_enableInput)
        {
            OnWalk();
            OnJump();
            OnShootHook();
        }

        HandleShootHook();

        HandleGravityState();
        HandleSpriteRotate();
        HandleFlipSprite();

    }

    private void FixedUpdate()
    {
        HandleIdle();
        HandleWalk();
        HandleJump();
        _compBotRigidBody.AddForce(_gravityDirection * _gravityScale);
    }

    private void HandleGravityState()
    {
        Vector2[] directions = new Vector2[4];
        directions[0] = Vector2.right;
        directions[1] = Vector2.up;
        directions[2] = Vector2.left;
        directions[3] = Vector2.down;

        foreach (Vector2 dir in directions)
        {
            if (!_isGrounded && Physics2D.Raycast(transform.position, dir, castDistance, groundLayer))
            {
                _gravityDirection = dir;
                if (dir == Vector2.up && !_isGrounded)
                {
                    _currentPlane = ClimbPlane.Ceiling;
                }
                else if (dir == Vector2.down)
                {
                    _currentPlane = ClimbPlane.Ground;
                }
                else if (dir == Vector2.right)
                {
                    _currentPlane = ClimbPlane.RightWall;
                }
                else if (dir == Vector2.left)
                {
                    _currentPlane = ClimbPlane.LeftWall;
                }
            }
        }
    }

    private void BoolStatusCheck()
    {
        _groundHit = Physics2D.Raycast(
                        origin: transform.position,
                        direction: _gravityDirection, distance: castDistance,
                        layerMask: groundLayer
                   );
        _grapplerHit = Physics2D.Raycast(
                    origin: transform.position,
                    direction: _aimingController.AimingCircleDirection.normalized * maxDistance,
                    distance: maxDistance,
                    layerMask: groundLayer
                );
        _isGrounded = _groundHit;

        Debug.DrawRay(transform.position, new Vector2(castDistance, 0), Color.blue);
        Debug.DrawRay(transform.position, new Vector2(-castDistance, 0), Color.yellow);
        Debug.DrawRay(transform.position, new Vector2(0, -castDistance), Color.red);
        Debug.DrawRay(transform.position, new Vector2(0, castDistance), Color.green);

    }
    private void OnShootHook()
    {

        _isShootPressed = Input.GetKeyDown(KeyCode.Mouse0);
        if (_isShootPressed && !_toShoot)
        {
            _hookPosition = _aimingController.AimingPositionGlobal;
            _hookDirection = _aimingController.AimingCircleDirection;
            _toShoot = true;
        }
    }
    private void HandleShootHook()
    {
        if (!_toShoot)
        {
            grapplerHitObject = _grapplerHit;
            _isHit = grapplerHitObject;
            _shootAngle = _aimingController.AimingAngle;
        }

        if (grapplerHitObject && _toShoot)
        {
            _enableInput = false;
            Vector2 target = grapplerHitObject.point;
            //StartCoroutine(RopeAnimationCoroutine(hitPoint: target));
            _compBotLineRenderer.SetPosition(0, transform.position);
            _compBotLineRenderer.SetPosition(1, target);
            _compBotRigidBody.MovePosition(
                    Vector2.MoveTowards(
                            current: transform.position,
                            target: target,
                            maxDistanceDelta: Time.deltaTime * grapplingSpeed
                        )
                );
            if (_isGrounded)
            {
                _enableInput = true;
                _toShoot = false;
                _compBotLineRenderer.SetPosition(0, Vector3.zero);
                _compBotLineRenderer.SetPosition(1, Vector3.zero);
            }
        }
    }

    private void HandleJump()
    {
        if (_isGrounded && _toJump)
        {
            _compBotRigidBody.AddForce(
                    _groundHit.normal.normalized * jumpSpeed,
                    ForceMode2D.Impulse
                );
            _toJump = false;
        }

        if (!_isGrounded)
        {
            ChangeAnimationState(COMPBOT_JUMP);
        }
    }

    private void HandleWalk()
    {
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
        if (_walkInput == 0 && _isGrounded)
        {
            ChangeAnimationState(COMPBOT_IDLE);
        }
    }

    private void HandleFlipSprite()
    {
        switch (_currentPlane)
        {
            case ClimbPlane.Ground:
                {
                    transform.localScale =
                        _compBotRigidBody.velocity.x > 0.5f ?
                        Vector3.one : _compBotRigidBody.velocity.x < -0.5f ?
                        new Vector3(-1, 1, 1) : transform.localScale;
                    break;
                }
            case ClimbPlane.Ceiling:
                {
                    transform.localScale =
                        _compBotRigidBody.velocity.x > 0.5f ?
                        new Vector3(-1, 1, 1) : _compBotRigidBody.velocity.x < -0.5f ?
                        Vector3.one : transform.localScale;
                    break;
                }
            case ClimbPlane.LeftWall:
                {
                    transform.localScale =
                        _compBotRigidBody.velocity.y > 0.5f ?
                        new Vector3(-1, 1, 1) : _compBotRigidBody.velocity.y < -0.5f ?
                        Vector3.one : transform.localScale;
                    break;
                }
            case ClimbPlane.RightWall:
                {
                    transform.localScale =
                            _compBotRigidBody.velocity.y > 0.5f ?
                            Vector3.one : _compBotRigidBody.velocity.y < -0.5f ?
                            new Vector3(-1, 1, 1) : transform.localScale;
                    break;
                }
        }
    }

    private void HandleSpriteRotate()
    {
        switch (_currentPlane)
        {
            case ClimbPlane.Ground:
                {
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                }
            case ClimbPlane.Ceiling:
                {
                    transform.rotation = Quaternion.Euler(0, 0, 180);
                    break;
                }
            case ClimbPlane.LeftWall:
                {
                    transform.rotation = Quaternion.Euler(0, 0, 270);
                    break;
                }
            case ClimbPlane.RightWall:
                {
                    transform.rotation = Quaternion.Euler(0, 0, 90);
                    break;
                }
        }
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
        if (_isJumpPressed && _isGrounded)
        {
            _toJump = true;
        }
    }


    private void ChangeAnimationState(string newAnimationState)
    {
        if (_currentAnimationState == newAnimationState) return;

        _compBotAnimator.Play(newAnimationState);

        _currentAnimationState = newAnimationState;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 3);
    }
}
