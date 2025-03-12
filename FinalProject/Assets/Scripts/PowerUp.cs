using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { Speed, Health };
    public PowerUpType powerUpType;
    public float powerUpAmount = 1f;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                ApplyPowerUp(player);
            }
            Destroy(gameObject);
        }
    }

    void ApplyPowerUp(PlayerController player)
    {
        switch (powerUpType)
        {
            case PowerUpType.Speed:
                player.moveSpeed += powerUpAmount;
                break;
            case PowerUpType.Health:
                PlayerHealth health = player.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    health.Heal((int)powerUpAmount);
                }
                break;
        }
    }
}
