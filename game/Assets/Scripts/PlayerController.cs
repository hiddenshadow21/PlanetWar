using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private float moveSpeed = 50f;
    private Vector2 moveDir;
    public Rigidbody2D rb;

    void HandleMovement()
    {
        if (!isLocalPlayer)
            return;
        float moveHorizontal = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveVertical = 0; //Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        moveDir = new Vector2(moveHorizontal, moveVertical);

    }

    void Update()
    {
        HandleMovement();
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + (Vector2)transform.TransformDirection(moveDir) * moveSpeed * Time.deltaTime);
    }
}
