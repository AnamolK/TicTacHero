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

    //audio manager
    private AudioSource audioSource;
    public AudioClip[] soundList;
    private AudioClip selected;


    void Start()
    {
        movePoint.parent = null;
        audioSource = GetComponent<AudioSource>();
        animation = asset.GetComponent<AnimationGeneric>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, movePoint.position) <= 0.1f) {
            
            if (Input.GetKeyDown(KeyCode.A)) {
                mostRecentPress = "left";
                playSFX();

            } else if (Input.GetKeyDown(KeyCode.D)) {
                mostRecentPress = "right";
                playSFX();

            } else if (Input.GetKeyDown(KeyCode.S)) {
                mostRecentPress = "down";
                playSFX();

            } else if (Input.GetKeyDown(KeyCode.W)) {
                mostRecentPress = "up";
                playSFX();
                
            } else {
                mostRecentPress = "NaN";
            }

            checkKeyPressChange(mostRecentPress);

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

    void playSFX() {
        int index = Random.Range(0, soundList.Length);
        selected = soundList[index];
        audioSource.clip = selected;
        audioSource.Play();
    }
}
