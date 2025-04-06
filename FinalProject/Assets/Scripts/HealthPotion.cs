using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    public int healAmount = 1;

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("HealthPotion: OnTriggerEnter2D triggered by: " + collision.gameObject.name + " with tag: " + collision.tag);
        PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
        if(playerHealth == null)
            playerHealth = collision.GetComponentInParent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.Heal(healAmount);
            Debug.Log("HealthPotion: Player healed by " + healAmount);
            Debug.Log("HealthPotion: Destroying potion.");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("HealthPotion: Collided object is not a player.");
        }
    }
}
