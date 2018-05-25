using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    #region Movement Parameters
    [Header("Movement Parameters")]
    public float moveSpeed = 15f;
    public float runSpeedBoost = 5f;
    private bool facingRight = true;
    [Header("Jump Parameters")]
    public float jumpSpeed = 15f;
    public float fallingForce = 25f;
    public float slowFallingForce = 5f;
    [Header("Dash Parameters")]
    public float dashSpeed = 150f;
    [Header("Freeze Parameters")]
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
    private bool canDash = false;
    #endregion

    #region Death parameters
    private bool isDead = false;

    public bool IsDead { get { return isDead; } }
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

        if (Input.GetAxis("Horizontal") != 0)
        {
            MoveHorizontally();
        }

        if (Input.GetButtonDown("Dash") && canDash)
        {
            Dash();
        }

        if (Input.GetButton("Jump"))
        {
            Jump();
        }
        else if(!isGrounded && !Input.GetButton("Jump"))
        {
            ApplyFallingForce();
        }

        if (Input.GetButton("Freeze") && freezeTime > 0)
        {
            Freeze();
        }
        else
        {
            ResetConstraints();
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer==LayerMask.NameToLayer("Deadly"))
        {
            Die();
        }

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
    // Speed depends on wether run button is pressed or not
    // TODO change speed if player is in air
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

    // Flips player object
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
      
    }

    // Makes player fall faster if he is not holding space
    private void ApplyFallingForce()
    {
        rigidBody.AddForce(Vector2.down * fallingForce);
    }

    // Makes player dash
    // TODO Put cooldown timer for ground dashes
    // TODO Make dash look sligthly slower
    private void Dash()
    {        
        int orientation = facingRight ? 1: -1;
        rigidBody.velocity = Vector2.zero;
        rigidBody.velocity=new Vector2(orientation * dashSpeed, 0);
        canDash = false;
    }

    // Makes player freeze in space for a period of time
    // TODO put cooldown timer for ground freeze
    private void Freeze()
    {
            rigidBody.velocity = Vector2.zero;
            rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
            freezeTime -= Time.deltaTime;        
    }

    // Resets parameters that limit usage of movement skills in air
    private void ResetAirLimiters()
    {
        freezeTime = maxFreezeDuration;
        canDash = true;
    }

    // Resets Rigidbody constraints to have only rotation frozen
    private void ResetConstraints()
    {
        rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    // Sets is dead property to true so other objects can know when player dies
    private void Die()
    {
        isDead = true;
    }
}
