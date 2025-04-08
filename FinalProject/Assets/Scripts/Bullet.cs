using UnityEngine;
public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public int baseDamage = 1;
    public float lifetime = 2f;
    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.up * speed;
        Destroy(gameObject, lifetime);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            EnemyPathfinder enemy = collision.GetComponentInParent<EnemyPathfinder>();
            if(enemy != null)
            {
                int turretDamageBonus = Mathf.RoundToInt(UpgradeManager.Instance.turretUpgradeDamageIncrease * UpgradeManager.Instance.turretUpgradeLevel);
                int totalDamage = baseDamage + turretDamageBonus;
                enemy.TakeDamage(totalDamage);
            }
            Destroy(gameObject);
        }
    }
}
