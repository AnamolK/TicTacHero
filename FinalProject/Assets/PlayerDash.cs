using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    public float dashDistance = 1.5f;
    public float dashCooldown = 5f;
    public LayerMask enemyLayer;

    private bool canDash = true;
    private PlayerStats playerStats;
    private PlayerHealth playerHealth;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (playerStats != null && playerStats.dashUnlocked && Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            AttemptDash();
        }
    }

    void AttemptDash()
    {
        Vector2 dashDir = Vector2.zero;

        if (Input.GetKey(KeyCode.W)) dashDir = Vector2.up;
        else if (Input.GetKey(KeyCode.S)) dashDir = Vector2.down;
        else if (Input.GetKey(KeyCode.A)) dashDir = Vector2.left;
        else if (Input.GetKey(KeyCode.D)) dashDir = Vector2.right;

        if (dashDir == Vector2.zero) return;

        Vector2 dashStart = (Vector2)transform.position;
        RaycastHit2D[] hits = Physics2D.RaycastAll(dashStart, dashDir, dashDistance, enemyLayer);

        if (hits.Length == 1)
        {
            // Dash successful
            Transform enemy = hits[0].transform;
            Vector2 newPosition = (Vector2)enemy.position + dashDir * 0.5f;
            transform.position = newPosition;

            // Deal 2x damage and stun
            EnemyPathfinder e = enemy.GetComponentInParent<EnemyPathfinder>();
            DragonPathfinder d = enemy.GetComponentInParent<DragonPathfinder>();
            int damage = playerStats.currentAttackDamage * 2;

            if (e != null)
            {
                e.TakeDamage(damage);
                e.Stun(1f);
            }
            else if (d != null)
            {
                d.TakeDamage(damage);
                d.Stun(1f);
            }

            Debug.Log("💨Dash success: dealt 2x damage and stunned enemy.");
        }
        else
        {
            // Dash failed
            playerHealth.TakeDamage(1);
            Debug.Log(" Dash failed: hit multiple or no enemies. Took 1 damage.");
        }

        StartCoroutine(DashCooldown());
    }

    System.Collections.IEnumerator DashCooldown()
    {
        canDash = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
