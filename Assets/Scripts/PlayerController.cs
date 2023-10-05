using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] bool isDisable;

    [Header("Controlling")]
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float jumpSpeed;
    [SerializeField] float crouchSpeed;
    [SerializeField] float maxAccelerate;
    [SerializeField] float maxDeccelerate;
    [SerializeField] float velocityPower;
    [SerializeField] float frictionAmount;

    [Header("RayCast Ground")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Vector2 boxSize;
    [SerializeField] float castDistance;

    [Header("BoxCast Ceiling")]
    [SerializeField] Vector2 boxSizeAbove;
    [SerializeField] float aboveCastDistance;

    [field: Header("RopePoints")]
    [field: SerializeField] public float PlayerRopeRadius { get; private set; }
    [SerializeField] LayerMask ropePointLayer;
    [SerializeField] float ropeBoxWidth;
    [SerializeField] float swingForce;
    [SerializeField] float maxSwingSpeed;

    [Header("Wall Jump")]
    [SerializeField] float wallJumpTime;
    [SerializeField] float wallJumpForceX;
    [SerializeField] float wallJumpForceY;
    [SerializeField] float wallSlideSpeed;
    [SerializeField] float wallDistance;
    [SerializeField] float jumpTime;

    [Header("Gravity")]
    [SerializeField] float fallMultiplier;
    [SerializeField] float lowJumpMultiplier;

    private Rigidbody2D _playerRigidBody;
    private Animator _playerAnimator;

    public LineRenderer PlayerRopeRenderer { get; private set; }
    public DistanceJoint2D PlayerRopeJoint { get; private set; }

    private float _walkInput = 0f;
    private bool _disableX = false;
    private bool _isJumpPressed = false;
    private bool _isSprintPressed = false;
    private bool _isCrouchPressed = false;
    public bool IsSwingPressed { get; private set; } = false;

    private bool _isCrouching = false;
    private bool _isWallSliding = false;
    private bool _isSprinting = false;

    private bool _toJump = false;
    private bool _toToggleCrouch = false;

    private bool _isSpaceAbove = true;
    private bool _isTouchWall = false;
    public bool IsGrounded { get; private set; } = false;
    public RaycastHit2D FrontRopePointHit { get; private set; }

    private const string PLAYER_IDLE = "PlayerIdle";
    private const string PLAYER_WALK = "PlayerWalk";
    private const string PLAYER_JUMP = "PlayerJump";
    private const string PLAYER_DROP = "PlayerDrop";
    private const string PLAYER_RUN = "PlayerRun";
    private const string PLAYER_CROUCH = "PlayerCrouchMove";
    private const string PLAYER_CROUCH_MOVE = "PlayerCrouchMoving";
    private const string PLAYER_WALL_SLIDE = "PlayerWallSlide";

    private string _currentAnimationState = PLAYER_IDLE;

    private void Awake()
    {
        _playerRigidBody = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();
        PlayerRopeJoint = GetComponent<DistanceJoint2D>();
        PlayerRopeRenderer = GetComponent<LineRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerRopeJoint.enabled = false;
        PlayerRopeRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        BoolAndRayCheck();

        OnJump();
        OnSprint();
        OnWalk();
        OnCrouch();
        OnSwing();

        HandleFlipSprite();

        AnimationStateMachineHandler();

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
    }

    private void BoolAndRayCheck()
    {
        _isSprinting = _isSprintPressed && !_isCrouching;
        CastCheck();
    }

    private void CastCheck()
    {
        IsGrounded = Physics2D.BoxCast(
                        origin: transform.position, size: boxSize, angle: 0,
                        direction: -transform.up, distance: castDistance,
                        layerMask: groundLayer
                   );
        _isSpaceAbove = !Physics2D.BoxCast(
                origin: transform.position, size: boxSizeAbove, angle: 0f,
                direction: transform.up, distance: aboveCastDistance,
                layerMask: groundLayer
            );

        FrontRopePointHit = Physics2D.BoxCast(
                origin: transform.position,
                size: new Vector2(ropeBoxWidth, 2 * PlayerRopeRadius),
                angle: 0f,
                direction: new Vector2(transform.localScale.x, 0f),
                distance: PlayerRopeRadius,
                layerMask: ropePointLayer
            );
    }
    private void OnWalk()
    {
        _walkInput = Input.GetAxis("Horizontal");
    }

    private void OnSprint()
    {
        _isSprintPressed = Input.GetKey(KeyCode.LeftShift);
    }

    private void OnJump()
    {
        _isJumpPressed = Input.GetKeyDown(KeyCode.Space);
        if (_isJumpPressed && !_toJump && (IsGrounded || _isTouchWall))
        {
            _toJump = true;
        }
    }

    private void OnCrouch()
    {
        _isCrouchPressed = Input.GetKeyDown(KeyCode.LeftControl);
        if (_isCrouchPressed && !_toToggleCrouch)
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

        float resultSpeed = _walkInput * (
                _isSprinting ? sprintSpeed
                : _isCrouching ? crouchSpeed
                : walkSpeed
            );
        
        float accel = (Mathf.Abs(resultSpeed) > .01f ? maxAccelerate : maxDeccelerate);
        float speedDif = resultSpeed - _playerRigidBody.velocity.x;

        float movement = Mathf.Pow(
                Mathf.Abs(speedDif) * accel, velocityPower
            ) * Mathf.Sign(speedDif);
        _playerRigidBody.AddForce(movement * Vector2.right);
    }

    private void HandleOnSwing() 
    {
        if (PlayerRopeJoint.enabled && Vector3.Magnitude(_playerRigidBody.velocity) < maxSwingSpeed) 
        {
            
            if (_walkInput > 0.5)
            {
                _playerRigidBody.velocity += new Vector2 (swingForce * Time.deltaTime, 0f);
            }
            else if (_walkInput < -0.5) 
            {
                _playerRigidBody.velocity -= new Vector2 (swingForce * Time.deltaTime, 0f);
            }
        };

    }

    private void HandleGravity() 
    {
        if (IsSwingPressed && PlayerRopeJoint.enabled) 
        {
            _playerRigidBody.AddForce(Vector2.down * 50f);
            return;
        };

        if (_playerRigidBody.velocity.y < 0)
        {
            _playerRigidBody.velocity +=
                Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1)
                * Time.deltaTime;
        }
        else if (_playerRigidBody.velocity.y > 0 && !Input.GetButton("Jump")) 
        {
            _playerRigidBody.velocity +=
                Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1)
                * Time.deltaTime;
        }
    }

    private void HandleJump()
    {
        if (IsGrounded && _toJump && !_isCrouching)
        {
            _playerRigidBody.AddForce(
                    Vector2.up * jumpSpeed, ForceMode2D.Impulse
                );
            _toJump = false;
        }
        else if (_toJump && _isWallSliding)
        {
            _playerRigidBody.AddForce(
                    new Vector2(
                            -transform.localScale.x
                            * wallJumpForceX,
                            wallJumpForceY
                        ),
                    ForceMode2D.Impulse
                );
            _toJump = false;
        }
        
    }

    private void HandleWallSlide()
    {
        bool isFacingRight = transform.localScale.x == 1;

        RaycastHit2D hitGround = Physics2D.Raycast(
                transform.position,
                new Vector2(
                        isFacingRight ?
                        wallDistance : -wallDistance, 0
                    ),
                wallDistance,
                groundLayer
            );
        _isTouchWall = hitGround && (
                hitGround.transform.gameObject.CompareTag("Climbable")
            );

        Debug.DrawRay(transform.position, new Vector2(wallDistance, 0), Color.blue);

        if (_isTouchWall &&
            !IsGrounded &&
            _playerRigidBody.velocity.y < -0.5
            )
        {
            _isWallSliding = true;
        }
        else
        {
            _isWallSliding = false;
        }

        if (_isWallSliding && _playerRigidBody.velocity.y < -wallSlideSpeed)
        {
            _playerRigidBody.velocity = new Vector2(
                    _playerRigidBody.velocity.x,
                    -wallSlideSpeed
                );
        }

    }
    private void HandleCrouching()
    {
        if (IsGrounded && !_isSprinting && _toToggleCrouch)
        {
            _toToggleCrouch = false;
            if (!_isSpaceAbove) return;
            _isCrouching = !_isCrouching;
        }
    }

    private void HandleFlipSprite()
    {
        if (_playerRigidBody.velocity.x > 0.5f)
        {
            transform.localScale = Vector3.one;
        }
        if (_playerRigidBody.velocity.x < -0.5f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void HandleFriction() 
    {
        if (IsGrounded && Mathf.Abs(_walkInput) < .01f) 
        {
            float appliedFriction = Mathf.Min(
                    Mathf.Abs(_playerRigidBody.velocity.x),
                    frictionAmount   
                ) * Mathf.Sign(_playerRigidBody.velocity.x);
            _playerRigidBody.AddForce(Vector2.right * -appliedFriction, ForceMode2D.Impulse);
        }
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
                    if (_isCrouching)
                    {
                        ChangeAnimationState(PLAYER_CROUCH);
                    }
                    if (Mathf.Abs(_walkInput) > 0.5f)
                    {
                        ChangeAnimationState(PLAYER_WALK);
                    }
                    break;
                case PLAYER_CROUCH:
                    if (Mathf.Abs(_walkInput) > 0.5f)
                    {
                        ChangeAnimationState(PLAYER_CROUCH_MOVE);
                    }
                    if (!_isCrouching)
                    {
                        ChangeAnimationState(PLAYER_IDLE);
                    }
                    break;
                case PLAYER_CROUCH_MOVE:
                    if (!_isCrouching)
                    {
                        ChangeAnimationState(PLAYER_IDLE);
                    }
                    if (_walkInput == 0)
                    {
                        ChangeAnimationState(PLAYER_CROUCH);
                    }
                    break;
                case PLAYER_WALK:
                    if (_walkInput == 0)
                    {
                        ChangeAnimationState(PLAYER_IDLE);
                    }
                    if (_isCrouching)
                    {
                        ChangeAnimationState(PLAYER_CROUCH);
                    }
                    if (_isSprintPressed)
                    {
                        ChangeAnimationState(PLAYER_RUN);
                    }
                    break;
                case PLAYER_RUN:
                    if (!_isSprintPressed)
                    {
                        ChangeAnimationState(PLAYER_WALK);
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
                    if (_playerRigidBody.velocity.y > 1f)
                    {
                        ChangeAnimationState(PLAYER_JUMP);
                    }
                    if (_isWallSliding)
                    {
                        ChangeAnimationState(PLAYER_WALL_SLIDE);
                    }
                    break;
                case PLAYER_JUMP:
                    if (_playerRigidBody.velocity.y < -1f)
                    {
                        ChangeAnimationState(PLAYER_DROP);
                    }
                    if (_isWallSliding)
                    {
                        ChangeAnimationState(PLAYER_WALL_SLIDE);
                    }
                    break;
                case PLAYER_WALL_SLIDE:
                    if (!_isWallSliding)
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
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position - transform.up * castDistance, boxSize);
        Gizmos.DrawWireCube(transform.position + transform.up * aboveCastDistance, boxSizeAbove);
        Gizmos.DrawWireCube(
                    center: transform.position + new Vector3(transform.localScale.x, 0f, 0f) * (PlayerRopeRadius),
                    size: new Vector2(ropeBoxWidth, 2 * PlayerRopeRadius)
                );
    }

}
