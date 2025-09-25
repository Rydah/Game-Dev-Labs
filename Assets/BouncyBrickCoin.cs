using UnityEngine;
using System.Collections;

public class BouncyBrickCoin : MonoBehaviour
{
    private SpringJoint2D spring;
    private Rigidbody2D rb;
    private Vector3 startLocalPos;
    private bool isBouncing = false;
    private bool isDisabled = false;

    public Animator blockAnimator;

    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject goombaPrefab;
    [SerializeField] private bool spawnCoin;

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
        if (isBouncing || isDisabled) return; // block further triggers while bouncing

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

        rb.bodyType = RigidbodyType2D.Dynamic;
        spring.enabled = true;
        rb.AddForce(Vector2.up * 10f, ForceMode2D.Impulse);

        if (coinPrefab != null && spawnCoin)
        {
            GameManager.Instance.AddScore(500);
            Instantiate(coinPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }

        if (coinPrefab != null && spawnCoin)
        {
            if (Random.value < 0.1f)
            {
                Instantiate(goombaPrefab, transform.position + Vector3.down * 0.5f, Quaternion.identity);
            }
        }

        yield return new WaitForSeconds(0.5f);

        spring.enabled = false;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Static;
        transform.localPosition = startLocalPos;

        isBouncing = false;
    }
}
