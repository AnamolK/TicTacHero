using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    public string mainMenuSceneName = "MainMenu";
    public string gameSceneName = "SampleScene"; // or your real game scene name
    public string winSceneName = "WinScene";
    public string loseSceneName = "LoseScene";

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void WinGame()
    {
        SceneManager.LoadScene(winSceneName);
    }

    public void LoseGame()
    {
        SceneManager.LoadScene(loseSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
