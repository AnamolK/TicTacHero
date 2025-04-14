using System.Collections;
using UnityEngine;

public class SlimeBossPathfinder : MonoBehaviour
{
    [Header("Movement Tick Settings")]
    public float moveTickDuration = 1f;       // Time between each "tick"
    public float occupancyCheckRadius = 0.3f; // For checking blocked cells
    public Transform movePoint;              // Child for snapping to grid

    [Header("Boss Stats")]
    public int maxHealth = 20;
    private int currentHealth;
    private bool isDead = false;

    // If you want a smaller or bigger clamp area, or no clamp at all, adjust here
    [Header("Arena/Clamping")]
    public bool clampToUpperArea = false;
    public float upperArenaY = 2f;

    [Header("Attack Settings")]
    // Normal pathfinder "melee attack" example:
    public float meleeRange = 1.1f; 
    public int meleeDamage = 1; 
    public int meleeTickInterval = 2; // e.g., every 2 ticks, attempt a melee if close

    // Jump every 6 ticks
    public int jumpTickInterval = 6; 
    public float jumpRange = 3f; 
    public int jumpDamage = 2;
    public int minionsToSpawn = 2;         // if you want to spawn small slimes
    public GameObject slimeMinionPrefab;   // smaller slime prefab

    [Header("Animation / Visuals")]
    [SerializeField] private GameObject asset;  // Child that has AnimationGeneric
    private AnimationGeneric animGeneric;

    private Transform player;
    private bool isAttacking = false;  
    private int tickCounter = 0;       // increments each tick

    void Start()
    {
        currentHealth = maxHealth;

        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        // Snap the movePoint to grid at start
        if (movePoint != null)
        {
            movePoint.position = new Vector3(
                SnapToGrid(transform.position.x),
                SnapToGrid(transform.position.y),
                transform.position.z
            );
        }

        // Grab the AnimationGeneric
        animGeneric = asset.GetComponent<AnimationGeneric>();

        // Start the tick-based behavior
        StartCoroutine(SlimeBossRoutine());
    }

    private IEnumerator SlimeBossRoutine()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(moveTickDuration);
            tickCounter++;

            // 1) If not currently jumping/attacking, do normal pathfinder movement
            if (!isAttacking)
            {
                MoveTowardPlayerOneCell();
            }

            // 2) Attempt Melee every X ticks
            if (!isAttacking && tickCounter % meleeTickInterval == 0)
            {
                AttemptMeleeIfClose();
            }

            // 3) Jump Attack every 6 ticks
            if (!isAttacking && tickCounter % jumpTickInterval == 0)
            {
                StartCoroutine(DoJumpAttack());
            }

            // (Optional) clamp to top area
            if (clampToUpperArea && transform.position.y < upperArenaY)
            {
                // forcibly keep y above upperArenaY
                if (movePoint.position.y < upperArenaY)
                {
                    Vector3 mp = movePoint.position;
                    mp.y = upperArenaY;
                    movePoint.position = mp;
                }
            }

