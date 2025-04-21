using System.Collections;
using UnityEngine;

public class DragonPathfinder : MonoBehaviour
{
   [Header("Dragon Stats")]
    public float moveTickDuration = 1f; // Movement frequency
    public int maxHealth = 30;         // More HP for a boss
    private int currentHealth;

    [Header("Movement")]
    public Transform movePoint;           // Snap-to-grid target
    public float occupancyCheckRadius = 0.3f;

    private Transform player;             // Player reference

   //animation/asset manager
    [SerializeField] private GameObject asset;
    private new AnimationGeneric animTween;

    // Dragon-specific attack fields
    [Header("Dragon Attack Settings")]
    public float fireRange = 2f;          // Distance to player to trigger the breath
    public float telegraphDuration = 1f;  // Time spent telegraphing/winding up
    public GameObject firePrefab;         // The projectile/effect for the fire
    private bool isAttacking = false;     // Tracks if the dragon is in the middle of a telegraph/attack

    // If you want to keep the “attack side” logic from your original script:
    public string currentAttackSide = "None";

 // If you want to optionally drop items on death
    public bool willDropPotion = false;
    public GameObject healthPotionPrefab;
    private bool isStunned = false;

    void Start()
    {
        currentHealth = maxHealth;

        // Find Player by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        // Snap movePoint to grid at the start, if needed
        if (movePoint != null)
        {
            movePoint.position = new Vector3(
                SnapToGrid(transform.position.x),
                SnapToGrid(transform.position.y),
                transform.position.z
            );
        }

        // Start the movement/attack loop
        StartCoroutine(MoveTick());

        // Grab the AnimationGeneric component from the 'asset' child object
        animTween = asset.GetComponent<AnimationGeneric>();
    }

    IEnumerator MoveTick()
    {
        while (true)
        {
            // Wait for the designated “tick” duration
            yield return new WaitForSeconds(moveTickDuration);
            if (isStunned)
            {
                Debug.Log($"{gameObject.name} is stunned. Skipping tick.");
                continue;
            }

            // If the dragon isn't attacking, try to move
            if (!isAttacking)
                MoveTowardPlayer();

            // Check if within fire range to start telegraphing
            if (!isAttacking && player != null)
            {
                float distToPlayer = Vector2.Distance(movePoint.position, player.position);
                if (distToPlayer <= fireRange)
                {
                    StartCoroutine(FireAttackSequence());
                }
            }
        }
    }

    private IEnumerator FireAttackSequence()
    {
        isAttacking = true;

        // Optional telegraph animation
        Debug.Log("Dragon telegraphs attack...");
        // E.g.: animTween.AttackMelee(...) or a custom method

        // Wait for the telegraph/wind-up duration
        GetComponent<VFX_ColorChange>().AnticipateAttack();
        yield return new WaitForSeconds(telegraphDuration);
        

        // Actually breathe fire
        if (player != null && firePrefab != null)
        {
            Vector2 directionToPlayer = (player.position - transform.position).normalized;

            // Instantiate the fire prefab
            GameObject fireObj = Instantiate(firePrefab, transform.position, Quaternion.identity);

            // Access the FireProjectile script
            FireProjectile fireScript = fireObj.GetComponent<FireProjectile>();
            if (fireScript != null)
                fireScript.Init(directionToPlayer);
        }


        // Small delay after firing
        yield return new WaitForSeconds(0.3f);

        isAttacking = false;
    }

    private void MoveTowardPlayer()
    {
        if (player == null)
            return;

        Vector2 direction = GetMoveDirection();
        if (direction != Vector2.zero)
        {
            Vector3 currentDestination = movePoint.position;
            Vector3 newDestination = currentDestination + new Vector3(direction.x, direction.y, 0);
            newDestination = new Vector3(
                SnapToGrid(newDestination.x),
                SnapToGrid(newDestination.y),
                newDestination.z
            );

            // If the cell is free, move
            if (!IsCellOccupied(newDestination))
            {
                movePoint.position = newDestination;
                currentAttackSide = GetDirectionString(direction);

                Debug.Log("Dragon moving to: " + newDestination);

                // Optional bounce animation
                if (!IsCellOccupiedForAnimation(newDestination) && animTween != null)
                {
                    animTween.MoveBounce(0.4f);
                }
            }
            else
            {
                Debug.Log("Destination blocked, not moving.");
            }
        }
    }

    private Vector2 GetMoveDirection()
    {
        if (player == null) return Vector2.zero;
        Vector2 currentPos = movePoint.position;
        Vector2 diff = (Vector2)player.position - currentPos;

        // Move along the dominant axis
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            return new Vector2(Mathf.Sign(diff.x), 0);
        else if (Mathf.Abs(diff.y) != 0)
            return new Vector2(0, Mathf.Sign(diff.y));

        return Vector2.zero;
    }

    // Round to integer coordinates for a simple grid
    private float SnapToGrid(float value)
    {
        return Mathf.Round(value);
    }

    private bool IsCellOccupied(Vector3 destination)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(destination, occupancyCheckRadius);
        foreach (Collider2D col in colliders)
        {
            // if it's an enemy or turret, block the cell
            if (col.gameObject != this.gameObject &&
                (col.CompareTag("Enemy") || col.CompareTag("Turret")))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsCellOccupiedForAnimation(Vector3 destination)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(destination, occupancyCheckRadius);
        foreach (Collider2D col in colliders)
        {
            // if it's a player or enemy, don't bounce
            if (col.gameObject != this.gameObject &&
                (col.CompareTag("Enemy") || col.CompareTag("Player")))
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

    public void Stun(float duration)
    {
        if (!isStunned)
        {
            StartCoroutine(StunRoutine(duration));
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


// Same damage logic from your original pathfinder
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Dragon took damage. Current health: " + currentHealth);
        GetComponent<VFX_ColorChange>().GetHit();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            if (animTween != null)
                animTween.DamageTakenEnemy(0.1f);
        }
    }

    private void Die()
    {
        Debug.Log("Dragon died.");

        StopAllCoroutines(); // does this need to send a message to pathfinder?
        this.enabled = false;

        // Disable collisions and visuals
        Collider2D[] cols = transform.root.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in cols)
            col.enabled = false;
        SpriteRenderer[] srs = transform.root.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in srs)
            sr.enabled = false;

        // If there's a movePoint child, destroy it
        if (movePoint != null)
        {
            Destroy(movePoint.gameObject);
            movePoint = null;
        }

        // Optionally drop potion
        if (willDropPotion && healthPotionPrefab != null)
            Instantiate(healthPotionPrefab, transform.position, transform.rotation);

        // Play death animation
        if (animTween != null)
            animTween.DieEnemy(0.3f);

        // Destroy the dragon object after a short delay
        Destroy(transform.root.gameObject, 0.5f);
    }


    
}
