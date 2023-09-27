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

    [Header("RopePoints")]
    [SerializeField] float playerRopeRadius;
    [SerializeField] LayerMask ropePointLayer;
    [SerializeField] float ropeBoxWidth;
    [SerializeField] float swingForce;

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
    private LineRenderer _playerRopeRenderer;
    private DistanceJoint2D _playerRopeJoint;

    private bool _disableX = false;

    private float _walkInput = 0f;
    private bool _isJumpPressed = false;
    private bool _isSprintPressed = false;
    private bool _isCrouchPressed = false;
    private bool _isSwingPressed = false;

    private bool _isCrouch = false;
    private bool _isWallSliding = false;
    private bool _isSprinting = false;
    private bool _isGroundJump = false;

    private bool _isGrounded = false;
    private bool _isSpaceAbove = true;
    private bool _isTouchWall = false;
    private RaycastHit2D _frontRopePointHit;

    public RaycastHit2D FrontRopePointHit { get { return _frontRopePointHit; } }
    public bool IsSwingPressed { get { return _isSwingPressed; } set { _isSwingPressed = value; } }
    public bool IsGrounded { get { return _isGrounded; } }
    public LineRenderer PlayerRopeRenderer { get { return _playerRopeRenderer; } set { _playerRopeRenderer = value; } }
    public DistanceJoint2D PlayerRopeJoint { get { return _playerRopeJoint; } set { _playerRopeJoint = value; } }
    public float PlayerRopeRadius { get { return playerRopeRadius; } }

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
        _playerRopeJoint = GetComponent<DistanceJoint2D>();
        _playerRopeRenderer = GetComponent<LineRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _playerRopeJoint.enabled = false;
        _playerRopeRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        BoolStatusCheck();

        OnJump();
        OnSprint();
        OnWalk();
        OnCrouch();
        OnSwing();

        HandleIdle();
        HandleHorizontalMoves();
        HandleJump();
        HandleFlipSprite();
        HandleCrouching();
        HandleWallSlide();
        HandleGravity();
        HandleFriction();
        HandleOnSwing();

        AnimationStateMachineHandler();

        HandleDisable();
    }

    private void FixedUpdate()
    {
        
    }

    private void BoolStatusCheck()
    {
        _isGroundJump = _isJumpPressed && _isGrounded;
        _isSprinting = _isSprintPressed && !_isCrouch;
        CastCheck();
    }

    private void CastCheck()
    {
        _isGrounded = Physics2D.BoxCast(
                        origin: transform.position, size: boxSize, angle: 0,
                        direction: -transform.up, distance: castDistance,
                        layerMask: groundLayer
                   );
        _isSpaceAbove = !Physics2D.BoxCast(
                origin: transform.position, size: boxSizeAbove, angle: 0f,
                direction: transform.up, distance: aboveCastDistance,
                layerMask: groundLayer
            );

        _frontRopePointHit = Physics2D.BoxCast(
                origin: transform.position,
                size: new Vector2(ropeBoxWidth, 2 * playerRopeRadius),
                angle: 0f,
                direction: new Vector2(transform.localScale.x, 0f),
                distance: playerRopeRadius,
                layerMask: ropePointLayer
            );
    }

    private void HandleIdle() { }

    private void HandleHorizontalMoves()
    {
        if (_disableX || _playerRopeJoint.enabled) return;

        float resultSpeed = _walkInput * (
                _isSprinting ? sprintSpeed
                : _isCrouch ? crouchSpeed
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
        if (_playerRopeJoint.enabled) 
        {
            if (_walkInput > 0.5)
            {
                _playerRigidBody.velocity += new Vector2 (swingForce * Time.deltaTime, 0f);
            }
            else if (_walkInput < -0.5) 
            {
                _playerRigidBody.velocity -= new Vector2(swingForce * Time.deltaTime, 0f);
            }
        };

    }

    private void HandleGravity() 
    {
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
        if (_isGroundJump && !_isCrouch)
        {
            //_playerRigidBody.velocity = new Vector2(
            //            _playerRigidBody.velocity.x,
            //            jumpSpeed
            //        );
            _playerRigidBody.AddForce(
                    Vector2.up * jumpSpeed, ForceMode2D.Impulse
                );
        }
        else if (_isJumpPressed && _isWallSliding)
        {
            _ = StartCoroutine(WallJumpCoroutine());
            _playerRigidBody.AddForce(
                    new Vector2(
                            -transform.localScale.x
                            * wallJumpForceX,
                            wallJumpForceY
                        ),
                    ForceMode2D.Impulse
                );
        }
        
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
    }

    private void OnCrouch()
    {
        _isCrouchPressed = Input.GetKeyDown(KeyCode.LeftControl);
    }

    private void OnSwing() 
    {
        _isSwingPressed = Input.GetKey(KeyCode.Mouse0);
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
            !_isGrounded &&
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
        if (!_isGroundJump && !_isSprinting && _isCrouchPressed)
        {
            if (_isCrouch && !_isSpaceAbove) return;
            _isCrouch = !_isCrouch;
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
        if (_isGrounded && Mathf.Abs(_walkInput) < .01f) 
        {
            float appliedFriction = Mathf.Min(
                    Mathf.Abs(_playerRigidBody.velocity.x),
                    frictionAmount   
                ) * Mathf.Sign(_playerRigidBody.velocity.x);
            _playerRigidBody.AddForce(Vector2.right * -appliedFriction, ForceMode2D.Impulse);
        }
    }

    IEnumerator WallJumpCoroutine()
    {
        _disableX = true;
        _playerRigidBody.velocity = new Vector2(0f, 0f);
        yield return new WaitForSeconds(.3f);
        _disableX = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position - transform.up * castDistance, boxSize);
        Gizmos.DrawWireCube(transform.position + transform.up * aboveCastDistance, boxSizeAbove);
        Gizmos.DrawWireCube(
                    center: transform.position + new Vector3 (transform.localScale.x, 0f, 0f) * (playerRopeRadius),
                    size: new Vector2 (ropeBoxWidth, 2 * playerRopeRadius)
                );
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
        if (_isGrounded)
        {
            switch (_currentAnimationState)
            {
                case PLAYER_IDLE:
                    if (_isCrouch)
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
                    if (!_isCrouch)
                    {
                        ChangeAnimationState(PLAYER_IDLE);
                    }
                    break;
                case PLAYER_CROUCH_MOVE:
                    if (!_isCrouch)
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
                    if (_isCrouch)
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
}
