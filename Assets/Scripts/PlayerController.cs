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

    private bool _cannotWalk = false;

    private float _walkInput = 0f;
    private bool _isJumpPressed = false;
    private bool _isSprintPressed = false;
    private bool _isCrouchPressed = false;

    private bool _isCrouch = false;
    private bool _isWallSliding = false;
    private bool _isSprinting = false;
    private bool _isGroundJump = false;
    private bool _isGrounded = false;
    private bool _isSpaceAbove = true;

    private RaycastHit2D _isTouchWall;

    // Start is called before the first frame update
    void Start()
    {
        _playerRigidBody = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        _isGroundJump = _isJumpPressed && _isGrounded;
        _isSprinting = _isSprintPressed && !_isCrouch;

        _isGrounded = Physics2D.BoxCast(
                transform.position, boxSize, 0,
                -transform.up, castDistance, groundLayer
           );
        _isSpaceAbove = !Physics2D.BoxCast(
                transform.position, boxSizeAbove, 0,
                transform.up, aboveCastDistance, groundLayer
            );

        OnJump();

        OnSprint();

        OnWalk();

        OnCrouch();
         
        HandleHorizontalMoves();

        HandleJump();

        HandleFlipSprite();

        HandleCrouching();

        HandleWallSlide();

        HandleGravity();

        _playerAnimator.SetFloat("walkSpeed", Mathf.Abs(_playerRigidBody.velocity.x));
        _playerAnimator.SetFloat("jumpSpeed", _playerRigidBody.velocity.y);
        _playerAnimator.SetBool("isWallSlide", _isWallSliding);
        _playerAnimator.SetBool("isCrouch", _isCrouch);
    }

    private void HandleHorizontalMoves()
    {
        if (_cannotWalk) return;
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
        if (_isGroundJump)
        {
            _playerRigidBody.velocity = new Vector2(
                        _playerRigidBody.velocity.x,
                        jumpSpeed
                    );
        } else if (_isJumpPressed && _isWallSliding)
        {
            _ = StartCoroutine("CancelWalkCoroutine");
            _playerRigidBody.AddForce(
                    new Vector2 (
                            - transform.localScale.x * wallJumpForceX,
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
        _isCrouchPressed = Input.GetKeyDown(KeyCode.C);
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
                groundLayer
            );
        Debug.DrawRay(transform.position, new Vector2(wallDistance, 0), Color.blue);

        if (_isTouchWall &&
            !_isGrounded &&
            _playerRigidBody.velocity.y < -0.5 &&
            Mathf.Abs(_walkInput) > 0.5f)
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
        if (_walkInput > Mathf.Epsilon)
        {
            transform.localScale = Vector3.one;
        }
        if (_walkInput < -Mathf.Epsilon)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    IEnumerator CancelWalkCoroutine()
    {
        _cannotWalk = true;
        _playerRigidBody.velocity = new Vector2(0f, 0f);
        yield return new WaitForSeconds(.2f);
        _cannotWalk = false;
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireCube(transform.position - transform.up * castDistance, boxSize);
        Gizmos.DrawWireCube(transform.position + transform.up * aboveCastDistance, boxSizeAbove);
    }
}
