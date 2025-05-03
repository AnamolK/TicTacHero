using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBossController : MonoBehaviour
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
    public Sprite[] images;
    public GameObject image;
    

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
            Collider2D collider = playerObj.GetComponent<Collider2D>();
            if (!collider.IsTouching(movePoint.GetComponent<Collider2D>()))
            {
                moveTo = new Vector3(movePoint.position.x, movePoint.position.y, 0f);
                jumpImgSequence();
            }
            else
            {
                jumpImgSequence();
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, moveTo, moveSpeed * Time.deltaTime);
    }

    private IEnumerator jumpImgSequence() {
        image.GetComponent<SpriteRenderer>().sprite = images[1];
        yield return new WaitForSeconds(0.02f);
        image.GetComponent<SpriteRenderer>().sprite = images[2];
        yield return new WaitForSeconds(0.27f);
        image.GetComponent<SpriteRenderer>().sprite = images[0];
    }
    void setImage(string val)
    {
        if (val == "N") {
            image.GetComponent<SpriteRenderer>().sprite = images[1];
        }
        else if (val == "E"){
            image.GetComponent<SpriteRenderer>().sprite = images[3];
        } else if (val == "W"){
            image.GetComponent<SpriteRenderer>().sprite = images[2];
        } else if (val == "S") {
            image.GetComponent<SpriteRenderer>().sprite = images[0];
        }
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
