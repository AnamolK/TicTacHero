using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawMovementController : MonoBehaviour
{
   
    public float moveSpeed = 999f;
    public Transform movePoint;
    public Transform playerPosition;
    public GameObject playerObj;
    public LayerMask BlockedArea;

    private string facing;
    private bool isDead = false;

    //animation/asset manager
    [SerializeField] private GameObject assetContainer;
    private new AnimationGeneric animation;
    public GameObject image;

    private bool isJumping = false;
    

    void Start()
    {

        playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerPosition = playerObj.transform;
                                             
        if (movePoint != null)
            movePoint.parent = null;

        animation = assetContainer.GetComponent<AnimationGeneric>();
    }

    void FixedUpdate()
    {
        if (isDead)
            return;

        Vector3 moveTo = new Vector3(transform.position.x, transform.position.y, 0f);

        if (movePoint != null && movePoint.position.x % 0.5 == 0 && movePoint.position.y % 0.5 == 0)
        {
                moveTo = new Vector3(movePoint.position.x, movePoint.position.y, 0f);
        }

        transform.position = Vector3.MoveTowards(transform.position, moveTo, moveSpeed * Time.deltaTime);

    }

    public void Die()
    {
        isDead = true;
        this.enabled = false;
        if (movePoint != null)
            animation.DieEnemy(0.2f);
            Destroy(movePoint.gameObject, 0.2f);
    }
}
