using UnityEngine;

public delegate void PoopGunEvent();


public class PoopGun : MonoBehaviour
{
    public static PoopGunEvent PoopGunShoot;
    public GameObject projectilePrefab;
    public Transform spawnPoint;
    public float sizeMultiplier = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GoombaDieManager.goombaDieEvent += increasePoopBulletSize;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void shoot(Vector2 mousePos)
    {
        GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        projectile.transform.localScale *= sizeMultiplier;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0f));
        Vector2 dir = ((Vector2)mouseWorld - (Vector2)spawnPoint.position).normalized;
        Debug.Log($"direction: {dir}");
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        float speed = projectile.GetComponent<PoopBullet>().getSpeed();
        var rb = projectile.GetComponent<Rigidbody2D>();
        rb.linearVelocity = dir * speed;
        PoopGunShoot.Invoke();
    }

    public void increasePoopBulletSize()
    {
        sizeMultiplier *= 1.3f;
    }




}

