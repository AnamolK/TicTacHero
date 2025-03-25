using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementController : MonoBehaviour
{
    public LayerMask BlockedArea;

    public float moveTickDuration = 0.5f;
    public float moveDuration = 0.1f;
    private Transform player;
    public bool isMoving = false;
    public bool canMove = true;


    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) {
            player = playerObj.transform;
        }
            
        transform.position = new Vector3(SnapToGrid(transform.position.x), SnapToGrid(transform.position.y), transform.position.z);
        StartCoroutine(MoveTick());
    }

    IEnumerator MoveTick()
    {
        while (true)
        {
            yield return new WaitForSeconds(moveTickDuration);
            
            if (!isMoving && canMove)
            {
                Vector2 direction = GetMoveDirection();
                if (direction != Vector2.zero)
                {
                    Vector3 targetPos = transform.position + new Vector3(direction.x, direction.y, 0);

                    // Check if moving into the player
                    Collider2D hit = Physics2D.OverlapCircle(targetPos, 0.1f, LayerMask.GetMask("Player"));
                    if (hit != null)
                    {
                        Debug.Log("Enemy attacks the player!");

                        PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
                        if (playerHealth != null)
                        {
                            playerHealth.TakeDamage(1); // Customize damage here
                        }
                        continue; // Don't move into the player's tile
                    }

                    // If not blocked by player, go ahead and move
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
        //return Mathf.Round(value - 0.5f) + 0.5f;
    }

    void OnCollisionStay(Collision collision)
    {
        Debug.Log("Hit something: " + collision.gameObject.name);
        canMove = false;
    }

    void OnCollisionExit(Collision collision)
    {
        canMove = true;
    }
}
