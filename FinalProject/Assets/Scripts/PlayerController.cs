using System.Collections;
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float moveDuration = 0.1f;
    private bool isMoving = false;
    private Vector2 moveDirection = Vector2.right;
    void Update()
    {
        if (!isMoving)
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (input != Vector2.zero)
            {
                if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
                    input.y = 0;
                else
                    input.x = 0;
                moveDirection = input;
                StartCoroutine(Move(input));
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Attack();
            }
        }
    }
    IEnumerator Move(Vector2 direction)
    {
        isMoving = true;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(direction.x, direction.y, 0);
        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = endPos;
        isMoving = false;
    }
    void Attack()
    {
        Vector2 offset = Vector2.left * 1.0f;
        Vector2 attackPos = (Vector2)transform.position + offset;
        float attackRadius = 3f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPos, attackRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    enemy.TakeDamage(1);
                }
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 offset = Vector2.left * 1.0f;
        Vector2 attackPos = (Vector2)transform.position + offset;
        Gizmos.DrawWireSphere(attackPos, 1.5f);
    }
}
