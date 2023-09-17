using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
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

    [Header("Wall Jump")]
    [SerializeField] float wallJumpTime;
    [SerializeField] float wallSlideSpeed;
    [SerializeField] float wallDistance;
    [SerializeField] float jumpTime;

    public PlayerStateMachine StateMachine { get; set; }
    public PlayerIdleState IdleState { get; set; }
    public PlayerWalkState WalkState { get; set; }
    public PlayerJumpState JumpState { get; set; }
    public PlayerSprintState SprintState { get; set; }
    public PlayerCrouchState CrouchState { get; set; }
    public PlayerWallSlideState WallSlideState { get; set; }

    private Rigidbody2D _playerRigidBody;
    private Animator _playerAnimator;

    private float _walkInput = 0f;
    private bool _isJumpPressed = false;
    private bool _isSprintPressed = false;
    private bool _isCrouch = false;
    private bool _isWallSliding = false;
    private bool _isSprinting = false;
    private bool _isJump = false;
    private bool _isGrounded = false;
    private bool _isSpaceAbove = true;
    private RaycastHit2D _isTouchWall;

    public float WalkInput { get { return _walkInput; } set { _walkInput = value;  } }
    public bool IsJumpPressed { get { return _isJumpPressed; } set { _isJumpPressed = value; } }
    public bool IsSprintPressed { get { return _isSprintPressed; } set {_isSprintPressed = value; }}
    public bool IsCrouch { get { return _isCrouch; } set { _isCrouch=value; }}
    public bool IsWallSliding { get { return _isWallSliding; } set { _isWallSliding = value; } }
    public bool IsSprinting { get { return _isSprinting; } set { _isSprinting = value; } }
    public bool IsJump { get { return _isJump; } set { _isJump = value; } }
    public bool IsGrounded { get { return _isGrounded; } set { _isGrounded = value; } }
    public bool IsSpaceAbove { get { return _isSpaceAbove; } set { _isSpaceAbove = value; } }
    public RaycastHit2D IsTouchWall { get { return _isTouchWall; } set { _isTouchWall=value; } }

    public Rigidbody2D PlayerRigidBody { get { return _playerRigidBody; } }
    public Animator PlayerAnimator { get { return _playerAnimator; } }

    private void Awake()
    {
        StateMachine = new PlayerStateMachine();

        IdleState = new PlayerIdleState(this, StateMachine);
        WalkState = new PlayerWalkState(this, StateMachine);
        JumpState = new PlayerJumpState(this, StateMachine);
        SprintState = new PlayerSprintState(this, StateMachine);
        CrouchState = new PlayerCrouchState(this, StateMachine);
        WallSlideState = new PlayerWallSlideState(this, StateMachine);
    }

    private void Start()
    {
        StateMachine.Initialize(IdleState);
    }
    private void Update()
    {
        StateMachine.CurrentState.FrameUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }

    private void AnimationTriggerEvent(AnimationTriggerType triggerType) 
    {
        StateMachine.CurrentState.AnimationTriggerEvent(triggerType);
    }

    void OnEnable()
    {
        gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        gameObject.SetActive(false);
    }

    public enum AnimationTriggerType { }
}
