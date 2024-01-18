using FMOD.Studio;
using System;
using System.Collections;
using UnityEngine;

public class CompBotController : IControllableOnGround
{
    enum ClimbPlane {
        Ceiling, Ground, LeftWall, RightWall
    }

    [Header("Control Panel")]
    [SerializeField] private CompBotPanelController panel;

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

    [Header("Grappling Hook")]
    [SerializeField] AimingController _aimingController;
    [SerializeField] float maxGrapplingDistance;
    [SerializeField] float grapplingSpeed;

    [Header("Rope Animation")]
    [SerializeField] int resolution;
    [SerializeField] int wobbleCounts;
    [SerializeField] float waveSize;
    [SerializeField] float animationSpeed;

    private bool _enableInput = true;

    private float _walkInput;
    private bool _isJumpPressed;

    private bool _toJump;
    private bool _isLockTilRelease;

    private RaycastHit2D _groundHit;
    public RaycastHit2D GrapplerHit { get; private set; }

    private RaycastHit2D grapplerHitObject;

    private Vector2 _hookDirection;
    private Vector2 _hookPosition;

    public bool IsShootHold { get; private set; }
    public bool IsOnHook { get; private set; }

    private float _shootAngle;

    private Rigidbody2D _compBotRB;
    private Animator _compBotAnimator;
    private LineRenderer _compBotLineRenderer;
    private PlayerPushPull _playerPushPull;

    private string _currentAnimationState;
    private const string COMPBOT_IDLE = "CompBotIdle";
    private const string COMPBOT_WALK = "CompBotWalk";
    private const string COMPBOT_JUMP = "CompBotJump";

    private float _gravityScale;
    private Vector2 _gravityDirection = Vector2.down;

    private ClimbPlane _currentPlane = ClimbPlane.Ground;

    private EventInstance _shootHookSound;

    public bool IsControlling => panel.IsControllingThis;

    private void Awake()
    {
        _compBotRB = GetComponent<Rigidbody2D>();
        _compBotAnimator = GetComponent<Animator>();
        _compBotLineRenderer = GetComponent<LineRenderer>();
        _playerPushPull = GetComponent<PlayerPushPull>();
    }

