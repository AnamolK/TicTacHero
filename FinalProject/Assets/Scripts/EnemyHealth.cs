using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 5;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0) {
            Die();
        } else {
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
