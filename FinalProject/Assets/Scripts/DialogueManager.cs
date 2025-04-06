using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialogueBox;       // Assign the DialogueBox panel
    public Text dialogueText;            // Assign the DialogueText (Legacy)
    public Button nextButton;            // Optional: assign Button_Next

    [Header("Dialogue Lines")]
    [TextArea(2, 5)]
    [SerializeField] private string[] tutorialBefore;
    [SerializeField] private string[] tutorialAfter;
    [SerializeField] private string[] currentLines;

    public int dialogueSequence = 0;
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

    public void StartDialogue(int sequence)
    {
        dialogueSequenceSelector(sequence);
        Time.timeScale = 0;
        Debug.Log("🟢 DialogueManager: StartDialogue called!");

        if (currentLines.Length == 0)
        {
            Debug.LogWarning("⚠️ No dialogue lines assigned!");
            return;
        }

        dialogueBox.SetActive(true);
        Debug.Log("✅ dialogueBox should now be active: " + dialogueBox.activeInHierarchy);
        currentLine = 0;
        dialogueText.text = currentLines[currentLine];
        dialogueActive = true;
    }

    private void dialogueSequenceSelector(int sequence) {
        if (sequence == 0) {
            currentLines = tutorialBefore;
        } else if (sequence == 1) {
            currentLines = tutorialAfter;
        }
    }

    public void DisplayNextLine()
    {
        currentLine++;
        if (currentLine < currentLines.Length)
        {
            dialogueText.text = currentLines[currentLine];
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        dialogueBox.SetActive(false);
        dialogueActive = false;
        Time.timeScale = 1;
    }
}
