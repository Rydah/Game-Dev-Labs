using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour
{
    [SerializeField] private float riseHeight = 1f;   // how high the coin goes
    [SerializeField] private float moveDuration = 0.5f; // total up+down time
    [SerializeField] private AudioClip coinSound;

    private SpriteRenderer sr;
    private AudioSource audioSource;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        if (coinSound != null)
        {
            audioSource.PlayOneShot(coinSound);
        }

        StartCoroutine(RiseAndReturn());
    }

    private IEnumerator RiseAndReturn()
    {
        Vector3 startPos = transform.position;
        Vector3 peakPos = startPos + Vector3.up * riseHeight;

        GameManager.Instance.AddScore(500);

        float halfDuration = moveDuration / 2f;
        float elapsed = 0f;

        // Move UP
        while (elapsed < halfDuration)
        {
            float t = elapsed / halfDuration;
            transform.position = Vector3.Lerp(startPos, peakPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // snap to peak
        transform.position = peakPos;

        // Reset elapsed for downward motion
        elapsed = 0f;

        // Move DOWN
        while (elapsed < halfDuration)
        {
            float t = elapsed / halfDuration;
            transform.position = Vector3.Lerp(peakPos, startPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // snap to start (inside block)
        transform.position = startPos;

        Destroy(gameObject); // remove coin
    }
}
