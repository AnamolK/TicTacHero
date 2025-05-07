using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenuUI; 


    private Button resumeButton;
    private Button restartButton;
    private Button quitButton;

    private bool isPaused = false;
    public Slider volumeSlider;
    public AudioMixer audioMixer;
    


    void Start()
    {
        // buttons inside PAUSEMENU
        resumeButton = pauseMenuUI.transform.Find("Button_Resume").GetComponent<Button>();
        restartButton = pauseMenuUI.transform.Find("Button_Restart").GetComponent<Button>();
        quitButton = pauseMenuUI.transform.Find("Button_Quit").GetComponent<Button>();

        resumeButton.onClick.AddListener(ResumeGame);
        restartButton.onClick.AddListener(RestartGame);
        quitButton.onClick.AddListener(QuitGame);

        if (volumeSlider != null)
    {
        float currentVolume;
        audioMixer.GetFloat("MusicVolume", out currentVolume);
        volumeSlider.value = Mathf.Pow(10f, currentVolume / 20f);
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }


        pauseMenuUI.SetActive(false); // hidden at start
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
        Debug.Log("[Game] PauseGame() called");
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        Debug.Log("[Game] ResumeGame() called");
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SetVolume(float sliderValue)
    {
        Debug.Log($"[Audio] Slider changed to {sliderValue}");
        float dB = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("MusicVolume", dB); //  USE THE NAME FROM THE MIXER
    }



    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