            // You can do a small bounce if idle
            if (!isAttacking && animGeneric != null)
            {
                animGeneric.MoveBounce(0.3f);
            }
        }
    }

    // ============= Normal Movement (like EnemyPathfinder) =============
    private void MoveTowardPlayerOneCell()
    {
        if (player == null || movePoint == null) return;

        // Figure out direction (dominant axis)
        Vector2 direction = GetMoveDirection();
        if (direction == Vector2.zero) return; // no movement if aligned with player

        Vector3 currentDest = movePoint.position;
        Vector3 newDest = currentDest + new Vector3(direction.x, direction.y, 0);
        newDest = new Vector3(
            SnapToGrid(newDest.x),
            SnapToGrid(newDest.y),
            newDest.z
        );

        // Check if blocked
        if (!IsCellOccupied(newDest))
        {
            movePoint.position = newDest;
        }
    }

    private Vector2 GetMoveDirection()
    {
        if (player == null) return Vector2.zero;

        Vector2 diff = player.position - movePoint.position;
        // Move along the dominant axis
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            return new Vector2(Mathf.Sign(diff.x), 0);
        else if (Mathf.Abs(diff.y) != 0)
            return new Vector2(0, Mathf.Sign(diff.y));
        return Vector2.zero;
    }

    private bool IsCellOccupied(Vector3 destination)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(destination, occupancyCheckRadius);
        foreach (Collider2D col in colliders)
        {
            if (col.gameObject != gameObject && (col.CompareTag("Enemy") || col.CompareTag("Turret")))
            {
                return true;
            }
        }
        return false;
    }

    private float SnapToGrid(float val)
    {
        return Mathf.Round(val);
    }

    // ============= Melee Attack =============
    private void AttemptMeleeIfClose()
    {
        if (player == null) return;
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= meleeRange)
        {
            // meelee hits
            Debug.Log("SlimeBoss: Melee hits player for " + meleeDamage + " damage");
            DealDamageToPlayer(meleeDamage);

            if (animGeneric != null)
                animGeneric.AttackMelee(Vector3.zero, 0.2f);
        }
    }

    // ============= Jump Attack =============
    private IEnumerator DoJumpAttack()
    {
        isAttacking = true;
        Debug.Log("SlimeBoss: Jump telegraph starts...");

        // small telegraph animation
        if (animGeneric != null)
            animGeneric.AttackMelee(new Vector3(0, 0.2f, 0), 0.5f);

        yield return new WaitForSeconds(0.5f);

        // pick new random spot
        Vector3 oldPos = movePoint.position;
        Vector3 newPos = oldPos + new Vector3(
            Random.Range(-jumpRange, jumpRange),
            Random.Range(-jumpRange, jumpRange),
            0
        );
        // snap if needed
        newPos.x = SnapToGrid(newPos.x);
        newPos.y = SnapToGrid(newPos.y);

        // movePoint = newPos
        if (clampToUpperArea && newPos.y < upperArenaY)
            newPos.y = upperArenaY;

        movePoint.position = newPos;

        // check if we land on the player
        if (player != null)
        {
            float dist = Vector2.Distance(newPos, player.position);
            if (dist < 0.75f)
            {
                Debug.Log("SlimeBoss: Landed on the player, dealing " + jumpDamage + " damage");
                DealDamageToPlayer(jumpDamage);
            }
        }

        // spawn minions
        for (int i = 0; i < minionsToSpawn; i++)
        {
            if (slimeMinionPrefab != null)
            {
                Vector3 spawnPos = newPos + new Vector3(
                    Random.Range(-1, 2),
                    Random.Range(-1, 2),
                    0
                );
                Instantiate(slimeMinionPrefab, spawnPos, Quaternion.identity);
            }
        }

        isAttacking = false;
    }

    private void DealDamageToPlayer(int dmg)
    {
        if (player == null) return;
        PlayerHealth ph = player.GetComponent<PlayerHealth>();
        if (ph != null)
            ph.TakeDamage(dmg);
    }

    // ============= TakeDamage/Die =============
    public void TakeDamage(int dmg)
    {
        if (isDead) return;
        currentHealth -= dmg;
        Debug.Log("SlimeBoss took " + dmg + " damage. Current HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            if (animGeneric != null)
                animGeneric.DamageTakenEnemy(0.1f);
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("SlimeBoss died.");
        StopAllCoroutines();
        this.enabled = false;

        if (animGeneric != null)
            animGeneric.DieEnemy(0.3f);

        // disable colliders, visuals
        Collider2D[] cols = GetComponentsInChildren<Collider2D>();
        foreach (var c in cols) c.enabled = false;
        SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>();
        foreach (var sr in srs) sr.enabled = false;

        if (movePoint != null)
            Destroy(movePoint.gameObject);

        Destroy(gameObject, 0.5f);
    }
}
