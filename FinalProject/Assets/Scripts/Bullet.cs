using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 1;
    public float speed = 5f;
    public float lifetime = 2f;

    private Vector2 direction;

    void Start() {
        direction = transform.up;
        Destroy(gameObject, lifetime);
    }

    void Update() {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if(collision.CompareTag("Enemy")) {
            EnemyPathfinder enemy = collision.GetComponentInParent<EnemyPathfinder>();
            if(enemy != null) {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}
