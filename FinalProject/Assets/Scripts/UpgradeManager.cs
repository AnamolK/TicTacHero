using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;
    public int upgradePoints = 0;
    public int healthLevel = 0;
    public int attackLevel = 0;
    public int healthUpgradeAmount = 1;
    public int attackUpgradeAmount = 1;
    public int turretUpgradeLevel = 0;
    public int turretUpgradeCost = 3;
    public float turretUpgradeDamageIncrease = 1f;
    public GameObject upgradePanel;
    public TMP_Text pointsText;
    public TMP_Text healthText;
    public TMP_Text attackText;
    public TMP_Text turretText;
    public Button upgradeHealthButton;
    public Button upgradeAttackButton;
    public Button turretUpgradeButton;
    public Button continueButton;
    public PlayerStats playerStats;
    public bool unlockPlayerMovement = false;
    public TMP_Text upgradeHealthButtonText;
    public TMP_Text upgradeAttackButtonText;
    public TMP_Text turretUpgradeButtonText;
    public GameObject infoPopup;
    public TMP_Text infoText; 
    public Button infoButton;
    public Button closeInfoButton;



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
        if (turretUpgradeButton != null)
            turretUpgradeButton.onClick.AddListener(UpgradeTurret);
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinue);
        if (infoButton != null)
            infoButton.onClick.AddListener(ShowInfoPopup);

        if (closeInfoButton != null)
            closeInfoButton.onClick.AddListener(HideInfoPopup);


        
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
            pointsText.text = "Points: " + upgradePoints;
        if (upgradeHealthButtonText != null)
            upgradeHealthButtonText.text = "Lvl: " + healthLevel;
        if (upgradeAttackButtonText != null)
            upgradeAttackButtonText.text = "Lvl: " + attackLevel;
        if (turretUpgradeButtonText != null)
            turretUpgradeButtonText.text = "Lvl: " + turretUpgradeLevel;

        if (upgradeAttackButtonText != null)
        {
            if (attackLevel == 2)
            {
                attackText.text = "Unlock ATK: Stun";
            }
            else if (attackLevel == 3)
            {
                attackText.text = "Unlock ATK: Dash";
            }
            else if (attackLevel == 4)
            {
                attackText.text = "Unlock ATK: AOE";
            }
            else if (attackLevel > 4)
            {
                attackText.text = "ATK: Maxed";
            }
            else
            {
                attackText.text = "ATK: +1 dmg";
            }
        }

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
            Time.timeScale = 0f;
        }
    }
    public void HideUpgradePanel()
    {
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
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
            upgradePoints--;
            if (healthLevel == 2) {
                // 3rd level: Unlock HP Regen instead of increasing max health.
                healthLevel += 2;
                playerStats.UnlockHealthRegen();
                Debug.Log("HP Regen unlocked: 1 health per wave.");
            }
            else
            {
                healthLevel++;
                playerStats.IncreaseMaxHealth(healthUpgradeAmount);
            }
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

            if (attackLevel == 3)
            {
                playerStats.UnlockStun();
                DialogueManager.Instance.StartDialogue(9);
                Debug.Log("Stun unlocked at Level 3!");
            }

            if (attackLevel == 4)
            {
                playerStats.UnlockDash();
                DialogueManager.Instance.StartDialogue(8);
                Debug.Log("Dash unlocked at Level 4!");
            }
            if (attackLevel == 5)
            {
                playerStats.UnlockAOEAttack();
                DialogueManager.Instance.StartDialogue(10);
                Debug.Log("AOE attack unlocked at Level 5!");
            }

            UpdateUI();
        }
    }


    public void UpgradeTurret()
    {
        if (upgradePoints >= turretUpgradeCost)
        {
            turretUpgradeLevel++;
            upgradePoints -= turretUpgradeCost;
            UpdateUI();
        }
    }

    public void ShowInfoPopup()
    {
        infoPopup.SetActive(true);
        Time.timeScale = 0f; 
    }

    public void HideInfoPopup()
    {
        infoPopup.SetActive(false);
    }
}

