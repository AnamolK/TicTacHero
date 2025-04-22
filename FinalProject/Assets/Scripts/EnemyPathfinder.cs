// using System.Collections;
// using UnityEngine;

// public class EnemyPathfinder : MonoBehaviour
// {
//     public float moveTickDuration = 1f;
//     public int maxHealth = 10;
//     [SerializeField] private int currentHealth;

//     public string currentAttackSide = "None";

//     public Transform movePoint;
//     // Radius used to check if the destination cell is occupied.
//     public float occupancyCheckRadius = 0.3f;

//     private Transform player;

//     //animation/asset manager
//     [SerializeField] private GameObject asset;
//     private new AnimationGeneric animTween;
    
//     public bool willDropPotion = false;
//     public GameObject healthPotionPrefab;

//     void Start()
//     {
//         currentHealth = maxHealth;
//         GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
//         if (playerObj != null)
//             player = playerObj.transform;

//         if (movePoint != null)
//             movePoint.position = new Vector3(SnapToGrid(transform.position.x),
//                                              SnapToGrid(transform.position.y),
//                                              transform.position.z);

//         StartCoroutine(MoveTick());
//         animTween = asset.GetComponent<AnimationGeneric>();
//     }

//     IEnumerator MoveTick()
//     {
//         while (true)
//         {
//             yield return new WaitForSeconds(moveTickDuration);

//             // Determine movement direction toward the player.
//             Vector2 direction = GetMoveDirection();
//             if (direction != Vector2.zero)
//             {
//                 // Calculate new destination.
//                 Vector3 currentDestination = movePoint.position;
//                 Vector3 newDestination = currentDestination + new Vector3(direction.x, direction.y, 0);
//                 newDestination = new Vector3(SnapToGrid(newDestination.x), SnapToGrid(newDestination.y), newDestination.z);
                
//                 // Check if the destination cell is occupied by another enemy.
//                 if (!IsCellOccupied(newDestination))
//                 {
//                     movePoint.position = newDestination;
//                     currentAttackSide = GetDirectionString(direction);
//                     Debug.Log("Enemy moving to: " + newDestination + " with active attack side: " + currentAttackSide);

//                     if (!IsCellOccupiedForAnimation(newDestination))
//                     {
//                         animTween.MoveBounce(0.4f);
//                     }
//                 }
//                 else
//                 {
//                     // Try alternative direction if direct move is blocked
//                     Vector2 alternativeDirection = Vector2.zero;
//                     if (direction.x != 0)
//                         alternativeDirection = new Vector2(0, Mathf.Sign(player.position.y - movePoint.position.y));
//                     else if (direction.y != 0)
//                         alternativeDirection = new Vector2(Mathf.Sign(player.position.x - movePoint.position.x), 0);

//                     if (alternativeDirection != Vector2.zero)
//                     {
//                         Vector3 altDestination = currentDestination + new Vector3(alternativeDirection.x, alternativeDirection.y, 0);
//                         altDestination = new Vector3(SnapToGrid(altDestination.x), SnapToGrid(altDestination.y), altDestination.z);
//                         if (!IsCellOccupied(altDestination))
//                         {
//                             movePoint.position = altDestination;
//                             currentAttackSide = GetDirectionString(alternativeDirection);
//                             Debug.Log("Enemy moving to: " + altDestination + " with active attack side: " + currentAttackSide);

//                             if (!IsCellOccupiedForAnimation(altDestination))
//                             {
//                                 animTween.MoveBounce(0.4f);
//                             }
//                         }
//                         else
//                         {
//                             Debug.Log("Destination " + newDestination + " and alternative " + altDestination + " are occupied. Not moving.");
//                         }
//                     }
//                     else
//                     {
//                         Debug.Log("Destination " + newDestination + " is occupied. Not moving.");
//                     }
//                 }
//             }
//         }
//     }

//     Vector2 GetMoveDirection()
//     {
//         if (player == null)
//             return Vector2.zero;

//         // Use movePoint's position as the current grid cell.
//         Vector2 pos = movePoint.position;
//         Vector2 playerPos = player.position;
//         Vector2 diff = playerPos - pos;

//         // Move along the dominant axis.
//         if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
//             return new Vector2(Mathf.Sign(diff.x), 0);
//         else if (Mathf.Abs(diff.y) > 0)
//             return new Vector2(0, Mathf.Sign(diff.y));
//         return Vector2.zero;
//     }

//     float SnapToGrid(float value)
//     {
//         // Assumes grid cells are 1 unit in size.
//         return Mathf.Round(value);
//     }

