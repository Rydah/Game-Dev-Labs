using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float maxSpeed = 8f;
    public float acceleration = 30f;
    public float maxAccelForce = 100f;
    public float upSpeed = 15f;
    public float stompInvincibilityTime = 0.3f;

    private bool onGroundState = false;
    private bool faceRightState = true;
    private bool isInvincible = false;
    public bool isDead = false;

    private Rigidbody2D marioBody;
    public Animator marioAnimator;
    private SpriteRenderer marioSprite;

    public AudioSource audioSource;
    public AudioClip jumpClip;
    public AudioClip coinClip;
    public AudioClip powerUpClip;
    public AudioClip deathClip;
    public AudioClip themeClip;
    public AudioClip stompClip;

    // Start is called before the first frame update
    void Start()
    {
        // Set to be 30 FPS
        Application.targetFrameRate = 30;
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
        marioAnimator.SetBool("onGround", onGroundState);

        audioSource.PlayOneShot(themeClip);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;

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
        if (isDead) return;

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

            audioSource.PlayOneShot(jumpClip);
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

        if (col.gameObject.CompareTag("Enemy"))
        {
            bool stomped = false;

            foreach (ContactPoint2D contact in col.contacts)
            {
                if (contact.normal.y > 0) // top contact
                {
                    stomped = true;
                    break;
                }
            }

            if (stomped)
            {
                // Score
                GameManager.Instance.AddScore(100);

                // Bounce
                marioBody.linearVelocity = new Vector2(marioBody.linearVelocity.x, upSpeed);

                // Play stomp sound
                audioSource.PlayOneShot(stompClip);

                // Destroy enemy
                Destroy(col.gameObject);

                // Start brief invincibility
                StartCoroutine(StompIFrame());
            }
            else if (!isInvincible)
            {
                // Player hit from side ¡æ game over
                isDead = true;
                marioAnimator.SetBool("onDeath", isDead);

                audioSource.Stop();
                //StopAllEnemies();

                audioSource.PlayOneShot(deathClip);
                marioBody.linearVelocity = new Vector2(-15f, 30f);
                Collider2D col2D = GetComponent<Collider2D>();
                col2D.enabled = false;

                StartCoroutine(WaitForDeathSound());
            }
        }
    }

    IEnumerator WaitForDeathSound()
    {
        // Wait for the length of the clip
        yield return new WaitForSeconds(deathClip.length);

        // Show game over screen
        GameManager.Instance.ShowGameOverScreen();
    }

    IEnumerator StompIFrame()
    {
        isInvincible = true;
        yield return new WaitForSeconds(stompInvincibilityTime);
        isInvincible = false;
    }

    private void StopAllEnemies()
    {
        // Find all GameObjects with tag "Enemy"
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            // Disable their movement script
            EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
            if (movement != null)
            {
                movement.enabled = false;
            }

            // Optional: also freeze their Rigidbody2D
            Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic; // prevent physics movement
            }
        }
    }
}