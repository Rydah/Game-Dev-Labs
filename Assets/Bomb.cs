using UnityEngine;

public class Bomb : MonoBehaviour
{
    public Sprite explosionSprite;
    public AudioClip explosionSound;
    public float explosionDuration = 0.5f;

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // Only explode if collided with Player, Enemy, or Ground
        if (col.CompareTag("Player") || col.CompareTag("Enemy") || col.CompareTag("Ground"))
        {
            Explode();
        }
    }

    void Explode()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;      // stop all movement
            rb.gravityScale = 0f;            // stop gravity
        }

        // Play explosion sprite
        if (explosionSprite != null)
        {
            spriteRenderer.sprite = explosionSprite;
        }

        // Play explosion sound
        if (explosionSound != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }

        // Disable collider so it doesn't trigger again
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Destroy bomb after explosion duration
        Destroy(gameObject, explosionDuration);
    }
}
