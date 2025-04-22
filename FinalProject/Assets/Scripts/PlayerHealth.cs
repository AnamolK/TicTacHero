using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    // Reference to the PlayerStats component that holds health values.
    private PlayerStats playerStats;

    // UI elements for displaying health and game over text.
    public TMP_Text healthText;
    public TMP_Text gameOverText;

    // Time (in seconds) between damage ticks.
    public float damageInterval = 1f;

    private Coroutine damageCoroutine;

    // Audio/SFX variables.
    private AudioSource audioSource;
    public AudioClip[] soundList;
    private AudioClip selected;

    // Store the last displayed health to detect changes.
    private int lastDisplayedHealth;

    //animation/asset manager
    [SerializeField] private GameObject asset;
    [SerializeField] private GameObject vcam;
    CameraShake shake;
    private new AnimationGeneric animation;

    void Start()
    {
        // Get the PlayerStats component.
        playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats component missing from Player!");
        }
        
        UpdateUI();
        lastDisplayedHealth = playerStats.currentHealth;
        
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);

        audioSource = GetComponent<AudioSource>();
        animation = asset.GetComponent<AnimationGeneric>();
        shake = vcam.GetComponent<CameraShake>();
    }

    void Update()
    {
        // Check if current health has changed since the last UI update.
        if (playerStats.currentHealth != lastDisplayedHealth ||
            playerStats.currentMaxHealth != lastDisplayedHealth) // optional: if you want to update for max health changes too
        {
            UpdateUI();
            lastDisplayedHealth = playerStats.currentHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        // Subtract damage from current health in PlayerStats.
        playerStats.currentHealth -= damage;
        playSFX();
        animation.DamageTaken(0.5f);
        shake.ShakeCamera(0.3f);
        UpdateUI();
        lastDisplayedHealth = playerStats.currentHealth;
        Debug.Log("Player took damage. Current health: " + playerStats.currentHealth);

        if (playerStats.currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        // Heal the player without exceeding the current maximum.
        playerStats.currentHealth = Mathf.Min(playerStats.currentMaxHealth, playerStats.currentHealth + amount);
        UpdateUI();
        lastDisplayedHealth = playerStats.currentHealth;
    }

    void UpdateUI()
    {
        // Update the health text to display current and maximum health.
        if (healthText != null)
            healthText.text = "Health: " + playerStats.currentHealth + " / " + playerStats.currentMaxHealth;
    }

    void Die()
    {
        SceneManager.LoadScene("LoseScene"); 
    }

    // When an enemy collides, start applying damage over time.
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (damageCoroutine == null)
            {
                damageCoroutine = StartCoroutine(ApplyDamageOverTime());
            }
        }
    }

    // When the enemy leaves, stop the damage coroutine.
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

    // Coroutine to apply damage repeatedly at fixed intervals.
    IEnumerator ApplyDamageOverTime()
    {
        // Apply damage immediately.
        TakeDamage(1);
        yield return new WaitForSeconds(damageInterval);

        while (true)
        {
            TakeDamage(1);
            yield return new WaitForSeconds(damageInterval);
        }
    }

    void playSFX()
    {
        int index = Random.Range(0, soundList.Length);
        selected = soundList[index];
        audioSource.clip = selected;
        audioSource.PlayOneShot(selected);
    }
}