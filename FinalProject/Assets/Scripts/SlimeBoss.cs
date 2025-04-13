using System.Collections;
using UnityEngine;

public class SlimeBoss : MonoBehaviour
{
    [Header("Movement & Position")]
    [SerializeField] private float moveTickDuration = 1f; 
    [SerializeField] private Transform movePoint;
    [SerializeField] private float occupancyCheckRadius = 0.3f;
    [SerializeField] private float upperArenaY = 2f; 
    [SerializeField] private bool clampToUpperArea = true;

    [Header("Boss Stats")]
    [SerializeField] private int maxHealth = 20;

    [Header("Attack Settings")]
    [SerializeField] private int meleeTickInterval = 2; 
    [SerializeField] private float meleeRange = 1.1f; 
    [SerializeField] private int meleeDamage = 1; 
    [SerializeField] private int jumpTickInterval = 6; 
    [SerializeField] private float jumpRange = 2f; 
    [SerializeField] private int jumpDamage = 2;

    [Header("Slime Minions")]
    [SerializeField] private GameObject slimeMinionPrefab;
    [SerializeField] private int minionsToSpawn = 2;

    [Header("Animation / Visuals")]
    [SerializeField] private GameObject asset;
    private AnimationGeneric animGeneric;

    private int currentHealth;
    private bool isDead = false;
    private int tickCounter = 0;
    private bool isAttacking = false;
    private Transform player;

    void Start()
    {
        currentHealth = maxHealth;

        // 1) Find the player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        // 2) Snap movePoint to grid if you're using that system
        if (movePoint != null)
        {
            movePoint.position = new Vector3(
                SnapToGrid(transform.position.x),
                SnapToGrid(transform.position.y),
                transform.position.z
            );
        }

        // 3) Grab AnimationGeneric
        animGeneric = asset.GetComponent<AnimationGeneric>();

        // 4) Start the main boss routine
        StartCoroutine(SlimeBossRoutine());
    }

    private IEnumerator SlimeBossRoutine()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(moveTickDuration);
            tickCounter++;

            // If you want to clamp Y so it stays up top:
            if (clampToUpperArea && transform.position.y < upperArenaY)
            {
                // Optionally also clamp the movePoint
                if (movePoint != null && movePoint.position.y < upperArenaY)
                {
                    movePoint.position = new Vector3(movePoint.position.x, upperArenaY, movePoint.position.z);
                }
            }

            // Every 2 ticks: attempt melee
            if (tickCounter % meleeTickInterval == 0 && !isAttacking)
            {
                AttemptMeleeAttack();
            }

            // Every 6 ticks: do jump
            if (tickCounter % jumpTickInterval == 0 && !isAttacking)
            {
                StartCoroutine(DoJumpAttack());
            }

            // Optionally bounce to show idle "jiggle"
            if (!isAttacking && animGeneric != null)
            {
                animGeneric.MoveBounce(0.3f);
            }
        }
    }

    private void AttemptMeleeAttack()
    {
        if (player == null) return;

        float distToPlayer = Vector2.Distance(transform.position, player.position);
        if (distToPlayer <= meleeRange)
        {
            Debug.Log("King Slime: Melee Attack hits player for " + meleeDamage + " damage.");
            DealDamageToPlayer(meleeDamage);

            if (animGeneric != null)
                animGeneric.AttackMelee(Vector3.zero, 0.2f);
        }
    }

    private IEnumerator DoJumpAttack()
    {
        isAttacking = true;
        Debug.Log("King Slime: Jump telegraph...");

        // Telegraph anim
        if (animGeneric != null)
            animGeneric.AttackMelee(new Vector3(0, 0.2f, 0), 0.5f);

        yield return new WaitForSeconds(0.5f);

        // Calculate a random jump destination
        Vector3 oldPos = transform.position;
        Vector3 newPos = oldPos + RandomJumpOffset();

        // Snap if you're using grid
        newPos.x = SnapToGrid(newPos.x);
        newPos.y = SnapToGrid(newPos.y);

        // IMPORTANT: Only update movePoint, not transform
        if (movePoint != null)
        {
            // If clamp is on and the new jump is below upperArenaY, clamp it
            if (clampToUpperArea && newPos.y < upperArenaY)
                newPos.y = upperArenaY;

            movePoint.position = newPos;
        }
        else
        {
            // If you have no movePoint, you'd have to set transform directly
            transform.position = newPos;
        }

        // If we land on the player (close enough), damage them
        if (player != null)
        {
            float dist = Vector2.Distance(newPos, player.position);
            if (dist < 0.75f)
            {
                Debug.Log("King Slime: Landed on player, dealing " + jumpDamage + " damage.");
                DealDamageToPlayer(jumpDamage);
            }
        }

        // Spawn minions
        for (int i = 0; i < minionsToSpawn; i++)
        {
            Vector3 spawnPos = new Vector3(
                newPos.x + Random.Range(-1, 2),
                newPos.y + Random.Range(-1, 2),
                newPos.z
            );
            if (slimeMinionPrefab != null)
                Instantiate(slimeMinionPrefab, spawnPos, Quaternion.identity);
        }

        isAttacking = false;
    }

    private Vector3 RandomJumpOffset()
    {
        float offsetX = Random.Range(-jumpRange, jumpRange);
        float offsetY = Random.Range(-0.5f, 1.0f); 
        return new Vector3(offsetX, offsetY, 0);
    }

    private void DealDamageToPlayer(int dmg)
    {
        if (player == null) return;
        PlayerHealth ph = player.GetComponent<PlayerHealth>();
        if (ph != null)
            ph.TakeDamage(dmg);
    }

    private float SnapToGrid(float val)
    {
        return Mathf.Round(val);
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;
        currentHealth -= dmg;
        Debug.Log("King Slime took " + dmg + " damage, current HP = " + currentHealth);

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
        if (isDead) return;
        isDead = true;
        Debug.Log("King Slime defeated!");

        StopAllCoroutines();
        this.enabled = false;

        if (animGeneric != null)
            animGeneric.DieEnemy(0.3f);

        // disable colliders, sprite, etc.
        Collider2D[] cols = GetComponentsInChildren<Collider2D>();
        foreach (var col in cols)
            col.enabled = false;
        SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>();
        foreach (var sr in srs)
            sr.enabled = false;

        if (movePoint != null)
            Destroy(movePoint.gameObject);

        Destroy(gameObject, 0.5f);
    }
}
