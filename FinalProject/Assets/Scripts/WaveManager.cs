using System.Collections;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;         // The original enemy
    public GameObject secondEnemyPrefab;   // The second enemy
    public GameObject thirdEnemyPrefab;    // The new third enemy (e.g. Dragon)

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Wave Settings")]
    // Delay between individual enemy spawns within a wave.
    public float spawnDelay = 0.5f;
    // Delay after a wave is cleared before the next wave begins.
    public float timeBetweenWaves = 2f;

    [Header("Wave Enemy Counts")]
    public int[] waveEnemyCounts = new int[] { 1, 2, 4, 6, 6, 8 };

    [Header("UI Elements")]
    public TMP_Text waveText;
    // Duration to show the wave message.
    public float waveDisplayDuration = 2f;

    // Current wave number.
    public int waveNumber = 1;

    void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        while (true)
        {
            // 1) Display wave text
            if (waveText != null)
            {
                waveText.text = "Wave " + waveNumber;
                waveText.gameObject.SetActive(true);
            }

            // Wait so the player sees the wave number
            yield return new WaitForSeconds(waveDisplayDuration);

            if (waveText != null)
                waveText.gameObject.SetActive(false);

            // 2) Determine enemy count for this wave
            int enemyCount;
            if (waveNumber <= waveEnemyCounts.Length)
            {
                enemyCount = waveEnemyCounts[waveNumber - 1];
            }
            else
            {
                // For waves beyond predefined ones, scale up further
                enemyCount = waveEnemyCounts[waveEnemyCounts.Length - 1]
                             + (waveNumber - waveEnemyCounts.Length) * 2;
            }

            Debug.Log("Wave " + waveNumber + " starting with " + enemyCount + " enemies.");

            // 3) Decide which enemy will drop a potion
            int dropIndex = Random.Range(0, enemyCount);

            // 4) Spawn enemies
            for (int i = 0; i < enemyCount; i++)
            {
                // Choose random spawn point
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

                // Decide which prefab to spawn
                GameObject prefabToSpawn = null;
                if (waveNumber == 1)
                {
                    // Wave 1 => only the original enemy
                    prefabToSpawn = enemyPrefab;
                }
                else if (waveNumber == 2)
                {
                    // Wave 2 => 50/50 between enemyPrefab & secondEnemyPrefab
                    if (Random.value < 0.5f)
                        prefabToSpawn = enemyPrefab;
                    else
                        prefabToSpawn = secondEnemyPrefab;
                }
                else
                {
                    // Wave 3+ => random among all three enemies
                    float r = Random.value;
                    if (r < 0.33f)
                        prefabToSpawn = enemyPrefab;
                    else if (r < 0.66f)
                        prefabToSpawn = secondEnemyPrefab;
                    else
                        prefabToSpawn = thirdEnemyPrefab;
                }

                // Instantiate the enemy
                GameObject enemyInstance = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);

                // Possibly mark this enemy to drop a potion
                if (i == dropIndex)
                {
                    EnemyPathfinder ep = enemyInstance.GetComponent<EnemyPathfinder>();
                    if (ep != null) 
                        ep.willDropPotion = true;

                    // If the third enemy uses "DragonPathfinder" instead, you can do:
                    // DragonPathfinder dp = enemyInstance.GetComponent<DragonPathfinder>();
                    // if (dp != null) dp.willDropPotion = true;
                }

                yield return new WaitForSeconds(spawnDelay);
            }

            // 5) Wait until all enemies with Tag "Enemy" are gone
            yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Enemy").Length == 0);
            Debug.Log("Wave " + waveNumber + " cleared.");

            // Award points
            int pointsAwarded = (waveNumber % 3 == 0) ? 2 : 1;
            UpgradeManager.Instance.AwardPoints(pointsAwarded);
            Debug.Log("Awarded " + pointsAwarded + " upgrade point(s).");

            // Show upgrade panel
            UpgradeManager.Instance.ShowUpgradePanel();
            // Wait for the player to close the upgrade panel
            yield return new WaitUntil(() => !UpgradeManager.Instance.upgradePanel.activeSelf);

            // Wait a short time before the next wave
            yield return new WaitForSeconds(timeBetweenWaves);

            // Increment wave
            waveNumber++;
        }
    }
}
