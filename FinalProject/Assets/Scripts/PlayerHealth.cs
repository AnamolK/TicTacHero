using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;


public class PlayerHealth : MonoBehaviour
{
    // Reference to the PlayerStats component that holds health values.
    private PlayerStats playerStats;

    // UI elements for displaying health and game over text.
    public GameObject heartHolder;
    public GameObject heartImg;
    public Sprite[] hearts;
    public TMP_Text gameOverText;

    // Time (in seconds) between damage ticks.
    public float damageInterval = 1.5f;

    private Coroutine damageCoroutine;
    private Collider2D currCollision;

    // Audio/SFX variables.
    private AudioSource audioSource;
    public AudioClip[] soundList;
    private AudioClip selected;

    // Store the last displayed health to detect changes.
    private int lastMaxHp;
    private int lastCurrHp;
    private int lastHeartSpot;

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
        lastMaxHp = playerStats.currentMaxHealth;
        lastCurrHp = playerStats.currentHealth;
        
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);

        audioSource = GetComponent<AudioSource>();
        animation = asset.GetComponent<AnimationGeneric>();
        shake = vcam.GetComponent<CameraShake>();
    }

    void FixedUpdate()
    {
        // Check if current health has changed since the last UI update.
        if (playerStats.currentHealth != lastCurrHp || playerStats.currentMaxHealth != lastMaxHp)
        {
            UpdateUI();
            lastMaxHp = playerStats.currentMaxHealth;
            lastCurrHp = playerStats.currentHealth;
        }

        if (currCollision != null) {
            Debug.Log(currCollision.tag);
            if (!currCollision.CompareTag("Enemy"))
            {
                
                if (damageCoroutine != null)
                {
                    StopCoroutine(damageCoroutine);
                    damageCoroutine = null;
                }
            }
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
        lastMaxHp = playerStats.currentMaxHealth;
        lastCurrHp = playerStats.currentHealth;

        if (playerStats.currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        // Heal the player without exceeding the current maximum.
        playerStats.currentHealth = Mathf.Min(playerStats.currentMaxHealth, playerStats.currentHealth + amount);
        UpdateUI();
        lastMaxHp = playerStats.currentMaxHealth;
        lastCurrHp = playerStats.currentHealth;
    }

    void UpdateUI()
    {
        // // Update the health text to display current and maximum health.
        // if (healthText != null)
        //     healthText.text = "Health: " + playerStats.currentHealth + " / " + playerStats.currentMaxHealth;

        //reset hearts
        foreach (Transform child in heartHolder.transform)
            Destroy(child.gameObject);

        for (int i = 0; i < playerStats.currentMaxHealth; i++)
        {
            GameObject heart = Instantiate(heartImg, heartHolder.transform);
            RectTransform rt = heart.GetComponent<RectTransform>();
            
            //shrink heart overall distance (DOESNT WORK)
            if (playerStats.currentMaxHealth * 90 > 600) {
                int transformAmount = 500/playerStats.currentMaxHealth;
                rt.anchoredPosition = new Vector2(i*transformAmount, 0);
            } else  {
                rt.anchoredPosition = new Vector2(i * 90, 0);
            }

            // Change sprite if within currentHealth
            Image heartImage = heart.GetComponent<Image>();
            if (i < playerStats.currentHealth)
                heartImage.sprite = hearts[0];
            else
                heartImage.sprite = hearts[1];
        }
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
            currCollision = collision;
            if (damageCoroutine == null)
            {
                damageCoroutine = StartCoroutine(ApplyDamageOverTime());
            }
        }

        if (collision.CompareTag("AOE"))
        {
            TakeDamage(2);
        }
    }

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