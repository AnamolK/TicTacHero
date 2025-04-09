using UnityEngine;

public class FireProjectile : MonoBehaviour
{
    [Header("Fire Sprites by Direction (Down, Up, Left, Right)")]
    public Sprite[] directionSprites;
    
    [Header("Stats")]
    public float speed = 3f;  // 0 if you want no movement
    public int damage = 2;    // how much damage to player
    public float lifetime = 2f; // how many seconds before it disappears

    private Vector2 moveDirection;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Dragon calls this to set direction + choose correct sprite
    public void Init(Vector2 direction)
    {
        moveDirection = direction.normalized;

        // Decide which sprite to use
        // We'll treat directionSprites as [0]=Down, [1]=Up, [2]=Left, [3]=Right
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // It's more horizontal
            if (direction.x > 0)
                sr.sprite = directionSprites[3]; // Right
            else
                sr.sprite = directionSprites[2]; // Left
        }
        else
        {
            // It's more vertical or equal
            if (direction.y > 0)
                sr.sprite = directionSprites[1]; // Up
            else
                sr.sprite = directionSprites[0]; // Down
        }

        // Destroy itself after 'lifetime' seconds
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // If speed > 0, it travels in the direction set by Init()
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }

    // OnTriggerEnter2D if it hits the player
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // If Player has a "PlayerHealth" or similar:
            // other.GetComponent<PlayerHealth>()?.TakeDamage(damage);

            Debug.Log("Fire hit the player, dealing " + damage + " damage!");
            // Destroy after hitting
            Destroy(gameObject);
        }
    }
}
