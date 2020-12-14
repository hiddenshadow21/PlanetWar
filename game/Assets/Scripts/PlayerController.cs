using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private float moveSpeed = 10f;
    private float jumpHeight = 3f;
    private Vector2 moveDir;

    public new Collider2D collider;
    public Rigidbody2D rb;

    public  bool isGrounded { 
        get;
        private set;
    }

    void Update()
    {
        HandleMovement();
    }

    private void FixedUpdate()
    {
        CheckIfOnGround();
    }

    void HandleMovement()
    {
        if (!isLocalPlayer)
            return;
        float moveHorizontal = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        moveDir = new Vector2(moveHorizontal, 0);
        transform.Translate(moveDir);

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(transform.up * 1000 * jumpHeight * Time.deltaTime);
        }
    }

    void CheckIfOnGround()
    {
        if (!isLocalPlayer)
            return;
        isGrounded = false;
        var t = transform.TransformDirection(
            transform.InverseTransformDirection(transform.position)
            - new Vector3(0, collider.bounds.extents.y, 0)
        );
        RaycastHit2D hit = Physics2D.Raycast(
            t,
            -transform.up);
        if (hit.collider != null)
        {

            if (hit.distance <= 0.2f)
                isGrounded = true;
        }
            
    }
    
}
