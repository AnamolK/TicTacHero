using TMPro;
using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    public TMP_Text healthText;
    public TMP_Text gameOverText;

    // Time in seconds between damage ticks
    public float damageInterval = 1f;


    private Coroutine damageCoroutine;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateUI();
        Debug.Log("Player took damage. Current health: " + currentHealth);
        if (currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UpdateUI();
    }

    void UpdateUI()
    {
        if (healthText != null)
            healthText.text = "Health: " + currentHealth;
    }

    void Die()
    {
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = "Game Over!";
        }
        Time.timeScale = 0f;
    }

    // Start damage coroutine when an enemy enters the trigger
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // If a coroutine isnt running, start one
            if (damageCoroutine == null)
            {
                damageCoroutine = StartCoroutine(ApplyDamageOverTime());
            }
        }
    }

    // Stop coroutine when the enemy leaves the trigger
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }

    // Coroutine that repeatedly applies damage at fixed intervals
    IEnumerator ApplyDamageOverTime()
    {
        // Apply damage immediately upon entering
        TakeDamage(1);
        yield return new WaitForSeconds(damageInterval);

        while (true)
        {
            TakeDamage(1);
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
