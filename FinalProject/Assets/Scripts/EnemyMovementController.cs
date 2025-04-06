using UnityEngine;
using UnityEngine.UI;

public class EnemyMovementController : MonoBehaviour
{
    public float moveSpeed = 999f;
    public Transform movePoint;
    public Transform playerPosition;
    public LayerMask BlockedArea;

    private string facing;
    private bool isDead = false;

    //animation/asset manager
    [SerializeField] private GameObject asset;
    private new AnimationGeneric animation;
    public Sprite[] images;
    public GameObject imageContainer;
    

    void Start()
    {

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerPosition = playerObj.transform;
                                             
        if (movePoint != null)
            movePoint.parent = null;

        animation = asset.GetComponent<AnimationGeneric>();
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
                rotateAsset(movePoint.position);
            }
        }

        rotateAsset(moveTo);
        transform.position = Vector3.MoveTowards(transform.position, moveTo, moveSpeed * Time.deltaTime);
    }

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

        setImage(facing);
    }

    void setImage(string val)
    {
        if (val == "N")
            imageContainer.GetComponent<SpriteRenderer>().sprite = images[1];
        else if (val == "E")
            imageContainer.GetComponent<SpriteRenderer>().sprite = images[3];
        else if (val == "W")
            imageContainer.GetComponent<SpriteRenderer>().sprite = images[2];
        else if (val == "S")
            imageContainer.GetComponent<SpriteRenderer>().sprite = images[0];
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
