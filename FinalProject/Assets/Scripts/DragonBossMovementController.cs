using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonBossMovementController : MonoBehaviour
{
    public float moveSpeed = 999f;
    public Transform movePoint;
    public Transform playerPosition;
    public GameObject playerObj;
    public LayerMask BlockedArea;

    private string facing;
    private bool isDead = false;

    //animation/asset manager
    [SerializeField] private GameObject asset;
    private new AnimationGeneric animation;
    private Vector3 originalSpawn;
    public Sprite[] images;
    public GameObject imageContainer;
    public GameObject fireContainer;
    public GameObject neckUp;
    public GameObject neckDown;
    public Transform rotateFire;
    
    void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerPosition = playerObj.transform;
                                             
        if (movePoint != null)
            movePoint.parent = null;

        animation = asset.GetComponent<AnimationGeneric>();
        originalSpawn = transform.position;
    }

    void FixedUpdate()
    {
        if (isDead)
            return;

        Vector3 moveTo = new Vector3(transform.position.x, transform.position.y, 0f);

        if (movePoint != null && movePoint.position.x % 0.5 == 0 && movePoint.position.y % 0.5 == 0)
        {
            Collider2D collider = playerObj.GetComponent<Collider2D>();
            moveTo = new Vector3(movePoint.position.x, movePoint.position.y, 0f);
        }

        if (gameObject.transform.position.y < originalSpawn.y - 1) {
            neckDown.SetActive(true);
            neckUp.SetActive(false);
        } else {
            neckDown.SetActive(false);
            neckUp.SetActive(true);
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
