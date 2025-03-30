using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("Pause Menu Prefab")]
    public GameObject pauseMenuPrefab;  // Drag PAUSEMENU prefab here

    private GameObject pauseMenuInstance;

    private Button resumeButton;
    private Button restartButton;
    private Button quitButton;

    private bool isPaused = false;

    void Start()
    {
        // Instantiate PAUSEMENU
        pauseMenuInstance = Instantiate(pauseMenuPrefab);
        pauseMenuInstance.name = "PAUSEMENU"; // Optional, avoid (Clone) suffix
        pauseMenuInstance.SetActive(false);

        // Find buttons directly inside the prefab
        resumeButton = pauseMenuInstance.transform.Find("Button_Resume")?.GetComponent<Button>();
        restartButton = pauseMenuInstance.transform.Find("Button_Restart")?.GetComponent<Button>();
        quitButton = pauseMenuInstance.transform.Find("Button_Quit")?.GetComponent<Button>();

        // Safety check
        if (resumeButton == null) Debug.LogError("Button_Resume not found in prefab!");
        if (restartButton == null) Debug.LogError("Button_Restart not found in prefab!");
        if (quitButton == null) Debug.LogError("Button_Quit not found in prefab!");

        // Hook up actions
        if (resumeButton != null) resumeButton.onClick.AddListener(ResumeGame);
        if (restartButton != null) restartButton.onClick.AddListener(RestartGame);
        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);
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
        pauseMenuInstance.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuInstance.SetActive(false);
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
