using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int baseMaxHealth = 5;
    public int currentMaxHealth;
    public int currentHealth;

    public int baseAttackDamage = 1;
    public int currentAttackDamage;

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
}
