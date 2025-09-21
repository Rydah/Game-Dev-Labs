using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float maxSpeed = 8f;
    public float acceleration = 30f;
    public float maxAccelForce = 100f;
    public float upSpeed = 15f;

    private bool onGroundState = false;
    private SpriteRenderer marioSprite;
    private bool faceRightState = true;
    private Rigidbody2D marioBody;
    public Animator marioAnimator;

    // Start is called before the first frame update
    void Start()
    {
        // Set to be 30 FPS
        Application.targetFrameRate = 30;
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
        marioAnimator.SetBool("onGround", onGroundState);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("a") && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;
        }

        if (Input.GetKeyDown("d") && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;
        }
        
        marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.linearVelocity.x));

        if (onGroundState && marioAnimator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        {
            if (Mathf.Abs(marioBody.linearVelocity.x) > 0.1f)
                marioAnimator.Play("Run");
            else
                marioAnimator.Play("Idle");
        }
    }

    void FixedUpdate()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        float targetSpeed = moveInput * maxSpeed;   // desired speed
        float speedDiff = targetSpeed - marioBody.linearVelocity.x;

        if (moveInput != 0)
        {
            // If input direction is opposite current velocity, give stronger acceleration
            float accelRate = (Mathf.Sign(targetSpeed) != Mathf.Sign(marioBody.linearVelocity.x))
                ? acceleration * 2f    // faster turn-around
                : acceleration;        // normal accel

            // Apply force
            float movement = Mathf.Clamp(speedDiff * accelRate, -maxAccelForce, maxAccelForce);
            marioBody.AddForce(Vector2.right * movement);
        }
        else
        {
            // Instant stop
            marioBody.linearVelocity = new Vector2(0, marioBody.linearVelocity.y);
        }

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && onGroundState)
        {
            marioBody.linearVelocity = new Vector2(marioBody.linearVelocity.x, upSpeed);
            onGroundState = false;
            marioAnimator.SetBool("onGround", onGroundState);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            // Loop through all contact points in this collision
            foreach (ContactPoint2D contact in col.contacts)
            {
                // A normal pointing up (0,1) means you hit the top surface
                if (contact.normal.y > 0.5f) // adjust threshold if needed
                {
                    onGroundState = true;
                    marioAnimator.SetBool("onGround", onGroundState);
                }
            }
        }
    }
}