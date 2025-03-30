using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    // cooldown
    public float attackCooldown = 0f;
    private float lastAttackTime = -Mathf.Infinity;

    // we call when the player’s collider enters a trigger collider.
    void OnTriggerEnter2D(Collider2D collision)
    {
        // attack cooldown.
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        if (collision.CompareTag("Attackable"))
        {
            EnemyPathfinder enemyPathfinder = collision.GetComponent<EnemyPathfinder>();
            if (enemyPathfinder != null)
            {
                // Compute vector from the enemy's center to the player.
                Vector2 enemyPos = enemyPathfinder.transform.position;
                Vector2 playerPos = transform.position;
                Vector2 diff = playerPos - enemyPos;

                // Determine where collision happened
                string hitSide = DetermineHitSide(diff);
                //Debug.Log("Player hit enemy on side: " + hitSide + " | Enemy current attack side: " + enemyPathfinder.currentAttackSide);

                // Only register the attack if the hit side is not enemy attack side

                if (checkInputAgainstHitside(hitSide)) {
                    if (hitSide != enemyPathfinder.currentAttackSide)
                    {   
                        Debug.Log("Attack Hit: " + hitSide);
                        enemyPathfinder.TakeDamage(1);
                        lastAttackTime = Time.time;
                    }
                    else
                    {
                        Debug.Log("Attack failed: Hit enemy's active attack side.");
                    }
                }
   
            }
        }
    }

    bool checkInputAgainstHitside(string side) {
        if (Input.GetAxisRaw("Horizontal") == 1f && side == "Left") {
            return true;
        } else if (Input.GetAxisRaw("Horizontal") == -1f && side == "Right") {
            return true;
        } else if (Input.GetAxisRaw("Vertical") == 1f && side == "Down") {
            return true;
        } else if (Input.GetAxisRaw("Vertical") == -1f && side == "Up") {
            return true;
        } else {
            return false;
        }
    }

    string DetermineHitSide(Vector2 diff)
    {
        diff.Normalize();
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
        {
            return diff.x > 0 ? "Right" : "Left";
        }
        else
        {
            return diff.y > 0 ? "Up" : "Down";
        }
    }
}