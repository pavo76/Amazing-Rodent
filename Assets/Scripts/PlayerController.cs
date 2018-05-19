using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    #region Movement Parameters
    [Header("Movement Parameters")]
    public float moveSpeed = 15f;
    public float runSpeedBoost = 5f;
    public float jumpSpeed = 15f;
    public float fallingForce = 25f;
    public float slowFallingForce = 5f;
    private bool facingRight = true;
    public float maxFreezeDuration = 0.1f;
    private float freezeTime;
    #endregion

    #region Collision Check Parameters
    [Header("Collision Check Parameters")]
    public Transform groundCheck;
    public Transform wallCheck;
    public float collisionCheckRadius;
    public LayerMask groundLayer;
    private bool isGrounded = false;
    private bool isCollidingWithWall = false;
    #endregion

    #region Components Variables
    private Rigidbody2D rigidBody;
    #endregion



    // Use this for initialization
    void Start () {
        // Initialize the components of player
        rigidBody = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        CheckCollisions();
        if(isGrounded)
        {
            ResetAirLimiters();
        }
        MoveHorizontally();        
        Jump();
        Freeze();
    }




    // Sets the collision variables depending on players collision with Layermasks
    private void CheckCollisions()
    {
        // Check ground collision
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, collisionCheckRadius, groundLayer);

        // Check wall collision
        isCollidingWithWall = Physics2D.OverlapCircle(wallCheck.position, collisionCheckRadius, groundLayer);
    }


    // Moves player horizontaly
    private void MoveHorizontally()
    {
        float horizontal = Input.GetAxis("Horizontal");
        // Check wether player is holding run button
        float running = Input.GetAxis("Run");

        if(horizontal<0 && facingRight)
        {
            Flip();
        }
        else if(horizontal > 0 && !facingRight)
        {
            Flip();
        }

        if(isGrounded || (!isCollidingWithWall && !isGrounded))
        {
            rigidBody.velocity = new Vector2(horizontal * (moveSpeed + runSpeedBoost*running), rigidBody.velocity.y);
        }
    }

    private void Flip()
    {
        transform.localScale = new Vector2(-1 * transform.localScale.x, transform.localScale.y);
        facingRight = !facingRight;
    }

    // Makes player jump
    private void Jump()
    {
        // Jump if player is grounded and jump button is pressed
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpSpeed);
        }

        // If player is in the air but is holding jump button
        // Fall slower
        else if (!isGrounded && Input.GetButton("Jump"))
        {
            rigidBody.AddForce(Vector2.down * slowFallingForce);
        }

        // If player is in the air but isn't holding jump button
        // Fall faster
        else if (!isGrounded && !Input.GetButton("Jump"))
        {
            rigidBody.AddForce(Vector2.down * fallingForce);
        }        
    }


    private void Freeze()
    {
        // Makes player freeze in space for a period of time
        //TODO Implement Logic for holding character in place depending on freeze time
        if(Input.GetButton("Freeze") && freezeTime>0)
        {
            rigidBody.velocity = Vector2.zero;
            rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
            freezeTime -= Time.deltaTime;
        }
        else
        {
            rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    // Resets parameters that limit usage of movement skills in air
    private void ResetAirLimiters()
    {
        freezeTime = maxFreezeDuration;
    }
}
