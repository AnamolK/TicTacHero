using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    
    public float moveSpeed = 20f;
    public Transform player;
    public float borderDistance = 1;
    public bool snapToGrid = false;

    private Vector3 moveTo = new Vector3(0f, 0f, -10f);
    

    // Update is called once per frame
    void FixedUpdate()
    {
        
        //check if distance is greater on x
        if (Mathf.Abs(transform.position.x - player.position.x) > 8-borderDistance || Mathf.Abs(transform.position.y - player.position.y) > 4-borderDistance) {


            //checks if player is at an integer position
            if ((player.position.x % 1 == 0 && player.position.y % 1 == 0) || !snapToGrid) {

                moveTo = new Vector3(player.position.x, player.position.y, transform.position.z);
                transform.position = Vector3.MoveTowards(transform.position, moveTo, moveSpeed * Time.deltaTime);
                
            }
        } 

        //move until correct position is reached
        if (transform.position != moveTo) {
            
        }   
    }
}
