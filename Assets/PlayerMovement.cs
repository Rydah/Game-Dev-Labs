using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float maxSpeed = 8f;
    public float acceleration = 30f;
    public float maxAccelForce = 100f;
    public float upSpeed = 15f;
    public float stompInvincibilityTime = 0.3f;
    public float jumpHoldForce = 5f;
    public float jumpHoldTime = 0.5f;

    private bool onGroundState = false;
    private bool faceRightState = true;
    private bool isInvincible = false;
    public bool isDead = false;

    private Rigidbody2D marioBody;
    private SpriteRenderer marioSprite;
    public Animator marioAnimator;

    public AudioSource audioSource;
    public AudioClip jumpClip, coinClip, powerUpClip, deathClip, themeClip, stompClip;

    [Header("References")]
    public ActionManager actionManager; 
    private float moveInput;
    private bool jumpHeld;
    private float jumpTimeCounter;

    public GameObject bombPrefab;

    void Start()
    {
        Application.targetFrameRate = 30;
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
        marioAnimator.SetBool("onGround", onGroundState);

        audioSource.PlayOneShot(themeClip);

        // Subscribe to ActionManager events
        actionManager.jump.AddListener(OnJumpPressed);
        actionManager.jumpHold.AddListener(OnJumpHeld);
        actionManager.moveCheck.AddListener(OnMoveInput);
    }

    void OnDestroy()
    {
        // Clean up event subscriptions
        actionManager.jump.RemoveListener(OnJumpPressed);
        actionManager.jumpHold.RemoveListener(OnJumpHeld);
        actionManager.moveCheck.RemoveListener(OnMoveInput);
    }

    public void OnMoveInput(int direction)
    {
        // -1 = left, 0 = stop, 1 = right
        moveInput = direction;

        // Flip sprite
        if (direction < 0 && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;
        }
        else if (direction > 0 && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;
        }
    }

    public void OnJumpPressed()
    {
        if (onGroundState && !isDead)
        {
            marioBody.linearVelocity = new Vector2(marioBody.linearVelocity.x, upSpeed);
            onGroundState = false;
            marioAnimator.SetBool("onGround", onGroundState);

            audioSource.PlayOneShot(jumpClip);
            jumpTimeCounter = jumpHoldTime;
        }
    }

    public void OnJumpHeld()
    {
        // mark as holding (ActionManager will keep firing while held)
        jumpHeld = true;
    }

    public void OnClickBomb()
    {
        if (bombPrefab == null) return;

        // Get world position of mouse cursor
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0f;

        // Spawn bomb
        Instantiate(bombPrefab, mousePos, Quaternion.identity);
    }

    void Update()
    {
        if (isDead) return;
        marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.linearVelocity.x));

        if (onGroundState && marioAnimator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        {
            marioAnimator.Play(Mathf.Abs(marioBody.linearVelocity.x) > 0.1f ? "Run" : "Idle");
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;

        // Horizontal movement
        float targetSpeed = moveInput * maxSpeed;
        float speedDiff = targetSpeed - marioBody.linearVelocity.x;

        if (Mathf.Abs(moveInput) > 0.01f)
        {
            float accelRate = (Mathf.Sign(targetSpeed) != Mathf.Sign(marioBody.linearVelocity.x))
                ? acceleration * 2f
                : acceleration;

            float movement = Mathf.Clamp(speedDiff * accelRate, -maxAccelForce, maxAccelForce);
            marioBody.AddForce(Vector2.right * movement);
        }
        else if (onGroundState)
        {
            marioBody.linearVelocity = new Vector2(0, marioBody.linearVelocity.y);
        }

        // Variable jump height
        if (jumpHeld && !onGroundState && jumpTimeCounter > 0)
        {
            marioBody.AddForce(Vector2.up * jumpHoldForce, ForceMode2D.Force);
            jumpTimeCounter -= Time.fixedDeltaTime;
        }
        else
        {
            jumpHeld = false;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint2D contact in col.contacts)
            {
                if (contact.normal.y > 0.5f)
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
                if (contact.normal.y > 0) { stomped = true; break; }
            }

            if (stomped)
            {
                GameManager.Instance.AddScore(100);
                marioBody.linearVelocity = new Vector2(marioBody.linearVelocity.x, upSpeed);
                audioSource.PlayOneShot(stompClip);
                Destroy(col.gameObject);
                StartCoroutine(StompIFrame());
            }
            else if (!isInvincible)
            {
                isDead = true;
                marioAnimator.SetBool("onDeath", isDead);
                audioSource.Stop();
                audioSource.PlayOneShot(deathClip);
                marioBody.linearVelocity = new Vector2(-15f, 30f);
                GetComponent<Collider2D>().enabled = false;
                StartCoroutine(WaitForDeathSound());
            }
        }
    }

    IEnumerator WaitForDeathSound()
    {
        yield return new WaitForSeconds(deathClip.length);
        GameManager.Instance.ShowGameOverScreen();
    }

    IEnumerator StompIFrame()
    {
        isInvincible = true;
        yield return new WaitForSeconds(stompInvincibilityTime);
        isInvincible = false;
    }
}
