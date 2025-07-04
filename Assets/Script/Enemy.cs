using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform player;
    public float chaseSpeed = 2f;
    public float jumpForce = 2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    public Transform groundCheckPos;
    public float groundCheckRadius = 0.2f;
    public int damage = 1;

    public bool debugLogs = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (player == null)
        {
            Debug.LogWarning("[Enemy] Player reference is not set!");
            return;
        }

        // Get direction toward player
        float direction = Mathf.Sign(player.position.x - transform.position.x);

        // Face player
        if (direction != 0)
        {
            transform.localScale = new Vector3(direction, 1, 1);
        }

        // Move towards player
        rb.velocity = new Vector2(direction * chaseSpeed, rb.velocity.y);

        if (debugLogs)
        {
            Debug.Log($"[Enemy] direction: {direction}, chaseSpeed: {chaseSpeed}");
            Debug.Log($"[Enemy] velocity set to: {rb.velocity}");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 checkPos = groundCheckPos ? (Vector2)groundCheckPos.position : (Vector2)transform.position + Vector2.down * 0.5f;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(checkPos, groundCheckRadius);
    }
}
