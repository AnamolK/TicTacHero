using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{

    public float moveSpeed = 5f;
    public Transform movePoint;

    public LayerMask BlockedArea;
    
    //animation/asset manager
    [SerializeField] private GameObject asset;
    private new AnimationGeneric animation;
    public Sprite[] images;
    public GameObject imageContainer;

    private string mostRecentPress;
    private bool isHeldHori;
    private bool isHeldVert;
    public float dashDistance = 2f;
    public float dashCooldown = 1f;
    private bool canDash = true;
    private PlayerStats playerStats;


    //audio manager
    private AudioSource audioSource;
    public AudioClip[] soundList;
    private AudioClip selected;
    


    void Start()
    {
        movePoint.parent = null;
        audioSource = GetComponent<AudioSource>();
        animation = asset.GetComponent<AnimationGeneric>();
        playerStats = GetComponent<PlayerStats>();

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, movePoint.position) <= 0.1f) {
            
            if (Input.GetAxisRaw("Horizontal") == -1f && isHeldHori == false) {
                mostRecentPress = "left";
                playSFX();

            } else if (Input.GetAxisRaw("Horizontal") == 1f && isHeldHori == false) {
                mostRecentPress = "right";
                playSFX();

            } else if (Input.GetAxisRaw("Vertical") == -1f && isHeldVert == false) {
                mostRecentPress = "down";
                playSFX();

            } else if (Input.GetAxisRaw("Vertical") == 1f && isHeldVert == false) {
                mostRecentPress = "up";
                playSFX();
                
            } else {
                mostRecentPress = "NaN";
            }

            checkKeyPressChange(mostRecentPress);

        }      
        checkForHoldHori(); 
        checkForHoldVert();
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && playerStats != null && playerStats.dashUnlocked)
        {
            StartCoroutine(PerformDash());
        }

    }

    void checkKeyPressChange (string direction) {

        if (direction == "left") {

            if (!(Physics2D.OverlapCircle(movePoint.position + new Vector3(-1f, 0f, 0f), 0.05f, BlockedArea))) {
                movePoint.position += new Vector3(-1f, 0f, 0f);
                animation.MoveShift(new Vector3(-0.3f, 0f, 0f), 0.3f);
                imageContainer.GetComponent<SpriteRenderer>().sprite = images[2];
            }

        } else if (direction == "right") {

            if (!(Physics2D.OverlapCircle(movePoint.position + new Vector3(1f, 0f, 0f), 0.05f, BlockedArea))) {
                movePoint.position += new Vector3(1f, 0f, 0f);
                animation.MoveShift(new Vector3(0.3f, 0f, 0f), 0.2f);
                imageContainer.GetComponent<SpriteRenderer>().sprite = images[3];
            }

        } else if (direction == "down") {

            if (!(Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, -1f, 0f), 0.05f, BlockedArea))) {
                movePoint.position += new Vector3(0f, -1f, 0f);
                animation.MoveShift(new Vector3(0f, -0.3f, 0f), 0.2f);
                imageContainer.GetComponent<SpriteRenderer>().sprite = images[0];
            }

        } else if (direction == "up") {

            if (!(Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, 1f, 0f), 0.05f, BlockedArea))) {
                movePoint.position += new Vector3(0f, 1f, 0f);
                animation.MoveShift(new Vector3(0f, 0.3f, 0f), 0.2f);
                imageContainer.GetComponent<SpriteRenderer>().sprite = images[1];
            }

        }

    }

    bool checkForHoldHori() {
        if (Input.GetAxisRaw("Horizontal") == 0f) {
            isHeldHori = false;
        } else {
            isHeldHori = true;
        }
        return isHeldHori;
    }

        bool checkForHoldVert() {
        if (Input.GetAxisRaw("Vertical") == 0f) {
            isHeldVert = false;
        } else {
            isHeldVert = true;
        }
        return isHeldVert;
    }

    void playSFX() {
        int index = Random.Range(0, soundList.Length);
        selected = soundList[index];
        audioSource.clip = selected;
        audioSource.PlayOneShot(selected);
    }

    private IEnumerator PerformDash()
    {
        canDash = false;

        Vector3 dashDirection = movePoint.position - transform.position;
        if (dashDirection == Vector3.zero)
            dashDirection = transform.right; // fallback in case

        Vector3 targetPos = movePoint.position + dashDirection.normalized * dashDistance;

        if (!Physics2D.OverlapCircle(targetPos, 0.05f, BlockedArea))
        {
            movePoint.position = new Vector3(
                Mathf.Round(targetPos.x),
                Mathf.Round(targetPos.y),
                movePoint.position.z
            );
            Debug.Log("Dashed to " + movePoint.position);
        }
        else
        {
            Debug.Log("Dash blocked!");
        }

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}