//     // Returns a cardinal direction string based on a normalized vector.
//     string GetDirectionString(Vector2 direction)
//     {
//         direction.Normalize();
//         if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
//             return direction.x > 0 ? "Right" : "Left";
//         else
//             return direction.y > 0 ? "Up" : "Down";
//     }

//     // Checks if the destination cell is occupied by any enemy (excluding self).
//     bool IsCellOccupied(Vector3 destination)
//     {
//         Collider2D[] colliders = Physics2D.OverlapCircleAll(destination, occupancyCheckRadius);
//         foreach (Collider2D col in colliders)
//         {
//             if (col.gameObject != this.gameObject && (col.CompareTag("Enemy") || col.CompareTag("Turret")))
//             {
//                 return true;
//             }
//         }
//         return false;
//     }

//     bool IsCellOccupiedForAnimation(Vector3 destination)
//     {
//         Collider2D[] colliders = Physics2D.OverlapCircleAll(destination, occupancyCheckRadius);
//         foreach (Collider2D col in colliders)
//         {
//             if (col.gameObject != this.gameObject && (col.CompareTag("Enemy") || col.CompareTag("Player")))
//             {
//                 return true;
//             }
//         }
//         return false;
//     }

//     public void TakeDamage(int damage)
//     {
//         currentHealth -= damage;
//         Debug.Log("Enemy took damage. Current health: " + currentHealth);
//         GetComponent<VFX_ColorChange>().GetHit();
//         if (currentHealth <= 0) {
//             Die();
//         } else {
//             animTween.DamageTakenEnemy(0.1f);
//         }
//     }

//     void Die()
//     {
//         Debug.Log("Enemy died.");

//         StopAllCoroutines();
//         this.enabled = false;

//         // Disable all colliders and sprites.
//         Collider2D[] cols = transform.root.GetComponentsInChildren<Collider2D>();
//         foreach (Collider2D col in cols)
//             col.enabled = false;
//         SpriteRenderer[] srs = transform.root.GetComponentsInChildren<SpriteRenderer>();
//         foreach (SpriteRenderer sr in srs)
//             sr.enabled = false;

//         if (movePoint != null)
//         {
//             Destroy(movePoint.gameObject);
//             movePoint = null;
//         }

//         if(willDropPotion && healthPotionPrefab != null)
//             Instantiate(healthPotionPrefab, transform.position, transform.rotation);

//         animTween.DieEnemy(0.3f);
//         // Destroy the enemy's root GameObject.
//         Destroy(transform.root.gameObject, 0.5f);
//     }
// }


using System.Collections;
using UnityEngine;

public enum EnemyType
{
    Normal,
    Dragon,
    SlimeBoss
}

public class EnemyPathfinder : MonoBehaviour
{
    [Header("Shared / Base Settings")]
    public EnemyType enemyType = EnemyType.Normal;
    
    public float moveTickDuration = 1f;
    public int maxHealth = 10;
    [SerializeField] private int currentHealth;

    public string currentAttackSide = "None";

    public Transform movePoint;
    
    public GameObject movePointObj;
    public float occupancyCheckRadius = 0.3f;

    // If you want items to drop
    public bool willDropPotion = false;
    public GameObject healthPotionPrefab;

    // For all enemies: reference the player & do pathfinding
    private Transform player;
    private GameObject playerObj;
    
    // Animation
    [SerializeField] private GameObject asset;
    private AnimationGeneric animTween;

    // Dragon
    [Header("Dragon Settings")]
    public GameObject fireHitbox;
    public float dragonFireRange = 1.5f; // Distance to start telegraphing
    public int dragonFireTickSkip = 2;
    private int dragonFireTickCount = 0;
    public float dragonTelegraphDuration = 1f; // Pause time
    public GameObject dragonFirePrefab;
    private bool isAttackingDragon = false; // track if the dragon is mid-telegraph

    // Slimeboss
    [Header("SlimeBoss Settings")]
    public float jumpRange = 2f; // how far it can jump
    public int jumpTickInterval = 6; // jump every X ticks
    public int jumpDamage = 3; // damage if it lands on player
    public GameObject slimeMinionPrefab;
    public int minionsToSpawn = 2;
    private bool isAttackingSlime = false; // track if slime is mid-jump

    // track ticks
    private int tickCounter = 0;
    private bool isStunned = false;


    void Start()
    {
        currentHealth = maxHealth;

        // Find player
        playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        // Snap to grid at start
        if (movePoint != null && enemyType == EnemyType.Normal)
        {
            movePoint.position = new Vector3(
                transform.position.x,
                transform.position.y,
                transform.position.z
            );
        } else {
            movePoint.position = new Vector3(
                SnapToGrid(transform.position.x) + 0.5f,
                SnapToGrid(transform.position.y) - 0.5f,
                transform.position.z
            );
        }

        // animation
        animTween = asset.GetComponent<AnimationGeneric>();
        // Start the main routine
        StartCoroutine(MoveTick());
    }


