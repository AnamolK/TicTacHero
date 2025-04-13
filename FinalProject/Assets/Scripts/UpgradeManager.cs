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
        if (turretText != null)
            turretText.text = "Turret Level: " + turretUpgradeLevel;
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
                healthLevel++;
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
}
