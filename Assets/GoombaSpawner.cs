using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject goombaPrefab; // assign prefab in Inspector
    public float spawnInterval = 3f; // seconds between spawns
    public int maxEnemies = 5;
    public Transform[] spawnPoints;  // optional spawn locations

    private float timer;
    private List<GameObject> activeEnemies = new List<GameObject>();

    void Update()
    {
        timer += Time.deltaTime;
        activeEnemies.RemoveAll(item => item == null);

        if (timer >= spawnInterval && activeEnemies.Count < maxEnemies)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        if (goombaPrefab == null) return;

        Vector3 spawnPos;

        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            // pick a random spawn point
            int index = Random.Range(0, spawnPoints.Length);
            spawnPos = spawnPoints[index].position;
        }
        else
        {
            // spawn at spawner's position
            spawnPos = transform.position;
        }

        GameObject enemy = Instantiate(goombaPrefab, spawnPos, Quaternion.identity);
        activeEnemies.Add(enemy);
    }
}
