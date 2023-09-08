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

    private Rigidbody2D _playerRigidBody;
    private Animator _playerAnimator;
    private bool isCrouch = false;

    // Start is called before the first frame update
    void Start()
    {
        _playerRigidBody = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>(); 
    }

    // Update is called once per frame
    void Update()
    {
        float walkInput = Input.GetAxis("Horizontal");
        bool isJump = Input.GetKeyDown(KeyCode.Space) && isGrounded();
        bool isSprint = Input.GetKey(KeyCode.LeftShift) && !isCrouch; 
        
        _playerRigidBody.velocity = new Vector2(
                walkInput * (
                isSprint ? 
                sprintSpeed 
                : isCrouch ? 
                crouchSpeed 
                : walkSpeed),
                isJump ? jumpSpeed : _playerRigidBody.velocity.y
            );
        _playerAnimator.SetFloat("walkSpeed", Mathf.Abs(_playerRigidBody.velocity.x));
        _playerAnimator.SetFloat("jumpSpeed", _playerRigidBody.velocity.y);
        HandleFlipSprite();

        if (!isJump && !isSprint && Input.GetKeyDown(KeyCode.C)) 
        {
            if (isCrouch && !isSpaceAbove()) return; 
            isCrouch = !isCrouch;
            _playerAnimator.SetBool("isCrouch", isCrouch);
        }

        Debug.Log(isGrounded());

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
