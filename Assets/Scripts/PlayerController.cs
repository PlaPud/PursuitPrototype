using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float jumpSpeed;
    [SerializeField] float crouchSpeed;

    [SerializeField] LayerMask groundLayer;

    [Header("RayCast Ground")]
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
    [SerializeField] LayerMask slidableLayer;

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
    private RaycastHit2D _isTouchWall;
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
        HandleOnSwing();

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

    private void HandleIdle() 
    {
        bool isNotInAir = !(Mathf.Abs(_playerRigidBody.velocity.y) > 0.5);
       
        if (_walkInput == 0 && isNotInAir) 
        {
            ChangeAnimationState(PLAYER_IDLE);
        } 
    }

    private void HandleHorizontalMoves()
    {
        if (_disableX || _playerRopeJoint.enabled ) return;
        _playerRigidBody.velocity = new Vector2(
                        _walkInput * (
                            _isSprinting ?
                            sprintSpeed
                            : _isCrouch ?
                            crouchSpeed
                            : walkSpeed
                        ),
                        _playerRigidBody.velocity.y
                    );
        if (_isGrounded && Mathf.Abs(_walkInput) > 0.5) 
        {
            ChangeAnimationState(
                    _isSprinting ? 
                    PLAYER_RUN 
                    : _isCrouch ?
                    PLAYER_CROUCH_MOVE
                    : PLAYER_WALK
                );
        }
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
            _playerRigidBody.velocity = new Vector2(
                        _playerRigidBody.velocity.x,
                        jumpSpeed
                    );
        } 
        else if (_isJumpPressed && _isWallSliding)
        {
            _ = StartCoroutine("WallJumpCoroutine");
            _playerRigidBody.AddForce(
                    new Vector2 (
                            -transform.localScale.x
                            * wallJumpForceX,
                            wallJumpForceY
                        ),
                    ForceMode2D.Impulse
                );
            _playerRigidBody.velocity += new Vector2 (
                    _walkInput * walkSpeed ,
                    _walkInput * walkSpeed 
                );
        }

        if (!_isGrounded && _playerRigidBody.velocity.y > 1f)
        {
            ChangeAnimationState(PLAYER_JUMP);
        }
        else if (!_isGrounded && _playerRigidBody.velocity.y < -1f && !_isWallSliding) 
        {
            ChangeAnimationState(PLAYER_DROP);
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
        _isCrouchPressed = Input.GetKeyDown(KeyCode.C);
    }

    private void OnSwing() 
    {
        _isSwingPressed = Input.GetKey(KeyCode.Mouse0);
    }

    private void HandleWallSlide()
    {
        bool isFacingRight = transform.localScale.x == 1;

        _isTouchWall = Physics2D.Raycast(
                transform.position,
                new Vector2(
                        isFacingRight ?
                        wallDistance : -wallDistance, 0
                    ),
                wallDistance,
                slidableLayer
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
            ChangeAnimationState(PLAYER_WALL_SLIDE);
        }

    }

    private void HandleCrouching()
    {
        if (!_isGroundJump && !_isSprinting && _isCrouchPressed)
        {
            if (_isCrouch && !_isSpaceAbove) return;
            _isCrouch = !_isCrouch;
            ChangeAnimationState(_isCrouch ? PLAYER_CROUCH : PLAYER_IDLE);
        }
    }

    private void HandleFlipSprite()
    {
        if (_playerRigidBody.velocity.x > Mathf.Epsilon)
        {
            transform.localScale = Vector3.one;
        }
        if (_playerRigidBody.velocity.x < -Mathf.Epsilon)
        {
            transform.localScale = new Vector3(-1, 1, 1);
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

}
