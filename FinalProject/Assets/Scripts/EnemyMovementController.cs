using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform movePoint;

    public LayerMask BlockedArea;

    private string mostRecentPress;

    void Start()
    {
        movePoint.parent = null;
    }

    // Update is called once per frame
    void Update()
    {

        if (!(Physics2D.OverlapCircle(movePoint.position, 0f, BlockedArea)) || transform.position != movePoint.position) {

            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
        }        
    }

}
