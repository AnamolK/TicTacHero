using System.Collections; 
using UnityEngine;

public class EnemyPathfinder : MonoBehaviour
{
    public float moveTickDuration = 1f;
    public float moveDuration = 0.05f;
    private Transform player;
    public bool isMoving = false;

    // Health settings
    public int maxHealth = 3;
    private int currentHealth;

    // Dynamically updated attack side.
    public string currentAttackSide = "None";

    void Start()
    {
        currentHealth = maxHealth;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        transform.position = new Vector3(SnapToGrid(transform.position.x), SnapToGrid(transform.position.y), transform.position.z);
        StartCoroutine(MoveTick());
    }

    IEnumerator MoveTick()
    {
        while (true)
        {
            yield return new WaitForSeconds(moveTickDuration);
            if (!isMoving)
            {
                Vector2 direction = GetMoveDirection();
                if (direction != Vector2.zero)
                {
                    // Update the active attack side dynamically based on movement direction.
                    currentAttackSide = GetDirectionString(direction);
                    StartCoroutine(Move(direction));
                }
            }
        }
    }

    Vector2 GetMoveDirection()
    {
        if (player == null)
            return Vector2.zero;
        Vector2 pos = transform.position;
        Vector2 playerPos = player.position;
        Vector2 diff = playerPos - pos;
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            return new Vector2(Mathf.Sign(diff.x), 0);
        else if (Mathf.Abs(diff.y) > 0)
            return new Vector2(0, Mathf.Sign(diff.y));
        return Vector2.zero;
    }

    IEnumerator Move(Vector2 direction)
    {
        isMoving = true;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(direction.x, direction.y, 0);
        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = new Vector3(SnapToGrid(endPos.x), SnapToGrid(endPos.y), endPos.z);
        isMoving = false;
    }

    float SnapToGrid(float value)
    {
        return value;
    }

    // Converts movement direction vector to a cardinal direction string.
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

        // Stop all coroutines and disable this script.
        StopAllCoroutines();
        this.enabled = false;


        Collider2D[] cols = transform.root.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in cols)
        {
            col.enabled = false;
        }

        SpriteRenderer[] srs = transform.root.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in srs)
        {
            sr.enabled = false;
        }


        Transform enemyAsset = transform.root.Find("EnemyAsset");
        if(enemyAsset != null)
            enemyAsset.gameObject.SetActive(false);

        Transform enemyAttackside = transform.root.Find("EnemyAttackside");
        if(enemyAttackside != null)
            enemyAttackside.gameObject.SetActive(false);


        EnemyMovementController moveCtrl = GetComponentInParent<EnemyMovementController>();
        if (moveCtrl != null)
            moveCtrl.Die();


        transform.root.gameObject.SetActive(false);

        Destroy(transform.root.gameObject, 0.1f);
    }
}