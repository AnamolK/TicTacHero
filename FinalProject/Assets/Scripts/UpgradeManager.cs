using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [Header("Upgrade Points")]
    public int upgradePoints = 0;

    [Header("Player Upgrade Levels")]
    public int healthLevel = 0;
    public int attackLevel = 0;

    [Header("Upgrade Values")]

    public int healthUpgradeAmount = 1;
    public int attackUpgradeAmount = 1;

    [Header("UI Elements")]
    public GameObject upgradePanel;     
    public TMP_Text pointsText;          
    public TMP_Text healthText;          
    public TMP_Text attackText;         
    public Button upgradeHealthButton;   
    public Button upgradeAttackButton;   
    public Button continueButton;      
    [Header("Player Reference")]
    public PlayerStats playerStats;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (upgradePanel != null)
            upgradePanel.SetActive(false);
        UpdateUI();

        if (upgradeHealthButton != null)
            upgradeHealthButton.onClick.AddListener(UpgradeHealth);
        if (upgradeAttackButton != null)
            upgradeAttackButton.onClick.AddListener(UpgradeAttack);
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinue);

        // If not assigned, try to find the player stats by tag.
        if (playerStats == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                playerStats = playerObj.GetComponent<PlayerStats>();
        }
    }

    void UpdateUI()
    {
        if (pointsText != null)
            pointsText.text = "Upgrade Points: " + upgradePoints;
        if (healthText != null)
            healthText.text = "Health Level: " + healthLevel;
        if (attackText != null)
            attackText.text = "Attack Level: " + attackLevel;
    }

    public void AwardPoints(int points)
    {
        upgradePoints += points;
        UpdateUI();
    }

    public void ShowUpgradePanel()
    {
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(true);
            // Pause the game while upgrading.
            Time.timeScale = 0f;
        }
    }

    public void HideUpgradePanel()
    {
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
            // Resume the game.
            Time.timeScale = 1f;
        }
    }

    void OnContinue()
    {
        HideUpgradePanel();
    }

    public void UpgradeHealth()
    {
        if (upgradePoints > 0 && playerStats != null)
        {
            healthLevel++;
            upgradePoints--;
            playerStats.IncreaseMaxHealth(healthUpgradeAmount);
            UpdateUI();
        }
    }

    public void UpgradeAttack()
    {
        if (upgradePoints > 0 && playerStats != null)
        {
            attackLevel++;
            upgradePoints--;
            playerStats.IncreaseAttackDamage(attackUpgradeAmount);
            UpdateUI();
        }
    }
}
