using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Attackable"))
        {
            // Get the EnemyPathfinder from the parent of the AttackableArea.
            EnemyPathfinder enemyPathfinder = collision.GetComponentInParent<EnemyPathfinder>();
            if (enemyPathfinder != null)
            {
                Vector2 enemyPos = enemyPathfinder.transform.position;
                Vector2 playerPos = transform.position;
                Vector2 diff = playerPos - enemyPos;

                string hitSide = DetermineHitSide(diff);

                if (CheckInputAgainstHitSide(hitSide))
                {
                    // Only register damage if the hit side is not the enemy's active attack side.
                    if (hitSide != enemyPathfinder.currentAttackSide)
                    {
                        Debug.Log("Attack Hit: " + hitSide);
                        enemyPathfinder.TakeDamage(1);
                    }
                    else
                    {
                        Debug.Log("Attack failed: Hit enemy's active attack side.");
                    }
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
}
