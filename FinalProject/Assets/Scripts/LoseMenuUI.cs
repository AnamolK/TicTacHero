using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseSceneUI : MonoBehaviour
{
    public void RestartGame()
    {
        SceneManager.LoadScene("SampleScene"); // or Sample Scene we can change
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
