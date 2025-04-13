using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int baseMaxHealth = 5;
    public int currentMaxHealth;
    public int currentHealth;

    public int baseAttackDamage = 1;
    public int currentAttackDamage;

    // HP Regen flag unlocked with health upgrade level 3.
    public bool regenUnlocked = false;

    void Start()
    {
        currentMaxHealth = baseMaxHealth;
        currentHealth = baseMaxHealth;
        currentAttackDamage = baseAttackDamage;
    }
    public void IncreaseMaxHealth(int amount)
    {
        int oldMax = currentMaxHealth;
        currentMaxHealth += 1;
        
        // If player isn't at full health, heal by 1.
        if (currentHealth < oldMax)
        {
            currentHealth += 1;
            if (currentHealth > currentMaxHealth)
                currentHealth = currentMaxHealth;
        }
        else
        {
            // If already full, update current health to new max.
            currentHealth = currentMaxHealth;
        }
        Debug.Log("Player max health increased to: " + currentMaxHealth + ". Current health: " + currentHealth);
    }

    public void IncreaseAttackDamage(int amount)
    {
        currentAttackDamage += amount;
        Debug.Log("Player attack damage increased to: " + currentAttackDamage);
    }
    
    public void UnlockHealthRegen()
    {
        regenUnlocked = true;
        Debug.Log("Player health regen unlocked.");
    }

    // New Heal method added to support the HP Regen functionality in WaveManager.
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentMaxHealth, currentHealth + amount);
        Debug.Log("Player healed " + amount + " point(s). Current health: " + currentHealth);
    }
}
