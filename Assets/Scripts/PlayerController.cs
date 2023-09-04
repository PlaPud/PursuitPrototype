using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D playerRigidBody;
    private Animator playerAnimator;
    // Start is called before the first frame update
    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>(); 
    }

    // Update is called once per frame
    void Update()
    {
        float walkInput = Input.GetAxis("Horizontal");
        playerRigidBody.velocity = new Vector2(
                walkInput * 5.0f,
                playerRigidBody.velocity.y
            );
        playerAnimator.SetFloat("walkSpeed", Mathf.Abs(walkInput));

        if (Input.GetAxis("Horizontal") > Mathf.Epsilon)
        {
            transform.localScale = Vector3.one;
        }
        if (Input.GetAxis("Horizontal") < -Mathf.Epsilon)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        
    }
}
