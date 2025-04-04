using System.Collections;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;
    
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
    private int waveNumber = 1;
    
    void Start()
    {
        StartCoroutine(SpawnWaves());
    }
    
    IEnumerator SpawnWaves()
    {
        while (true)
        {
            // Display wave text.
            if (waveText != null)
            {
                waveText.text = "Wave " + waveNumber;
                waveText.gameObject.SetActive(true);
            }
            
            // Wait so the player sees the wave number.
            yield return new WaitForSeconds(waveDisplayDuration);
            
            if (waveText != null)
                waveText.gameObject.SetActive(false);
            
            // Determine enemy count for this wave.
            int enemyCount = 0;
            if (waveNumber <= waveEnemyCounts.Length)
            {
                enemyCount = waveEnemyCounts[waveNumber - 1];
            }
            else
            {
                // For waves beyond the predefined ones, scale enemy count further.
                enemyCount = waveEnemyCounts[waveEnemyCounts.Length - 1] + (waveNumber - waveEnemyCounts.Length) * 2;
            }
            
            Debug.Log("Wave " + waveNumber + " starting with " + enemyCount + " enemy(ies).");
            
            // Spawn enemies one at a time.
            for (int i = 0; i < enemyCount; i++)
            {
                // Choose a random spawn point.
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                // Instantiate enemy at the chosen spawn point.
                Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                yield return new WaitForSeconds(spawnDelay);
            }
            

            yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Enemy").Length == 0);
            Debug.Log("Wave " + waveNumber + " cleared.");
            

            int pointsAwarded = (waveNumber % 3 == 0) ? 2 : 1;
            UpgradeManager.Instance.AwardPoints(pointsAwarded);
            Debug.Log("Awarded " + pointsAwarded + " upgrade point(s).");
            

            UpgradeManager.Instance.ShowUpgradePanel();
            

            yield return new WaitUntil(() => !UpgradeManager.Instance.upgradePanel.activeSelf);
            

            yield return new WaitForSeconds(timeBetweenWaves);
            
            waveNumber++;
        }
    }
}