    void Start()
    {
        _gravityScale = _compBotRB.gravityScale;
        _compBotRB.gravityScale = 0;
        _currentPlane = ClimbPlane.Ground;

        _shootHookSound = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.CompBotShootHook);
    }

    void Update()
    {
        RayCheck();

        if (!IsControlling || GameManager.Instance.IsPaused) _FreezeCompBot();

        if (!CompBotManager.Instance.IsControlCompBot || !IsControlling || GameManager.Instance.IsPaused)
        {
            _ResetBools();
            return;
        }

        _FreezeRotation();

        OnWalk();
        OnJump();
        OnShootHook();

        CheckGravityState();

        HandleSpriteRotate();
        HandleFlipSprite();

    }

    private void FixedUpdate()
    {
        HandleIdle();
        HandleWalk();
        HandleJump();
        HandleShootHook();
        HandleGravity();

        UpdateSound();
    }

    private void CheckGravityState()
    {
        Vector2[] directions = new Vector2[4];
        directions[0] = Vector2.right;
        directions[1] = Vector2.up;
        directions[2] = Vector2.left;
        directions[3] = Vector2.down;

        foreach (Vector2 dir in directions)
        {
            RaycastHit2D wallHitCD = Physics2D
                .Raycast(transform.position, dir, castDistance, groundLayer);

            if (!IsGrounded && _IsRayHitWall(wallHitCD))
            {
                _gravityDirection = dir;
                if (dir == Vector2.up && !IsGrounded)
                {
                    _currentPlane = ClimbPlane.Ceiling;
                    _playerPushPull.enabled = false;
                }
                else if (dir == Vector2.down)
                {
                    _currentPlane = ClimbPlane.Ground;
                    _playerPushPull.enabled = true;
                }
                else if (dir == Vector2.right)
                {
                    _currentPlane = ClimbPlane.RightWall;
                    _playerPushPull.enabled = false;
                }
                else if (dir == Vector2.left)
                {
                    _currentPlane = ClimbPlane.LeftWall;
                    _playerPushPull.enabled = false;
                }
            }
        }
    }

    override public void RayCheck()
    {
        _groundHit = Physics2D.Raycast(
                        origin: transform.position,
                        direction: _gravityDirection, distance: castDistance,
                        layerMask: groundLayer
                   );

        if (!IsOnHook) 
        {
            GrapplerHit = Physics2D.Raycast(
                        origin: transform.position,
                        direction: _aimingController.AimingCircleDirection.normalized,
                        distance: maxGrapplingDistance,
                        layerMask: groundLayer
                    );
        }

        IsGrounded = _groundHit;
    }

    private void OnWalk()
    {
        if (!_enableInput) return;

        if (_currentPlane == ClimbPlane.LeftWall || _currentPlane == ClimbPlane.RightWall)
        {
            _walkInput = Input.GetAxis("Vertical");
            return;
        }
        _walkInput = Input.GetAxis("Horizontal");
    }

    private void OnJump()
    {
        if (!_enableInput) return;

        _isJumpPressed = Input.GetKeyDown(KeyCode.Space);
        if (_isJumpPressed && IsGrounded)
        {
            _toJump = true;
        }
    }

    private void OnShootHook()
    {
        IsShootHold = Input.GetKey(KeyCode.Mouse0);
        _LockIfNotHit();
    }

    private void _LockIfNotHit()
    {
        if (IsShootHold && !GrapplerHit)
        {
            _isLockTilRelease = true;
        }

        if (_isLockTilRelease && !IsShootHold)
        {
            _isLockTilRelease = false;
        }
    }

    private void HandleGravity() => _compBotRB.AddForce(_gravityDirection * _gravityScale);
 
    private void HandleShootHook()
    {
        if (_isLockTilRelease) return;

        if (IsShootHold && !IsOnHook) 
        {
            IsOnHook = true;
            grapplerHitObject = GrapplerHit;
        }

        if (IsShootHold && IsOnHook)
        {
            _enableInput = false;

            Vector2 target = grapplerHitObject.point;
            _SetLineToTarget(target);

            _compBotRB.MovePosition(
                Vector2.MoveTowards(
                    current: transform.position,
                    target: target,
                    maxDistanceDelta: Time.deltaTime * grapplingSpeed
                )
            );
        }

        if (!IsShootHold && IsOnHook)
        {
            _enableInput = true;
            IsOnHook = false;
            _RemoveAndDisableLine();
        }

    }

    private void HandleJump()
    {
        if (IsGrounded && _toJump)
        {
            _compBotRB.AddForce(
                    _groundHit.normal.normalized * jumpSpeed,
                    ForceMode2D.Impulse
                );
            _toJump = false;
        }

        if (!IsGrounded)
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
            float speedDif = resultSpeed - _compBotRB.velocity.x;

            float movement = Mathf.Pow(
                    Mathf.Abs(speedDif) * accel, velocityPower
                ) * Mathf.Sign(speedDif);
            _compBotRB.AddForce(movement * Vector2.right);
            if (IsGrounded && Mathf.Abs(_compBotRB.velocity.x) > 0.5f)
            {
                ChangeAnimationState(COMPBOT_WALK);
            }
        }
        else
        {
            float accel = (Mathf.Abs(resultSpeed) > .01f ? maxAccelerate : maxDeccelerate);
            float speedDif = resultSpeed - _compBotRB.velocity.y;

            float movement = Mathf.Pow(
                    Mathf.Abs(speedDif) * accel, velocityPower
                ) * Mathf.Sign(speedDif);
            _compBotRB.AddForce(movement * Vector2.up);
            if (IsGrounded && Mathf.Abs(_compBotRB.velocity.y) > 0.5f)
            {
                ChangeAnimationState(COMPBOT_WALK);
            }
        }

    }
    
    private void HandleIdle()
    {
        if (Vector3.Magnitude(_compBotRB.velocity) < 0.5f  && IsGrounded)
        {
            ChangeAnimationState(COMPBOT_IDLE);
        }
    }
    
    private void HandleFlipSprite()
    {
        if (_playerPushPull.IsGrabbing) return;

        switch (_currentPlane)
        {
            case ClimbPlane.Ground:
                {
                    transform.localScale =
                        _compBotRB.velocity.x > 0.5f ?
                        Vector3.one : _compBotRB.velocity.x < -0.5f ?
                        new Vector3(-1, 1, 1) : transform.localScale;
                    break;
                }
            case ClimbPlane.Ceiling:
                {
                    transform.localScale =
                        _compBotRB.velocity.x > 0.5f ?
                        new Vector3(-1, 1, 1) : _compBotRB.velocity.x < -0.5f ?
                        Vector3.one : transform.localScale;
                    break;
                }
            case ClimbPlane.LeftWall:
                {
                    transform.localScale =
                        _compBotRB.velocity.y > 0.5f ?
                        new Vector3(-1, 1, 1) : _compBotRB.velocity.y < -0.5f ?
                        Vector3.one : transform.localScale;
                    break;
                }
            case ClimbPlane.RightWall:
                {
                    transform.localScale =
                            _compBotRB.velocity.y > 0.5f ?
                            Vector3.one : _compBotRB.velocity.y < -0.5f ?
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

    private void ChangeAnimationState(string newAnimationState)
    {
        if (_currentAnimationState == newAnimationState) return;

        _compBotAnimator.Play(newAnimationState);

        _currentAnimationState = newAnimationState;
    }

    private void _RemoveAndDisableLine()
    {
        _compBotLineRenderer.SetPosition(0, Vector3.zero);
        _compBotLineRenderer.SetPosition(1, Vector3.zero);
        _compBotLineRenderer.enabled = false;
    }

    private void _SetLineToTarget(Vector2 target)
    {
        _compBotLineRenderer.enabled = true;
        _compBotLineRenderer.SetPosition(0, transform.position);
        _compBotLineRenderer.SetPosition(1, target);
    }
    private void _ResetBools()
    {
        _toJump = false;
        _isJumpPressed = false;
        IsShootHold = false;
    }
    private void _FreezeCompBot()
    {
        _walkInput = 0f;
        _compBotRB.constraints = 
              RigidbodyConstraints2D.FreezeRotation 
            | RigidbodyConstraints2D.FreezePositionX;
    }
    
    private void _FreezeRotation() => _compBotRB.constraints = RigidbodyConstraints2D.FreezeRotation;
    
    private bool _IsRayHitWall(RaycastHit2D hit) => hit && hit.collider && !hit.collider.CompareTag("Moveable");

    private void UpdateSound() 
    {
        if (!ControllingManager.Instance.IsControllingCompBot)
        {
            PLAYBACK_STATE playbackState;
            _shootHookSound.getPlaybackState(out playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.PLAYING) || playbackState.Equals(PLAYBACK_STATE.STARTING))
            {
                _shootHookSound.stop(STOP_MODE.ALLOWFADEOUT);
            }
            return;
        }

        if (IsShootHold && IsOnHook) 
        {
            PLAYBACK_STATE playbackState;
            _shootHookSound.getPlaybackState(out playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                _shootHookSound.start();
            }
            return;
        }

        _shootHookSound.stop(STOP_MODE.ALLOWFADEOUT);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, maxGrapplingDistance);
    }

}
