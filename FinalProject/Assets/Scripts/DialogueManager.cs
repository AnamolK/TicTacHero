using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialogueBox;     
    public TMP_Text dialogueText;        
    public Button nextButton;

    [Header("Dialogue Lines")]
    [TextArea(2, 5)]
    
    private string[] currentLines;
    [SerializeField] private string[] tutorialBefore;
    [SerializeField] private string[] tutorialAfter;
    
    [SerializeField] private string[] dragonBefore;
    [SerializeField] private string[] dragonAfter;
    [SerializeField] private string[] WarningAfter;
    [SerializeField] private string[] Warning2After;
    [SerializeField] private string[] SlimeBefore;
    [SerializeField] private string[] SlimeAfter;

    private int currentLine = 0;
    public bool dialogueActive = false;

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
        } else if (sequence == 2) {
            currentLines = dragonBefore;
        } else if (sequence == 3) {
            currentLines = dragonAfter;
        } else if (sequence == 4) {
            currentLines = WarningAfter;
        } else if (sequence == 5) {
            currentLines = Warning2After;
        } else if (sequence == 6) {
            currentLines = SlimeBefore;
        } else if (sequence == 7) {
            currentLines = SlimeAfter;
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
