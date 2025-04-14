using System.Collections;
using UnityEngine;
using TMPro;

public class WaveManagerEndless : MonoBehaviour
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
    [System.Serializable] 
    public struct EnemySettings {
        public int typeInd;
        public int health;
        public int atk;
        public bool dropHp;
        public float spawnDelay;
    }

    [System.Serializable] 
    public struct waveContent {
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
    public int waveNumber = 1;

    void Start()
    {
        if (CheckpointManager.checkpointWave > 0) { 
            waveNumber = CheckpointManager.checkpointWave; 
            CheckpointManager.checkpointWave = 0; 
        }
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        DialogueManager dm = FindObjectOfType<DialogueManager>();
        while (true)
        {
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
            int enemyCount = 0;
            waveContent currWave = new waveContent();

            if (waveNumber <= waveArray.Length)
            {
                enemyCount = waveArray[waveNumber - 1].numEnemies;
                currWave = waveArray[waveNumber - 1];
            }
            else
            {
                // Endless mode
                enemyCount = waveArray[waveArray.Length - 1].numEnemies + (waveNumber - waveArray.Length) * 2;
                currWave.numEnemies = enemyCount;
                currWave.StartDialogue = -1;
                currWave.EndDialogue = -1;
                currWave.enemies = new EnemySettings[enemyCount];
                for (int i = 0; i < enemyCount; i++)
                {
                    EnemySettings es = new EnemySettings();

                    es.typeInd = 0;
                   
                    es.health = waveArray[waveArray.Length - 1].enemies[0].health + (waveNumber - waveArray.Length) * 2;

                    es.atk = waveArray[waveArray.Length - 1].enemies[0].atk + (waveNumber - waveArray.Length);
                    es.dropHp = waveArray[waveArray.Length - 1].enemies[0].dropHp;
                    es.spawnDelay = spawnDelay;
                    currWave.enemies[i] = es;
                }
            }

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