    IEnumerator MoveTick()
    {
        while (true)
        {
            yield return new WaitForSeconds(moveTickDuration);

            if (isStunned)
            {
                Debug.Log($"{gameObject.name} is stunned. Skipping this tick.");
                continue;
            }

            tickCounter++;

            // Movement
            if (enemyType == EnemyType.Normal)
            {
                // Normal enemy just moves toward player each tick
                MoveOneStepTowardPlayer(0);
            }
            else if (enemyType == EnemyType.Dragon)
            {
                MoveOneStepTowardPlayer(1);
                if (!isAttackingDragon)
                {
                    // Check if we want to do a Fire Attack
                    if (PlayerInDragonRange() && dragonFireTickCount >= dragonFireTickSkip)
                    {
                        StartCoroutine(DragonFireSequence());
                        dragonFireTickCount = 0;
                    } else {
                        dragonFireTickCount++;
                    }
                }
            }
            else if (enemyType == EnemyType.SlimeBoss)
            {
                MoveOneStepTowardPlayer(2);

                if (!isAttackingSlime && tickCounter % jumpTickInterval == 0)
                {
                    StartCoroutine(SlimeBossJumpAttack());
                }
            }
        }
    }

    // Pathfinding
    private void MoveOneStepTowardPlayer(int enemyInd)
    {
        if (player == null) return;

        Vector2 direction = GetMoveDirection();
        if (direction == Vector2.zero) return;

        Vector3 currentDestination = movePoint.position;
        Vector3 newDestination = currentDestination + new Vector3(direction.x, direction.y, 0);
        newDestination = new Vector3(
            // SnapToGrid(newDestination.x),
            // SnapToGrid(newDestination.y),
            // newDestination.z;
            newDestination.x,
            newDestination.y,
            newDestination.z
        );

        if (!IsCellOccupied(newDestination))
        {
            movePoint.position = newDestination;
            currentAttackSide = GetDirectionString(direction);

            if (!IsCellOccupiedForAnimation(newDestination))
            {
                if (enemyInd == 0) {
                    animTween.MoveBounce(0.4f);
                } else if (enemyInd == 1) {
                    animTween.MoveDragon(0.4f);
                }
            }
        }
        // else
        // {
        //     // fallback logic if blocked
        //     Vector2 alt = Vector2.zero;
        //     if (direction.x != 0)
        //         alt = new Vector2(0, Mathf.Sign(player.position.y - movePoint.position.y));
        //     else if (direction.y != 0)
        //         alt = new Vector2(Mathf.Sign(player.position.x - movePoint.position.x), 0);

        //     if (alt != Vector2.zero)
        //     {
        //         Vector3 altDest = currentDestination + new Vector3(alt.x, alt.y, 0);
        //         altDest = new Vector3(
        //             // SnapToGrid(altDest.x),
        //             // SnapToGrid(altDest.y),
        //             // altDest.z
        //             altDest.x,
        //             altDest.y,
        //             altDest.z
        //         );
        //         if (!IsCellOccupied(altDest))
        //         {
        //             movePoint.position = altDest;
        //             currentAttackSide = GetDirectionString(alt);
        //             if (!IsCellOccupiedForAnimation(altDest))
        //             {
        //                 if (enemyInd == 0) {
        //                     animTween.MoveBounce(0.4f);
        //                 } else if (enemyInd == 1) {
        //                     animTween.MoveDragon(0.4f);
        //                 }
        //             }
        //         }
        //     }
        // }
    }

    private Vector2 GetMoveDirection()
    {
        if (player == null) return Vector2.zero;
        Vector2 pos = movePoint.position;
        Vector2 diff = (Vector2)player.position - pos;

        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            return new Vector2(Mathf.Sign(diff.x), 0);
        else if (Mathf.Abs(diff.y) != 0)
            return new Vector2(0, Mathf.Sign(diff.y));
        return Vector2.zero;
    }

    private float SnapToGrid(float value)
    {
        return Mathf.Round(value);
    }

