using UnityEngine;

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

        //transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, getRotVal(facing));
    }

    int getRotVal(string val)
    {
        if (val == "N")
            return 0;
        else if (val == "E")
            return 270;
        else if (val == "W")
            return 90;
        else if (val == "S")
            return 180;
        return 180;
    }

    public void Die()
    {
        isDead = true;
        this.enabled = false;
        if (movePoint != null)
            Destroy(movePoint.gameObject, 0.1f);
    }
}
