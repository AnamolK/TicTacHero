using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialogueBox;      
    public Text dialogueText;           
    public Button nextButton;           

    [Header("Dialogue Lines")]
    [TextArea(2, 5)]
    public string[] lines;

    private int currentLine = 0;
    private bool dialogueActive = false;

    void Start()
    {
        //dialogueBox.SetActive(false); // Start hidden

        if (nextButton != null)
            nextButton.onClick.AddListener(DisplayNextLine);
    }

    void Update()
    {
        if (dialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            DisplayNextLine();
        }
    }

    public void StartDialogue()
    {

        Debug.Log("🟢 DialogueManager: StartDialogue called!");

        if (lines.Length == 0)
        {
            Debug.LogWarning("⚠️ No dialogue lines assigned!");
            return;
        }

        dialogueBox.SetActive(true);
        FindObjectOfType<WaveManager>().enabled = false;
        Debug.Log("✅ dialogueBox should now be active: " + dialogueBox.activeInHierarchy);
        currentLine = 0;
        dialogueText.text = lines[currentLine];
        dialogueActive = true;
    }

    public void DisplayNextLine()
    {
        currentLine++;
        if (currentLine < lines.Length)
        {
            dialogueText.text = lines[currentLine];
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        dialogueBox.SetActive(false);
        FindObjectOfType<WaveManager>().enabled = true;
        dialogueActive = false;
    }
}
