using System.Collections;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [Header("Enemy Settings")]

    public GameObject[] enemiesTypes;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Wave Settings")]
    // Delay between individual enemy spawns within a wave.
    public float spawnDelay = 0.5f;
    // Delay after a wave is cleared before the next wave begins.
    public float timeBetweenWaves = 2f;

    public waveContent[] waveArray;
    [System.Serializable] public struct EnemySettings {
        public int typeInd;
        public int health;
        public int atk;
        public bool dropHp;
        public float spawnDelay;
    }

    [System.Serializable] public struct waveContent {
        public EnemySettings[] enemies;
        public int numEnemies;
        public int StartDialogue;
        public int EndDialogue;
    }

    [Header("UI Elements")]
    public TMP_Text waveText;
    // Duration to show the wave message.
    public float waveDisplayDuration = 2f;
 
    // Current wave number.
    public int waveNumber = 0;

    [Header("Intro Slime")]
    public GameObject slimeBossPrefab;
    public Vector3 slimeSpawnPosition = new Vector3(2,9,0);
    public int slimeIntroDialogue = -1;

    void Start()
    {
        DialogueManager dm = FindObjectOfType<DialogueManager>();
        if (slimeBossPrefab != null)
        {
            GameObject slime = Instantiate(slimeBossPrefab, slimeSpawnPosition, Quaternion.identity);
            if (slimeIntroDialogue != -1)
                dm.StartDialogue(slimeIntroDialogue);
        }
        if (CheckpointManager.checkpointWave > 0) { waveNumber = CheckpointManager.checkpointWave; CheckpointManager.checkpointWave = 0; }
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        DialogueManager dm = FindObjectOfType<DialogueManager>();
        while (true)
        {
            // tutorial wave
            if (waveNumber == 0)
            {
                if (waveText != null)
                {
                    waveText.text = "Tutorial";
                    waveText.gameObject.SetActive(true);
                }
                yield return new WaitForSeconds(waveDisplayDuration);

                if (waveText != null)
                    waveText.gameObject.SetActive(false);

               
                GameObject prefab = enemiesTypes[waveArray[0].enemies[0].typeInd];
                Vector3 spawnPos = slimeSpawnPosition;
                GameObject tutEnemy = Instantiate(prefab, spawnPos, Quaternion.identity);
                EnemyPathfinder epTut = tutEnemy.GetComponent<EnemyPathfinder>();
                epTut.moveTickDuration = Mathf.Infinity;

                Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
                Vector2 diff = new Vector2(playerPos.x - spawnPos.x, playerPos.y - spawnPos.y);
                if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
                    epTut.currentAttackSide = diff.x > 0 ? "Right" : "Left";
                else
                    epTut.currentAttackSide = diff.y > 0 ? "Up" : "Down";

                yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Enemy").Length == 0);
                waveNumber = 1;
                continue;
            }

            // 1) Display wave text
            if (waveText != null)
            {
                waveText.text = "Wave " + waveNumber;
                waveText.gameObject.SetActive(true);
            }
            yield return new WaitForSeconds(waveDisplayDuration);

            if (waveText != null)
                waveText.gameObject.SetActive(false);

            // get enemy type and count for this wave
            int enemyCount = waveArray[waveNumber-1].numEnemies;
            waveContent currWave = waveArray[waveNumber-1];

            Debug.Log("Wave " + waveNumber + " starting with " + enemyCount + " enemies.");

            if (currWave.StartDialogue != -1) {
                dm.StartDialogue(currWave.StartDialogue);
            }

            for (int i = 0; i < enemyCount; i++) {
                
                // Choose random spawn point
                Transform spawnPoint = spawnPoints[i % 5];
                GameObject prefabToSpawn = null;

                EnemySettings currEnemy = currWave.enemies[i];

                // Instantiate the enemy
                prefabToSpawn = enemiesTypes[currEnemy.typeInd];
                GameObject enemyInstance = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);

                //set enemy stats
                EnemyPathfinder ep = enemyInstance.GetComponent<EnemyPathfinder>();

                ep.maxHealth = currEnemy.health;

                // Possibly mark this enemy to drop a potion
                if (currEnemy.dropHp)
                {
                    if (ep != null)
                        ep.willDropPotion = true;
                }

                yield return new WaitForSeconds(spawnDelay);
            }

            // After Each wave (all enemies dead) ----------------------------------------------------
            yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Enemy").Length == 0);
            Debug.Log("Wave " + waveNumber + " cleared.");

            // Award points
            int pointsAwarded = (waveNumber % 3 == 0) ? 2 : 1;
            UpgradeManager.Instance.AwardPoints(pointsAwarded);
            Debug.Log("Awarded " + pointsAwarded + " upgrade point(s).");

            // Apply HP Regen effect if unlocked: heal player by 1 health per wave.
            if(UpgradeManager.Instance.playerStats != null && UpgradeManager.Instance.playerStats.regenUnlocked)
            {
                UpgradeManager.Instance.playerStats.Heal(1);
                Debug.Log("HP Regen applied: Recovered 1 health.");
            }

            // Set checkpoint if wave is a multiple of 3
            if (waveNumber % 3 == 0)
                CheckpointManager.Instance.SetCheckpoint(waveNumber);

            //trigger end dialogue
            if (currWave.EndDialogue != -1) {
                dm.StartDialogue(currWave.EndDialogue);
                yield return new WaitUntil(() => !dm.dialogueActive);
            }

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
