using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    private float nextSpawnTime;
    public float spawnInterval = 6f;

    void Start()
    {
        SetNextSpawnTime();
        Debug.Log("Start: Initial next spawn time set for: " + nextSpawnTime);
    }

    void Update()
    {
        Debug.Log("Update: Current Time: " + Time.time + " | Next Spawn Time: " + nextSpawnTime);

        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            SetNextSpawnTime();
            Debug.Log("Update after spawn: Spawned enemy at: " + Time.time + ". Next spawn time set for: " + nextSpawnTime);
        }
    }

    void SetNextSpawnTime()
    {
        nextSpawnTime = Time.time + spawnInterval;
        Debug.Log("SetNextSpawnTime: Updated next spawn time to: " + nextSpawnTime);
    }

    void SpawnEnemy()
    {
        Vector3 spawnPosition = Random.onUnitSphere * 10f;
        spawnPosition.y = 10f;

        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        enemy.GetComponent<Rigidbody>().useGravity = true;

        Debug.Log("SpawnEnemy: Enemy spawned at position: " + spawnPosition);
    }
}
