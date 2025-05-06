using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public enum EnemyType
{
    Normal,
    Dragon,
    SlimeBoss, 
    DragonBoss,
    Claw
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
    public GameObject fire;
    public float dragonFireRange = 1.5f; // Distance to start telegraphing
    public int dragonFireTickSkip = 3;
    private int dragonFireTickCount = 0;
    public float dragonTelegraphDuration = 1.5f; // Pause time
    private bool isAttackingDragon = false; // track if the dragon is mid-telegraph
    private bool movingUp = false;
    private Vector3 originalSpawn;
    public GameObject leftClaw;
    public GameObject rightClaw;
    public GameObject batMinionPrefab;

    // Slimeboss
    [Header("SlimeBoss Settings")]
    public float jumpRange = 2f; // how far it can jump
    public int spawnTickInterval = 12; // jump every X ticks
    public int JumpTickInterval = 2; // jump every X ticks
    public int jumpDamage = 3; // damage if it lands on player
    public GameObject slimeMinionPrefab;
    public int minionsToSpawn = 2;
    private bool isAttackingSlime = false; // track if slime is mid-jump

    // track ticks
    private int tickCounter = 6;
    private bool isStunned = false;


    void Start()
    {
        currentHealth = maxHealth;
        originalSpawn = transform.position;

        // Find player
        playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        // Snap to grid at start
        if (movePoint != null && enemyType != EnemyType.Dragon)
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
                if (!isAttackingDragon)
                {
                    MoveOneStepTowardPlayer(1);
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
                if (!isAttackingSlime && tickCounter % spawnTickInterval == 0)
                {
                    StartCoroutine(slimeSpawnAttack());
                } else {
                    isAttackingSlime = true;
                    movePointObj.tag = "Untagged";
                    gameObject.GetComponent<Collider2D>().enabled = false;
                    MoveOneStepTowardPlayer(2);
                    StartCoroutine(jumpHitbox());
                }
            } else if (enemyType == EnemyType.DragonBoss) {

                if (!isAttackingDragon)
                {
                    if (movingUp == false) {
                        movePoint.position = gameObject.transform.position + new Vector3(0, -1, 0);
                    } else {
                        movePoint.position = gameObject.transform.position + new Vector3(0, 1, 0);
                    }

                    if (gameObject.transform.position.y >= originalSpawn.y && movingUp) {
                        movingUp = false;
                    } else if (gameObject.transform.position.y <= originalSpawn.y - 2 && movingUp == false) {
                        movingUp = true;
                    }
                    // Check if we want to do a Fire Attack
                    if (PlayerInDragonRange() && dragonFireTickCount >= dragonFireTickSkip)
                    {
                        StartCoroutine(DragonBOSSFireSequence());
                        dragonFireTickCount = 0;
                    } else {
                        dragonFireTickCount++;
                    }
                }
            } else if (enemyType == EnemyType.Claw) {

                if (movingUp == false) {
                        isAttackingSlime = true;
                        movePointObj.tag = "Untagged";
                        gameObject.GetComponent<Collider2D>().enabled = false;
                        MoveOneStepTowardPlayer(4);
                        StartCoroutine(jumpHitbox());
                    } else {
                        movePoint.position = originalSpawn;
                        movingUp = false;
                        Debug.Log("Claw Reset");
                        if (tickCounter > 16 && tickCounter % spawnTickInterval == 0) {
                            StartCoroutine(dragonSpawnAttack());
                        }
                        
                    }

                    if (gameObject.transform.position.y <= originalSpawn.y - 1f) {
                        movingUp = true;
                    }                    
            }
        }
    }

    private IEnumerator jumpHitbox() {
        yield return new WaitForSeconds(2.5f);
        movePointObj.tag = "Enemy";
        gameObject.GetComponent<Collider2D>().enabled = true;
        isAttackingSlime = false;
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
                } else if (enemyInd == 2) {
                    animTween.MoveSlimeBoss(2.5f, 4f);
                }  else if (enemyInd == 4) {
                    animTween.MoveSlimeBoss(2.5f, 2f);
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
        //                 } else if (enemyInd == 2) {

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
            Collider2D collider = col.GetComponent<Collider2D>();
            if (collider.tag == "Enemy") {
                if (collider.gameObject != gameObject && collider.gameObject != movePointObj) {                
                    return true;
                }
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
        if (player != null)
        {
            Vector2 toPlayer = (player.position - transform.position).normalized;
            animTween.AttackMelee(toPlayer * 0.5f, 0.4f);

            fire.tag = "AOE";
            fire.transform.GetChild(0).gameObject.SetActive(true);
            AnimationGeneric fireTween = fire.GetComponent<AnimationGeneric>();
            fireTween.fireHitboxSolverSmall(0.2f);
        }

        yield return new WaitForSeconds(moveTickDuration);
        fire.tag = "Untagged";
        fire.transform.GetChild(0).gameObject.SetActive(false);
        isAttackingDragon = false;
        
    }

    private IEnumerator DragonBOSSFireSequence()
    {
        isAttackingDragon = true;
        // Telegraphed wait
        fire.transform.GetChild(1).gameObject.SetActive(true);
        yield return new WaitForSeconds(dragonTelegraphDuration);

        fire.tag = "AOE";
        fire.transform.GetChild(1).gameObject.SetActive(false);
        fire.transform.GetChild(0).gameObject.SetActive(true);
        AnimationGeneric fireTween = fire.GetComponent<AnimationGeneric>();
        fireTween.fireHitboxSolver(0.1f);
        

        yield return new WaitForSeconds(moveTickDuration);
        fire.tag = "Untagged";
        fire.transform.GetChild(0).gameObject.SetActive(false);
        isAttackingDragon = false;
        
    }

    private IEnumerator dragonSpawnAttack()
    {
        isAttackingSlime = true;

        //Telegraphing
        animTween.AttackBatSpawn(0.5f);
        yield return new WaitForSeconds(2f);

        Vector3 oldPos = movePoint.position;
  
        // Spawning the minion
        Vector3 spawnPos = new Vector3(
            SnapToGrid(Random.Range(-2, 2) + oldPos.x),
            SnapToGrid(Random.Range(-2, 0) + oldPos.y),
            0
        );
        Instantiate(batMinionPrefab, spawnPos, Quaternion.identity);
        isAttackingSlime = false;
    }

    // Slimeboss spawn attack
    private IEnumerator slimeSpawnAttack()
    {
        isAttackingSlime = true;

        //Telegraphing
        animTween.AttackSlimeSpawn(2f);
        yield return new WaitForSeconds(2f);

        //Compute the jump target toward the player
        Vector3 oldPos = movePoint.position;
  
        // Spawning the minions
        for (int i = 0; i < minionsToSpawn; i++)
        {
            Vector3 spawnPos = oldPos + new Vector3(
                Random.Range(-3, 3),
                Random.Range(-2, 0),
                0
            );
            Instantiate(slimeMinionPrefab, spawnPos, Quaternion.identity);
        }

        isAttackingSlime = false;
    }

    private IEnumerator slimeJumpAttack()
    {
        isAttackingSlime = true;

        //Telegraphing
        animTween.AttackSlimeSpawn(1.5f);
        yield return new WaitForSeconds(2f);

        //Compute the jump target toward the player
        Vector3 oldPos = movePoint.position;
  
        // Spawning the minions
        for (int i = 0; i < minionsToSpawn; i++)
        {
            Vector3 spawnPos = oldPos + new Vector3(
                Random.Range(-3, 3),
                Random.Range(-2, 0),
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
        if (enemyType == EnemyType.Dragon) {
            if (leftClaw != null) {
                leftClaw.SetActive(false);
            } else if (rightClaw != null) {
                rightClaw.SetActive(false);
            }
        }
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
            Instantiate(healthPotionPrefab, transform.position+new Vector3(-1,-1,0), transform.rotation);
        }
        animTween.DieEnemy(0.3f);

        Destroy(transform.root.gameObject, 0.5f);
    }
}