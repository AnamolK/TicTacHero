using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenuUI; // Assign PAUSEMENU object here

    private Button resumeButton;
    private Button restartButton;
    private Button quitButton;

    private bool isPaused = false;

    void Start()
    {
        // Find buttons inside the existing PAUSEMENU
        resumeButton = pauseMenuUI.transform.Find("Button_Resume").GetComponent<Button>();
        restartButton = pauseMenuUI.transform.Find("Button_Restart").GetComponent<Button>();
        quitButton = pauseMenuUI.transform.Find("Button_Quit").GetComponent<Button>();

        // Hook up buttons
        resumeButton.onClick.AddListener(ResumeGame);
        restartButton.onClick.AddListener(RestartGame);
        quitButton.onClick.AddListener(QuitGame);

        pauseMenuUI.SetActive(false); // Make sure it starts hidden
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
