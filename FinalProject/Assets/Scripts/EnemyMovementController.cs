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
    private string facing;

    void Start()
    {
        movePoint.parent = null;
    }

    void FixedUpdate()
    {
        var moveTo = new Vector3(transform.position.x, transform.position.y, 0f);

        if (movePoint.position.x % 1 == 0 && movePoint.position.y % 1 == 0) {

            if (!(playerPosition.position == movePoint.position)) {
                
                moveTo = new Vector3(movePoint.position.x, movePoint.position.y, 0f);
                
            } else {
                Debug.Log("COLLIDED W/: " + moveTo.x + "  , " + moveTo.y);
                rotateAsset(movePoint.position);
            }

        }

        rotateAsset(moveTo);
        transform.position = Vector3.MoveTowards(transform.position, moveTo, moveSpeed * Time.deltaTime);

    }

    //set facing direction depending on where the obj has to move.
    void rotateAsset(Vector3 target) {
        if (transform.position.x < target.x) {
            facing = "E";
        } else if (transform.position.x > target.x) {
            facing = "W";
        } else if (transform.position.y < target.y) {
            facing = "N";
        } else if (transform.position.y > target.y) {
            facing = "S";
        }

        transform.eulerAngles = new Vector3(transform.position.x, transform.position.y, getRotVal(facing));
    }

    int getRotVal(string val) {
        if (val == "N") {
            return 0;
        } else if (val == "E") {
            return 270;
        } else if (val == "W") {
            return 90;
        } else if (val == "S"){
            return 180;
        }
        return 180;
    }
}
