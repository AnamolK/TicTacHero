using System.Collections;
using UnityEngine;

public class EnemyPathfinder : MonoBehaviour
{
    public float moveTickDuration = 1f;
    public int maxHealth = 10;
    private int currentHealth;


    public string currentAttackSide = "None";


    public Transform movePoint;

    private Transform player;

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
                // Update movePoint’s position by one grid cell.
                Vector3 currentDestination = movePoint.position;
                Vector3 newDestination = currentDestination + new Vector3(direction.x, direction.y, 0);
                newDestination = new Vector3(SnapToGrid(newDestination.x), SnapToGrid(newDestination.y), newDestination.z);
                movePoint.position = newDestination;

                // Set the enemy’s active attack side to match its movement direction.
                currentAttackSide = GetDirectionString(direction);

                Debug.Log("Enemy moving to: " + newDestination + " with active attack side: " + currentAttackSide);
            }
        }
    }

    Vector2 GetMoveDirection()
    {
        if (player == null)
            return Vector2.zero;

        // Use movePoint’s position as the current grid cell.
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
        // Assumes grid cells of size 1.
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

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Enemy took damage. Current health: " + currentHealth);
        if (currentHealth <= 0)
            Die();
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

        // Finally, destroy the enemy root.
        Destroy(transform.root.gameObject, 0.1f);
    }
}
