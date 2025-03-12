using UnityEngine;
using System.Collections;
public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 3f;
    public Transform player;
    public Transform village;
    public float minX = -10f, maxX = 10f;
    public float minY = -10f, maxY = 10f;
    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }
    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnEnemy();
        }
    }
    void SpawnEnemy()
    {
        Vector2 spawnOffset = Random.insideUnitCircle.normalized * 5;
        Vector2 spawnPos = (Vector2)player.position + spawnOffset;
        spawnPos.x = Mathf.Clamp(spawnPos.x, minX, maxX);
        spawnPos.y = Mathf.Clamp(spawnPos.y, minY, maxY);
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        float distance = Vector2.Distance(player.position, village.position);
        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.maxHealth = Mathf.CeilToInt(1 + distance / 10f);
        }
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.moveTickDuration = Mathf.Max(0.3f, 0.5f - distance / 100f);
        }
    }
}
