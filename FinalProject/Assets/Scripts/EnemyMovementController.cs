using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementController : MonoBehaviour
{
    public float moveSpeed = 999f;
    public Transform movePoint;
    public Transform playerPosition;
    public LayerMask BlockedArea;

    private string facing;

    // Flag to stop updates once the enemy is dead.
    private bool isDead = false;

    void Start()
    {
        // Detach movePoint from this enemy.
        if (movePoint != null)
            movePoint.parent = null;
    }

    void FixedUpdate()
    {
        if (isDead)
            return;

        Vector3 moveTo = new Vector3(transform.position.x, transform.position.y, 0f);

        if (movePoint != null && (movePoint.position.x % 1 == 0 && movePoint.position.y % 1 == 0))
        {
            if (!(playerPosition != null && playerPosition.position == movePoint.position))
            {
                moveTo = new Vector3(movePoint.position.x, movePoint.position.y, 0f);
            }
            else
            {
                Debug.Log("COLLIDED W/: " + moveTo.x + " , " + moveTo.y);
                rotateAsset(movePoint.position);
            }
        }

        rotateAsset(moveTo);
        transform.position = Vector3.MoveTowards(transform.position, moveTo, moveSpeed * Time.deltaTime);
    }

    // Sets facing direction based on target position.
    void rotateAsset(Vector3 target)
    {
        if (transform.position.x < target.x)
            facing = "E";
        else if (transform.position.x > target.x)
            facing = "W";
        else if (transform.position.y < target.y)
            facing = "N";
        else if (transform.position.y > target.y)
            facing = "S";

        transform.eulerAngles = new Vector3(transform.position.x, transform.position.y, getRotVal(facing));
    }

    int getRotVal(string val)
    {
        if (val == "N")
            return 0;
        else if (val == "E")
            return 270;
        else if (val == "W")
            return 90;
        else if (val == "S")
            return 180;
        return 180;
    }

    public void Die()
    {
        isDead = true;
        this.enabled = false;
        if (movePoint != null)
            movePoint.gameObject.SetActive(false);
    }
}