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
    public bool hasHealthRegen = false;
    public bool unlockPlayerMovement = false;
    public GameObject turretPrefab;
    public Transform turretSpawnPoint; // Optional if you're placing one

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
            healthLevel++;
            upgradePoints--;
            playerStats.IncreaseMaxHealth(healthUpgradeAmount);

            if (healthLevel == 3)
            {
                hasHealthRegen = true;
                Debug.Log(" Health Regen unlocked!");
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
                unlockPlayerMovement = true;
                Debug.Log(" Player movement-based damage upgrade unlocked!");
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

            switch (turretUpgradeLevel)
            {
                case 1:
                    Debug.Log("Turret unlocked: you can now place it.");
                    // You can instantiate turretPrefab here if desired:
                    // Instantiate(turretPrefab, turretSpawnPoint.position, Quaternion.identity);
                    break;
                case 2:
                    Debug.Log(" Turret range increased!");
                    break;
                case 3:
                    Debug.Log(" Turret damage increased!");
                    break;
            }

            UpdateUI();
        }
    }

    void Update()
    {
        if (hasHealthRegen && playerStats.currentHealth < playerStats.currentMaxHealth)
        {
            playerStats.currentHealth += 1;
            UpdateUI();
        }
    }


}
