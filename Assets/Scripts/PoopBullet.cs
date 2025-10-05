using UnityEngine;

public class PoopBullet : MonoBehaviour
{

    [SerializeField] private float projectileSpeed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public float getSpeed()
    {
        return projectileSpeed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {

            GoombaDieManager.goombaDieEvent.Invoke();
            Destroy(collision.gameObject);
        }

        Destroy(gameObject);

    }


}
