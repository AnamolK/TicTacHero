using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    public string mainMenuSceneName = "MainMenu";
    public string gameSceneName = "SampleScene";
    public string winSceneName = "WinScene";
    public string loseSceneName = "LoseScene";

    void Start()
    {
        Debug.Log("GameHandler.Start() called");

        DialogueManager dm = FindObjectOfType<DialogueManager>();
        if (dm == null)
        {
            Debug.LogWarning(" No DialogueManager found in this scene!");
        }
        else
        {
            Debug.Log("DialogueManager found. Starting dialogue...");
            dm.StartDialogue(0); // activate the DialogueBox
        }
    }

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
