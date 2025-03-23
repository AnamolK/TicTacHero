using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAssetMover : MonoBehaviour
{

    public float moveSpeed = 5f;
    public bool canMove = false;
    public Transform movePoint;


    // Update is called once per frame
    void Update()
    {
        if (canMove) {
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
        }
    }
}
