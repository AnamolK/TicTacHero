using System.Collections;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public float fireRate = 1.0f;
    private float fireCooldown = 0f;
    public float range = 5f;
    public GameObject bulletPrefab;
    public Transform firePoint;

    void Update()
    {
        fireCooldown -= Time.deltaTime;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;
        foreach (GameObject enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < nearestDistance && dist <= range)
            {
                nearestEnemy = enemy;
                nearestDistance = dist;
            }
        }
        if (nearestEnemy != null)
        {
            Vector2 direction = nearestEnemy.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
            if (fireCooldown <= 0f)
            {
                Fire();
                fireCooldown = fireRate;
            }
        }
    }

    void Fire()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}
