using UnityEngine;
using UnityEngine.UI;

public class VillageManager : MonoBehaviour
{
    public int treasureCount = 0;
    public Text treasureText;  // Assign via the Canvas.

    void Start()
    {
        UpdateUI();
    }

    public void AddTreasure(int amount)
    {
        treasureCount += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (treasureText != null)
        {
            treasureText.text = "Treasure: " + treasureCount;
        }
    }
}
