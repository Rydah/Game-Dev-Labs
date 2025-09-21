using UnityEngine;
using System.Collections;

public class BlockBounce : MonoBehaviour
{
    private SpringJoint2D spring;
    private Rigidbody2D rb;
    private Vector3 startLocalPos;
    private bool isBouncing = false;
    public Animator blockAnimator;

    void Start()
    {
        spring = GetComponent<SpringJoint2D>();
        rb = GetComponent<Rigidbody2D>();

        spring.enabled = false;
        rb.bodyType = RigidbodyType2D.Static;

        // Save the starting position so we can reset exactly
        startLocalPos = transform.localPosition;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (isBouncing) return; // block further triggers while bouncing

        if (col.gameObject.CompareTag("Player"))
        {
            foreach (ContactPoint2D contact in col.contacts)
            {
                // only trigger if Mario hit from below
                if (contact.normal.y > 0.5f)
                {
                    Debug.Log("hit below!");
                    StartCoroutine(DoBounce());
                    break;
                }
            }
        }
    }

    IEnumerator DoBounce()
    {
        isBouncing = true;
        blockAnimator.SetBool("isBounce", isBouncing);

        rb.bodyType = RigidbodyType2D.Dynamic;
        spring.enabled = true;
        rb.AddForce(Vector2.up * 10f, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.5f);

        spring.enabled = false;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Static;
        transform.localPosition = startLocalPos;

        isBouncing = false;
        blockAnimator.SetBool("isBounce", isBouncing);
    }
}
