using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip[] soundList;
    private AudioClip selected;

    // Reference to the enemy currently in the attackable area.
    private EnemyPathfinder currentEnemy;

    // Reference to the PlayerStats component for upgraded attack damage.
    private PlayerStats playerStats;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogWarning("PlayerStats component not found on the PlayerAttack GameObject!");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Attackable"))
        {
            EnemyPathfinder enemyPathfinder = collision.GetComponentInParent<EnemyPathfinder>();
            if (enemyPathfinder != null)
            {
                currentEnemy = enemyPathfinder;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Attackable"))
        {
            EnemyPathfinder enemyPathfinder = collision.GetComponentInParent<EnemyPathfinder>();
            if (enemyPathfinder != null && enemyPathfinder == currentEnemy)
            {
                currentEnemy = null;
            }
        }
    }

    void Update()
    {
        if (currentEnemy != null)
        {
            Vector2 enemyPos = currentEnemy.transform.position;
            Vector2 playerPos = transform.position;
            Vector2 diff = playerPos - enemyPos;
            string hitSide = DetermineHitSide(diff);

            if (CheckInputAgainstHitSide(hitSide))
            {
                // Only register damage if the hit side is not the enemy's active attack side.
                if (hitSide != currentEnemy.currentAttackSide)
                {
                    Debug.Log("Attack Hit: " + hitSide);
                    // Get damage from PlayerStats; if not found, default to 1.
                    int damage = (playerStats != null) ? playerStats.currentAttackDamage : 1;
                    currentEnemy.TakeDamage(damage);
                    PlaySFX();
                }
                else
                {
                    Debug.Log("Attack failed: Hit enemy's active attack side.");
                }
            }
        }
    }

    bool CheckInputAgainstHitSide(string side)
    {
        if (Input.GetKeyDown(KeyCode.D) && side == "Left")
            return true;
        else if (Input.GetKeyDown(KeyCode.A) && side == "Right")
            return true;
        else if (Input.GetKeyDown(KeyCode.W) && side == "Down")
            return true;
        else if (Input.GetKeyDown(KeyCode.S) && side == "Up")
            return true;
        else
            return false;
    }

    string DetermineHitSide(Vector2 diff)
    {
        diff.Normalize();
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            return diff.x > 0 ? "Right" : "Left";
        else
            return diff.y > 0 ? "Up" : "Down";
    }

    void PlaySFX()
    {
        int index = Random.Range(0, soundList.Length);
        selected = soundList[index];
        audioSource.clip = selected;
        audioSource.Play();
    }
}
