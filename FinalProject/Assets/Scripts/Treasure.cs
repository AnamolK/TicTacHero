using UnityEngine;

public class Treasure : MonoBehaviour
{
    public int treasureValue = 1;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Update the village treasure count.
            VillageManager villageManager = FindObjectOfType<VillageManager>();
            if (villageManager != null)
            {
                villageManager.AddTreasure(treasureValue);
            }
            Destroy(gameObject);
        }
    }
}
