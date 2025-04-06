using System.Collections;
using UnityEngine;

public class EnemyPathfinder : MonoBehaviour
{
    public float moveTickDuration = 1f;
    public int maxHealth = 10;
    private int currentHealth;

    public string currentAttackSide = "None";

    public Transform movePoint;
    // Radius used to check if the destination cell is occupied.
    public float occupancyCheckRadius = 0.3f;

    private Transform player;

    //animation/asset manager
    [SerializeField] private GameObject asset;
    private new AnimationGeneric animation;
    
    public bool willDropPotion = false;
    public GameObject healthPotionPrefab;

    void Start()
    {
        currentHealth = maxHealth;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        if (movePoint != null)
            movePoint.position = new Vector3(SnapToGrid(transform.position.x),
                                             SnapToGrid(transform.position.y),
                                             transform.position.z);

        StartCoroutine(MoveTick());
        animation = asset.GetComponent<AnimationGeneric>();
    }

    IEnumerator MoveTick()
    {
        while (true)
        {
            yield return new WaitForSeconds(moveTickDuration);

            // Determine movement direction toward the player.
            Vector2 direction = GetMoveDirection();
            if (direction != Vector2.zero)
            {
                // Calculate new destination.
                Vector3 currentDestination = movePoint.position;
                Vector3 newDestination = currentDestination + new Vector3(direction.x, direction.y, 0);
                newDestination = new Vector3(SnapToGrid(newDestination.x), SnapToGrid(newDestination.y), newDestination.z);

                // Check if the destination cell is occupied by another enemy.
                if (!IsCellOccupied(newDestination))
                {
                    movePoint.position = newDestination;
                    currentAttackSide = GetDirectionString(direction);
                    Debug.Log("Enemy moving to: " + newDestination + " with active attack side: " + currentAttackSide);

                    if (!IsCellOccupiedForAnimation(newDestination))
                    {
                        animation.MoveBounce(0.4f);
                    }
                }
                else
                {
                    Debug.Log("Destination " + newDestination + " is occupied. Not moving.");
                }
            }
        }
    }

    Vector2 GetMoveDirection()
    {
        if (player == null)
            return Vector2.zero;

        // Use movePoint's position as the current grid cell.
        Vector2 pos = movePoint.position;
        Vector2 playerPos = player.position;
        Vector2 diff = playerPos - pos;

        // Move along the dominant axis.
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            return new Vector2(Mathf.Sign(diff.x), 0);
        else if (Mathf.Abs(diff.y) > 0)
            return new Vector2(0, Mathf.Sign(diff.y));
        return Vector2.zero;
    }

    float SnapToGrid(float value)
    {
        // Assumes grid cells are 1 unit in size.
        return Mathf.Round(value);
    }

    // Returns a cardinal direction string based on a normalized vector.
    string GetDirectionString(Vector2 direction)
    {
        direction.Normalize();
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            return direction.x > 0 ? "Right" : "Left";
        else
            return direction.y > 0 ? "Up" : "Down";
    }

    // Checks if the destination cell is occupied by any enemy (excluding self).
    bool IsCellOccupied(Vector3 destination)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(destination, occupancyCheckRadius);
        foreach (Collider2D col in colliders)
        {
            if (col.gameObject != this.gameObject && col.CompareTag("Enemy"))
            {
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
            if (col.gameObject != this.gameObject && (col.CompareTag("Enemy") || col.CompareTag("Player")))
            {
                return true;
            }
        }
        return false;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Enemy took damage. Current health: " + currentHealth);
        if (currentHealth <= 0) {
            Die();
        } else {
            animation.DamageTakenEnemy(0.1f);
        }
    }

    void Die()
    {
        Debug.Log("Enemy died.");

        StopAllCoroutines();
        this.enabled = false;

        // Disable all colliders and sprites.
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

        if(willDropPotion && healthPotionPrefab != null)
            Instantiate(healthPotionPrefab, transform.position, transform.rotation);

        animation.DieEnemy(0.3f);
        // Destroy the enemy's root GameObject.
        Destroy(transform.root.gameObject, 0.5f);
    }
}
