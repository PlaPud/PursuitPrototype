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
    [SerializeField] float wallSlideSpeed;
    [SerializeField] float wallDistance;
    [SerializeField] float jumpTime;

    private Rigidbody2D _playerRigidBody;
    private Animator _playerAnimator;
    private float walkInput = 0f;
    private bool _isCrouch = false;
    private bool _isWallSliding = false;
    private bool _isSprinting = false;
    private bool _isJump = false;

    private RaycastHit2D _wallCheckhit;

    // Start is called before the first frame update
    void Start()
    {
        _playerRigidBody = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>(); 
    }

    // Update is called once per frame
    void Update()
    {
        walkInput = Input.GetAxis("Horizontal");
        _isJump = Input.GetKeyDown(KeyCode.Space) && isGrounded();
        _isSprinting = Input.GetKey(KeyCode.LeftShift) && !_isCrouch; 
        
        _playerRigidBody.velocity = new Vector2(
                walkInput * (
                _isSprinting ? 
                sprintSpeed 
                : _isCrouch ? 
                crouchSpeed 
                : walkSpeed),
                _isJump ? jumpSpeed : _playerRigidBody.velocity.y
            );
        _playerAnimator.SetFloat("walkSpeed", Mathf.Abs(_playerRigidBody.velocity.x));
        _playerAnimator.SetFloat("jumpSpeed", _playerRigidBody.velocity.y);
        
        HandleFlipSprite();

        HandleCrouching();

        HandleWallSlide();

    }

    private void HandleWallSlide() 
    {
        bool isFacingRight = transform.localScale.x == 1;

        _wallCheckhit = Physics2D.Raycast(
                transform.position,
                new Vector2(
                        isFacingRight ? 
                        wallDistance : -wallDistance , 0
                    ),
                wallDistance,
                groundLayer
            );
        Debug.DrawRay(transform.position, new Vector2(wallDistance, 0), Color.blue);

        if (_wallCheckhit && !isGrounded() && Mathf.Abs(walkInput) > 0.5f)
        {
            _isWallSliding = true;
            jumpTime = Time.time + wallJumpTime;
        }
        else if (jumpTime < Time.time) 
        {
            _isWallSliding = false;
        }

        if (_isWallSliding) 
        {
            _playerRigidBody.velocity = new Vector2(
                    _playerRigidBody.velocity.x,
                    Mathf.Clamp(
                            _playerRigidBody.velocity.y,
                            wallSlideSpeed,
                            float.MaxValue 
                        )
                );
        }
    }

    private void HandleCrouching() 
    {
        if (!_isJump && !_isSprinting && Input.GetKeyDown(KeyCode.C))
        {
            if (_isCrouch && !isSpaceAbove()) return;
            _isCrouch = !_isCrouch;
            _playerAnimator.SetBool("isCrouch", _isCrouch);
        }
    }

    private void HandleFlipSprite()
    {
        if (Input.GetAxis("Horizontal") > Mathf.Epsilon)
        {
            transform.localScale = Vector3.one;
        }
        if (Input.GetAxis("Horizontal") < -Mathf.Epsilon)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private bool isGrounded() 
    {
        if (Physics2D.BoxCast(
                transform.position, boxSize, 0,
                -transform.up, castDistance, groundLayer
           ))
        {
            return true;
        }
        else 
        {
            return false;
        }
    }

    private bool isSpaceAbove() 
    {
        if (Physics2D.BoxCast(
                transform.position, boxSizeAbove, 0,
                transform.up, aboveCastDistance, groundLayer
            ))
        {
            return false;
        }
        else 
        {
            return true;
        }
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireCube(transform.position - transform.up * castDistance, boxSize);
        Gizmos.DrawWireCube(transform.position + transform.up * aboveCastDistance, boxSizeAbove);
    }
}
