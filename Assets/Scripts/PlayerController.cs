using System;
using System.Collections;
using UnityEngine;

public class PlayerController : IControllableOnGround, IDataPersistence
{
    
    [Header("Disable")]
    [SerializeField] bool isDisable;

    [Header("Controlling")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float jumpSpeed;

    [Header("Coyote Jump")]
    [SerializeField] private float coyoteJumpTime;

    [Header("Force and Accelaration")]
    [SerializeField] private float frictionAmount;
    [SerializeField] private float maxAccelerate;
    [SerializeField] private float maxDecelerate;
    [SerializeField] private float velocityPower;

    [Header("BoxCast Ceiling")]
    [SerializeField] private Vector2 boxSizeAbove;
    [SerializeField] private float aboveCastDistance;

    [field: Header("RopePoints")]
    [SerializeField] private LayerMask ropePointLayer;
    [field: SerializeField] public float PlayerRopeRadius { get; private set; }
    [SerializeField] private float ropeBoxWidth;
    [SerializeField] private float swingForce;
    [SerializeField] private float maxSwingSpeed;

    [Header("Wall Jump")]
    [SerializeField] private float wallJumpTime;
    [SerializeField] private float wallJumpForceX;
    [SerializeField] private float wallJumpForceY;
    [SerializeField] private float wallSlideSpeed;
    [SerializeField] private float wallDistance;

    [Header("Gravity")]
    [SerializeField] private float fallMultiplier;
    [SerializeField] private float lowJumpMultiplier;

    [SerializeField] private PlayerPushPull _playerPushPull;

    public LineRenderer PlayerRopeRenderer { get; private set; }
    public DistanceJoint2D PlayerRopeJoint { get; private set; }
    public RaycastHit2D IsSeeFrontRope { get; private set; }

    public float WalkInput { get; private set; } = 0f;
    public bool IsTryGetDown { get; private set; } = false;
    public bool IsSwingPressed { get; private set; } = false;
    public bool IsCrouching { get; private set; } = false;
    public bool IsWallSliding { get; private set; } = false;
    public bool IsSprinting { get; private set; } = false;
    public bool IsSpaceAbove { get; private set; } = true;
    public bool IsTouchWall { get; private set; } = false;
    public bool IsJumpFromWall { get; private set; } = false;

    private bool _disableX = false;
    private bool _isJumpPressed = false;
    private bool _isSprintPressed = false;
    private bool _isCrouchPressed = false;

    private bool _toJump = false;
    private bool _toToggleCrouch = false;

    private Rigidbody2D _playerRB;
    private Animator _playerAnimator;

    private const string PLAYER_IDLE = "PlayerIdle";
    private const string PLAYER_WALK = "PlayerWalk";
    private const string PLAYER_JUMP = "PlayerJump";
    private const string PLAYER_DROP = "PlayerDrop";
    private const string PLAYER_RUN = "PlayerRun";
    private const string PLAYER_CROUCH = "PlayerCrouchMove";
    private const string PLAYER_CROUCH_MOVE = "PlayerCrouchMoving";
    private const string PLAYER_WALL_SLIDE = "PlayerWallSlide";

    private const float MAGIC_COYOTEJUMP_NUMBER = 1.095f;

    private string _currentAnimationState = PLAYER_IDLE;
    private float _coyoteTimer;

    public Action OnPlayerFlipped;

    private void Awake()
    {
        _playerRB = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();
        PlayerRopeJoint = GetComponent<DistanceJoint2D>();
        PlayerRopeRenderer = GetComponent<LineRenderer>();
    }

    void Start()
    {
        PlayerRopeJoint.enabled = false;
        PlayerRopeRenderer.enabled = false;
        _coyoteTimer = coyoteJumpTime;
        if (!PlayerHealth.Instance) return;
        PlayerHealth.Instance.OnDamageTaken += KnockBackPlayer;
    }

    void Update()
    {
        BoolAndRayCheck();

        if (ControllingManager.Instance.IsControllingCat && !GameManager.Instance.IsPaused && !GameManager.Instance.IsFreezeControl)
        {
            _playerRB.constraints = RigidbodyConstraints2D.FreezeRotation;
            OnWalk();
            OnJump();
            OnSprint();
            OnCrouch();
            OnSwing();
        }
        else
        {
            _FreezePlayer();
        }

        if (!GameManager.Instance.IsPaused) AnimationStateMachineHandler();

        HandleDisable();
    }


    private void FixedUpdate()
    {
        HandleIdle();
        HandleHorizontalMoves();
        HandleJump();
        HandleCrouching();
        HandleWallSlide();
        HandleGravity();
        HandleFriction();
        HandleOnSwing();
        HandleFlipSprite();
    }

    private void BoolAndRayCheck()
    {
        IsSprinting = _isSprintPressed && !IsCrouching;
        RayCheck();
    }

    override public void RayCheck()
    {

        base.RayCheck();
        IsSpaceAbove = !Physics2D.BoxCast(
                origin: transform.position, size: boxSizeAbove, angle: 0f,
                direction: transform.up, distance: aboveCastDistance,
                layerMask: groundLayer
            );

        IsSeeFrontRope = Physics2D.BoxCast(
                origin: transform.position,
                size: new Vector2(ropeBoxWidth, 4 * PlayerRopeRadius),
                angle: 0f,
                direction: transform.right,
                distance: PlayerRopeRadius,
                layerMask: ropePointLayer
            );
    }
    private void OnWalk()
    {
        WalkInput = Input.GetAxis("Horizontal");
    }

    private void OnSprint()
    {
        if (_playerPushPull.IsFoundMoveable || IsJumpFromWall) 
        {
            _isSprintPressed = false;
            return;
        }

        _isSprintPressed = Input.GetKey(KeyCode.LeftShift);
    }

    private void OnJump()
    {
        _coyoteTimer = IsGrounded ? coyoteJumpTime : _coyoteTimer - Time.deltaTime;
        _isJumpPressed = Input.GetKeyDown(KeyCode.Space);
        if (!_playerPushPull.IsGrabbing && !IsCrouching && _isJumpPressed && !_toJump && (IsGrounded || IsTouchWall || _coyoteTimer > 0))
        {
            _toJump = true;
        }
    }

    private void OnCrouch()
    {
        _isCrouchPressed = Input.GetKeyDown(KeyCode.LeftControl);
        if (!_playerPushPull.IsGrabbing && IsGrounded && _isCrouchPressed && !_toToggleCrouch && !_toJump)
        {
            _toToggleCrouch = true;
        }
    }

    private void OnSwing()
    {
        IsSwingPressed = Input.GetKey(KeyCode.Mouse0);
    }
    private void HandleIdle() { }

    private void HandleHorizontalMoves()
    {
        if (_disableX || PlayerRopeJoint.enabled) return;

        float resultSpeed = WalkInput * (
                IsSprinting || IsJumpFromWall ? sprintSpeed
                : IsCrouching ? crouchSpeed
                : walkSpeed
            );
        
        float accel = (Mathf.Abs(WalkInput) > .01f ? maxAccelerate : maxDecelerate);
        float speedDif = resultSpeed - _playerRB.velocity.x;

        float movement = Mathf.Pow(
                Mathf.Abs(speedDif) * accel, velocityPower
            ) * Mathf.Sign(speedDif);

        _playerRB.AddForce(movement * Vector2.right);
    }

    private void HandleOnSwing() 
    {
        bool isSwingSpeedExceed = PlayerRopeJoint.enabled && Vector3.Magnitude(_playerRB.velocity) < maxSwingSpeed;

        if (!isSwingSpeedExceed) return;
        
            
        if (WalkInput > 0.5)
        {
            _playerRB.velocity += new Vector2 (swingForce * Time.deltaTime, 0f);
        }
        else if (WalkInput < -0.5) 
        {
            _playerRB.velocity -= new Vector2 (swingForce * Time.deltaTime, 0f);
        }

    }

    private void HandleGravity() 
    {
        if (IsSwingPressed && PlayerRopeJoint.enabled) 
        {
            _playerRB.AddForce(Vector2.down * 50f);
            return;
        };

        if (_playerRB.velocity.y < 0)
        {
            _playerRB.velocity +=
                Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1)
                * Time.deltaTime;
        }
        else if (_playerRB.velocity.y > 0 && !Input.GetButton("Jump")) 
        {
            _playerRB.velocity +=
                Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1)
                * Time.deltaTime;
        }
    }

    private void HandleJump()
    {
        if ((IsGrounded || _coyoteTimer > 0) && _toJump && !IsCrouching && !_playerPushPull.IsGrabbing)
        {
            _playerRB.velocity = new Vector2 (_playerRB.velocity.x, 0f);
            _playerRB.AddForce(
                Vector2.up * jumpSpeed * (IsGrounded ? 1f : MAGIC_COYOTEJUMP_NUMBER), 
                ForceMode2D.Impulse
            );
            _coyoteTimer = 0;

            AudioManager.Instance?.PlayOneShot(FMODEvents.Instance.PlayerJump, transform.position);
            _toJump = false;
        }
        else if (_toJump && IsWallSliding)
        {
            _playerRB.AddForce(
                new Vector2(
                        -transform.right.x
                        * wallJumpForceX,
                        wallJumpForceY
                    ),
                ForceMode2D.Impulse
            );

            StartCoroutine(_CheckUntilGround());

            AudioManager.Instance?.PlayOneShot(FMODEvents.Instance.PlayerJump, transform.position);
            _toJump = false;
        }
        
    }

    private IEnumerator _CheckUntilGround()
    {
        IsJumpFromWall = true;
        yield return new WaitUntil(() => IsGrounded);
        IsJumpFromWall = false;
    }

    private void HandleWallSlide()
    {
        bool isFacingRight = transform.right.x == 1;

        RaycastHit2D hitGround = Physics2D.Raycast(
                transform.position,
                new Vector2(
                        isFacingRight ?
                        wallDistance : -wallDistance, 0
                    ),
                wallDistance,
                groundLayer
            );
        IsTouchWall = hitGround && (
                hitGround.transform.gameObject.CompareTag("Climbable")
            );


        if (IsTouchWall && !IsGrounded && _playerRB.velocity.y < -0.5)
        {
            IsWallSliding = true;
        }
        else
        {
            IsWallSliding = false;
        }

        if (IsWallSliding && _playerRB.velocity.y < -wallSlideSpeed)
        {
            _playerRB.velocity = new Vector2(
                    _playerRB.velocity.x,
                    -wallSlideSpeed
                );
        }

    }
    private void HandleCrouching()
    {
        if (!_playerPushPull.IsGrabbing && IsGrounded && !IsSprinting && _toToggleCrouch)
        {
            _toToggleCrouch = false;
            if (!IsSpaceAbove) return;
            IsCrouching = !IsCrouching;
        }
    }

    private void HandleFlipSprite()
    {
        if (_playerPushPull.IsGrabbing) return;

        if (transform.rotation.y != 0f && _playerRB.velocity.x > 0.5f && WalkInput > .001f)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            OnPlayerFlipped?.Invoke();
            return;
        }

        if (transform.rotation.y == 0f && _playerRB.velocity.x < -0.5f && WalkInput < -.001f)
        {
            transform.rotation = Quaternion.Euler(0, 180f, 0);
            OnPlayerFlipped?.Invoke();
            return;
        }

    }

    private void HandleFriction()
    {
        if (!IsGrounded || Mathf.Abs(WalkInput) >= .01f) return;

        float appliedFriction = Mathf.Min(
                Mathf.Abs(_playerRB.velocity.x),
                frictionAmount
            ) * Mathf.Sign(_playerRB.velocity.x);
        _playerRB.AddForce(Vector2.right * -appliedFriction, ForceMode2D.Impulse);
    }

    private void KnockBackPlayer(int damage) 
    {
        if (damage <= 0) return;
        StartCoroutine(_KnockBack());
    }

    private IEnumerator _KnockBack() 
    {
        _playerRB.velocity = Vector3.zero;  
        yield return new WaitForSeconds(.02f);
        _playerRB.velocity = Vector2.up * 10f;
    }
    private void _FreezePlayer()
    {
        WalkInput = 0f;

        if (GameManager.Instance.IsPaused)
        {
            _playerRB.constraints = RigidbodyConstraints2D.FreezeAll;
            return;
        }

        _playerRB.constraints =
              RigidbodyConstraints2D.FreezeRotation
            | RigidbodyConstraints2D.FreezePositionX;
    }

    private void ChangeAnimationState(string newAnimationState) 
    {
        if (_currentAnimationState == newAnimationState) return;

        _playerAnimator.Play(newAnimationState);

        _currentAnimationState = newAnimationState;
    }

    private void HandleDisable()
    {
        gameObject.SetActive(!isDisable);
    }

    private void AnimationStateMachineHandler()
    {
        if (IsGrounded)
        {
            switch (_currentAnimationState)
            {
                case PLAYER_IDLE:
                    if (IsCrouching)
                    {
                        ChangeAnimationState(PLAYER_CROUCH);
                    }
                    if (Mathf.Abs(WalkInput) > 0.5f)
                    {
                        ChangeAnimationState(PLAYER_WALK);
                    }
                    break;
                case PLAYER_CROUCH:
                    if (Mathf.Abs(WalkInput) > 0.5f)
                    {
                        ChangeAnimationState(PLAYER_CROUCH_MOVE);
                    }
                    if (!IsCrouching)
                    {
                        ChangeAnimationState(PLAYER_IDLE);
                    }
                    break;
                case PLAYER_CROUCH_MOVE:
                    if (!IsCrouching)
                    {
                        ChangeAnimationState(PLAYER_IDLE);
                    }
                    if (WalkInput == 0)
                    {
                        ChangeAnimationState(PLAYER_CROUCH);
                    }
                    break;
                case PLAYER_WALK:
                    if (WalkInput == 0)
                    {
                        ChangeAnimationState(PLAYER_IDLE);
                    }
                    if (IsCrouching)
                    {
                        ChangeAnimationState(PLAYER_CROUCH);
                    }
                    if (_isSprintPressed)
                    {
                        ChangeAnimationState(PLAYER_RUN);
                    }
                    break;
                case PLAYER_RUN:
                    if (!_isSprintPressed && Mathf.Abs(WalkInput) > 0.5f)
                    {
                        ChangeAnimationState(PLAYER_WALK);
                    }
                    if (WalkInput == 0) 
                    {
                        ChangeAnimationState(PLAYER_IDLE);
                    }
                    break;
                default:
                    ChangeAnimationState(PLAYER_IDLE);
                    break;
            }
        }
        else
        {
            switch (_currentAnimationState)
            {
                case PLAYER_DROP:
                    if (_playerRB.velocity.y > 1f)
                    {
                        ChangeAnimationState(PLAYER_JUMP);
                    }
                    if (IsWallSliding)
                    {
                        ChangeAnimationState(PLAYER_WALL_SLIDE);
                    }
                    break;
                case PLAYER_JUMP:
                    if (_playerRB.velocity.y < -1f)
                    {
                        ChangeAnimationState(PLAYER_DROP);
                    }
                    if (IsWallSliding)
                    {
                        ChangeAnimationState(PLAYER_WALL_SLIDE);
                    }
                    break;
                case PLAYER_WALL_SLIDE:
                    if (!IsWallSliding)
                    {
                        ChangeAnimationState(PLAYER_DROP);
                    }
                    break;
                default:
                    ChangeAnimationState(PLAYER_DROP);
                    break;

            }
        }
    }

    public void LoadData(GameData data)
    {
        transform.position = data.PlayerPos.ToUnityVector3();
    }

    public void SaveData(GameData data)
    {
        data.PlayerPos = new Vector3Serialize(transform.position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + Vector3.down * castDistance, boxSize);
        Gizmos.DrawWireCube(transform.position + Vector3.up * aboveCastDistance, boxSizeAbove);
        Gizmos.DrawWireCube(
                    center: transform.position + transform.right * (PlayerRopeRadius),
                    size: new Vector2(ropeBoxWidth, 2 * PlayerRopeRadius)
                );
    }

}
