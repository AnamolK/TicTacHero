using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    public string mainMenuSceneName = "MainMenu";
    public string gameSceneName = "SampleScene";
    public string winSceneName = "WinScene";
    public string loseSceneName = "LoseScene";

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void Credits()
    {
        SceneManager.LoadScene("CreditsScene");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
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
}
