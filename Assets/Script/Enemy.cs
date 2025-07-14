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

        // Auto-assign player if not set in Inspector
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("[Enemy] Player not found! Make sure Player is tagged 'Player'");
            }
        }
    }

    private void FixedUpdate()
    {
        if (player == null)
            return;

        float direction = Mathf.Sign(player.position.x - transform.position.x);

        if (direction != 0)
            transform.localScale = new Vector3(direction, 2, 2);

        rb.velocity = new Vector2(direction * chaseSpeed, rb.velocity.y);

        if (debugLogs)
        {
            Debug.Log($"[Enemy] direction: {direction}, velocity: {rb.velocity}");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 checkPos = groundCheckPos ? (Vector2)groundCheckPos.position : (Vector2)transform.position + Vector2.down * 0.5f;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(checkPos, groundCheckRadius);
    }
}
