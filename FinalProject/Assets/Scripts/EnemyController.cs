using System.Collections;
using UnityEngine;
public class EnemyController : MonoBehaviour
{
    public float moveTickDuration = 0.5f;
    public float moveDuration = 0.1f;
    private Transform player;
    private bool isMoving = false;
    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        StartCoroutine(MoveTick());
    }
    IEnumerator MoveTick()
    {
        while (true)
        {
            yield return new WaitForSeconds(moveTickDuration);
            if (!isMoving)
            {
                Vector2 direction = GetMoveDirection();
                if (direction != Vector2.zero)
                {
                    StartCoroutine(Move(direction));
                }
            }
        }
    }
    Vector2 GetMoveDirection()
    {
        if (player == null) return Vector2.zero;
        Vector2 pos = transform.position;
        Vector2 playerPos = player.position;
        Vector2 diff = playerPos - pos;
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            return new Vector2(Mathf.Sign(diff.x), 0);
        else if (Mathf.Abs(diff.y) > 0)
            return new Vector2(0, Mathf.Sign(diff.y));
        return Vector2.zero;
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
}
