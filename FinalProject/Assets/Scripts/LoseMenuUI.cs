using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseSceneUI : MonoBehaviour
{
    public void RestartGame()
    {
        Debug.Log("RestartGame button clicked.");
        GameObject loseCanvas = GameObject.Find("LoseSceneCanvas");
        if (loseCanvas != null)
        {
            Destroy(loseCanvas);
        }
        if (CheckpointManager.Instance != null)
        {
            CheckpointManager.Instance.RestartFromCheckpoint();
        }
        else
        {
            SceneManager.LoadScene("GameScene");
        }
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
