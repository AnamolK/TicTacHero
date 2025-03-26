using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementController : MonoBehaviour
{
    public float moveSpeed = 999f;
    public Transform movePoint;
    public Transform playerPosition;

    public LayerMask BlockedArea;

    private string mostRecentPress;

    void Start()
    {
        movePoint.parent = null;
    }

    void FixedUpdate()
    {

        if (movePoint.position.x % 1 == 0 && movePoint.position.y % 1 == 0) {

                var moveTo = new Vector3(movePoint.position.x, movePoint.position.y, 0f);

            if (!(playerPosition.position == movePoint.position)) {

                Debug.Log("POSITION: " + moveTo.x + "  , " + moveTo.y);
                
                transform.position = Vector3.MoveTowards(transform.position, moveTo, moveSpeed * Time.deltaTime);
                
            } else {
                Debug.Log("COLIDE: " + moveTo.x + "  , " + moveTo.y);  
            }

        }
    }
}
