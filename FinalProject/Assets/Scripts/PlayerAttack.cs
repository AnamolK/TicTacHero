using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    //audio manager
    private AudioSource audioSource;
    public AudioClip[] soundList;
    private AudioClip selected;

    // Reference to the enemy currently in the attackable area.
    private EnemyPathfinder currentEnemy;
    // private DragonPathfinder currentEnemyDragon;

    // Reference to the PlayerStats component for upgraded attack damage.
    private PlayerStats playerStats;

    //animation/asset manager
    [SerializeField] private GameObject asset;
    private new AnimationGeneric animation;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        playerStats = GetComponent<PlayerStats>();
        animation = asset.GetComponent<AnimationGeneric>();
        
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
            // DragonPathfinder enemyPathfinderDragon = collision.GetComponentInParent<DragonPathfinder>();
            if (enemyPathfinder != null)
            {
                currentEnemy = enemyPathfinder;
            } 
            // else if (enemyPathfinderDragon != null)
            // {   
            //     currentEnemyDragon = enemyPathfinderDragon;
            // }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Attackable"))
        {
            EnemyPathfinder enemyPathfinder = collision.GetComponentInParent<EnemyPathfinder>();
            // DragonPathfinder enemyPathfinderDragon = collision.GetComponentInParent<DragonPathfinder>();
            if (enemyPathfinder != null && enemyPathfinder == currentEnemy)
            {
                currentEnemy = null;
            }
            // else if (enemyPathfinderDragon != null && enemyPathfinderDragon == currentEnemy)
            // {
            //     currentEnemyDragon = null;
            // }
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
                    playSFX();
                    currentEnemy.TakeDamage(damage);
                    if (playerStats != null && playerStats.aoeAttackUnlocked)
                    {
                        float aoeRadius = 2.5f; // adjust to taste
                        // Cap AOE damage to 2 regardless of player's attack stat.
                        int aoeDamage = Mathf.Min(damage, 2);
                        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, aoeRadius);

                        foreach (Collider2D hit in hits)
                        {
                            if (hit.CompareTag("Attackable"))
                            {
                                EnemyPathfinder enemy = hit.GetComponentInParent<EnemyPathfinder>();
                                DragonPathfinder dragon = hit.GetComponentInParent<DragonPathfinder>();

                                if ((enemy != null && enemy != currentEnemy) || dragon != null)
                                {
                                    if (enemy != null) enemy.TakeDamage(aoeDamage);
                                    if (dragon != null) dragon.TakeDamage(aoeDamage);
                                }
                            }
                        }

                        Debug.Log(" AOE attack applied to nearby enemies!");
                    }
                    animation.AttackMelee(AttackAnimationDirection(hitSide), 0.2f);
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

    Vector3 AttackAnimationDirection(string side)
    {
        if (side == "Left")
            return new Vector3(1f,0,0);
        else if (side == "Right")
            return new Vector3(-1f,0,0);
        else if (side == "Down")
            return new Vector3(0,1f,0);
        else if (side == "Up")
            return new Vector3(0,-1f,0);
        else
            return new Vector3(0,0,0);
    }

    string DetermineHitSide(Vector2 diff)
    {
        diff.Normalize();
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            return diff.x > 0 ? "Right" : "Left";
        else
            return diff.y > 0 ? "Up" : "Down";
    }

    void playSFX() {
        int index = Random.Range(0, soundList.Length);
        selected = soundList[index];
        audioSource.clip = selected;
        audioSource.PlayOneShot(selected);
    }
}
