using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
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
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, movePoint.position) <= 0.1f) {
            
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f) {
                mostRecentPress = "x";
            } 
            
            if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f) {
                mostRecentPress = "y";
            }

            checkKeyPressChange(mostRecentPress);

        }        
    }

    void checkKeyPressChange (string direction) {

        if (direction == "x") {

            if (!(Physics2D.OverlapCircle(movePoint.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f), 0.05f, BlockedArea))) {
                movePoint.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
            }

        } else {

            if (!(Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f), 0.05f, BlockedArea))) {
                movePoint.position += new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
            }

        }

    }
}
