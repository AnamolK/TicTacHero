using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;
    public static int checkpointWave = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void SetCheckpoint(int wave)
    {
        checkpointWave = wave;
    }

    public void RestartFromCheckpoint()
    {
        int newWave = (checkpointWave > 0) ? checkpointWave : 1;
        PlayerStats stats = FindObjectOfType<PlayerStats>();
        if (stats != null)
            stats.currentHealth = stats.currentMaxHealth;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