    bool IsCellOccupied(Vector3 destination)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(destination, occupancyCheckRadius);
        foreach (Collider2D col in colliders)
        {
            Collider2D collider = playerObj.GetComponent<Collider2D>();
            if (collider.IsTouching(gameObject.GetComponent<Collider2D>())){
                return true;
            }
        }
        return false;
    }

    bool IsCellOccupiedForAnimation(Vector3 destination)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(destination, occupancyCheckRadius);
        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    private string GetDirectionString(Vector2 direction)
    {
        direction.Normalize();
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            return direction.x > 0 ? "Right" : "Left";
        else
            return direction.y > 0 ? "Up" : "Down";
    }

    // Dragon fire
    private bool PlayerInDragonRange()
    {
        if (player == null) return false;
        float dist = Vector2.Distance(movePoint.position, player.position);
        return (dist <= dragonFireRange);
    }
    
    public void Stun(float duration)
    {
        if (!isStunned) {
            StartCoroutine(StunRoutine(duration));
        }
    }

    private IEnumerator DragonFireSequence()
    {
        isAttackingDragon = true;

        // Telegraphed wait
        yield return new WaitForSeconds(dragonTelegraphDuration);
        yield return new WaitForSeconds(moveTickDuration);
        tickCounter++;

        // Spew fire
        if (player != null && dragonFirePrefab != null)
        {
            Vector2 toPlayer = (player.position - transform.position).normalized;
            animTween.AttackMelee(toPlayer * 0.5f, 0.3f);

            fireHitbox.tag = "AOE";
            fireHitbox.transform.GetChild(0).gameObject.SetActive(true);

            // GameObject fireObj = Instantiate(dragonFirePrefab, transform.position, Quaternion.identity);
            // // If you have FireProjectile, do e.g. fireObj.GetComponent<FireProjectile>()?.Init(toPlayer);
        }

        yield return new WaitForSeconds(moveTickDuration);
        fireHitbox.tag = "Untagged";
        fireHitbox.transform.GetChild(0).gameObject.SetActive(false);
        isAttackingDragon = false;
        
    }

    // Slimeboss jump attack
    private IEnumerator SlimeBossJumpAttack()
    {
        isAttackingSlime = true;

        //Telegraphing
        animTween.AttackMelee(new Vector3(0, 0.2f, 0), 0.5f);
        yield return new WaitForSeconds(0.5f);

        //Compute the jump target toward the player
        Vector3 oldPos = movePoint.position;
        Vector3 playerGrid = new Vector3(
            SnapToGrid(player.position.x),
            SnapToGrid(player.position.y),
            oldPos.z
        );
        Vector3 toPlayer = playerGrid - oldPos;
        float jumpDist = Mathf.Min(toPlayer.magnitude, jumpRange);
        Vector3 newPos = oldPos + toPlayer.normalized * jumpDist;
        newPos.x = SnapToGrid(newPos.x);
        newPos.y = SnapToGrid(newPos.y);

        // Telling the MovementController to go there
        movePoint.position = newPos;

        // Waiting here until the boss has actually moved there
        while (Vector2.Distance(transform.position, newPos) > 0.05f)
            yield return null;

        // Since it landed we do the damage
        if (Vector2.Distance(transform.position, player.position) < 0.75f)
        {
            Debug.Log("SlimeBoss lands and hits for " + jumpDamage);
            var ph = player.GetComponent<PlayerHealth>();
            if (ph != null) ph.TakeDamage(jumpDamage);
        }

        // Spawning the minions
        for (int i = 0; i < minionsToSpawn; i++)
        {
            Vector3 spawnPos = newPos + new Vector3(
                Random.Range(-1, 2),
                Random.Range(-1, 2),
                0
            );
            Instantiate(slimeMinionPrefab, spawnPos, Quaternion.identity);
        }

        isAttackingSlime = false;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Enemy ({enemyType}) took {damage} damage. Current health: {currentHealth}");

        GetComponent<VFX_ColorChange>()?.GetHit();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animTween.DamageTakenEnemy(0.1f);
        }
    }

    private IEnumerator StunRoutine(float duration)
    {
        isStunned = true;
        Debug.Log($"{gameObject.name} stunned for {duration} seconds!");

        // Optional: Visual feedback
        // GetComponent<SpriteRenderer>().color = Color.cyan;

        yield return new WaitForSeconds(duration);

        isStunned = false;
        Debug.Log($"{gameObject.name} recovered from stun.");

        // Optional: Reset visuals
        // GetComponent<SpriteRenderer>().color = Color.white;
    }


    private void Die()
    {
        Debug.Log($"Enemy ({enemyType}) died.");
        StopAllCoroutines();
        this.enabled = false;

        // disable colliders & visuals
        Collider2D[] cols = transform.root.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in cols)
            col.enabled = false;

        SpriteRenderer[] srs = transform.root.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in srs)
            sr.enabled = false;

        if (movePoint != null)
        {
            Destroy(movePoint.gameObject);
            movePoint = null;
        }

        if (willDropPotion && healthPotionPrefab != null) {
            Instantiate(healthPotionPrefab, transform.position+new Vector3(-1,1,0), transform.rotation);
        }
        animTween.DieEnemy(0.3f);

        Destroy(transform.root.gameObject, 0.5f);
    }
}