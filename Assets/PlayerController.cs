using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float HorizontalMoveForce = 10f;
    public float maxSpeed = 10f;
    public float JumpForce = 35f;


    private Transform groundCheck;
    private bool isGrounded = false;
    private Rigidbody2D rigBody;

	// Use this for initialization
	void Start () {
        groundCheck = transform.Find("groundCheck");
        rigBody = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        isGrounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        if (Mathf.Abs(rigBody.velocity.x)< maxSpeed)
        {
            rigBody.AddForce(Vector2.right * horizontal * HorizontalMoveForce);
        }
        else if(Mathf.Abs(rigBody.velocity.x)>=maxSpeed)
        {
            rigBody.velocity = new Vector2(Mathf.Sign(rigBody.velocity.x) * maxSpeed, rigBody.velocity.y);
        }


        if (isGrounded && Input.GetAxis("Vertical")>0)
        {
            rigBody.AddForce(Vector2.up * JumpForce);
        }
    }
}
