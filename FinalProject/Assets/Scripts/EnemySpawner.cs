using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 3f;
    public Transform player;
    public Transform village;

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
        Camera cam = Camera.main;
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;
        float leftEdge = cam.transform.position.x - halfWidth;
        float rightEdge = cam.transform.position.x + halfWidth;
        float topEdge = cam.transform.position.y + halfHeight;

        int edge = Random.Range(0, 3);
        Vector2 spawnPos = Vector2.zero;
        switch (edge)
        {
            case 0:
                spawnPos.x = Random.Range(leftEdge, rightEdge);
                spawnPos.y = topEdge;
                break;
            case 1:
                spawnPos.x = leftEdge;
                spawnPos.y = Random.Range(cam.transform.position.y - halfHeight, topEdge);
                break;
            case 2:
                spawnPos.x = rightEdge;
                spawnPos.y = Random.Range(cam.transform.position.y - halfHeight, topEdge);
                break;
        }

        spawnPos.x = SnapToGrid(spawnPos.x);
        spawnPos.y = SnapToGrid(spawnPos.y);

        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        float distance = Vector2.Distance(player.position, village.position);
        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
            enemyHealth.maxHealth = Mathf.CeilToInt(1 + distance / 10f);
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        if (enemyController != null)
            enemyController.moveTickDuration = Mathf.Max(0.3f, 0.5f - distance / 100f);
    }

    float SnapToGrid(float value)
    {
        return Mathf.Round(value - 0.5f) + 0.5f;
    }
}
