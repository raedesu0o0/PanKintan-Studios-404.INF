using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform player;
    public float chaseSpeed = 2f;
    public float jumpForce = 2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool shouldJump;
    public Transform groundCheckPos;
    public float groundCheckRadius = 0.2f;

    public int damage = 1;

    //Start is called before the first frame update

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Ground check removed
    }

    private void FixedUpdate()
    {
        if (player == null) {
            Debug.LogWarning("[Enemy] Player reference is not set!");
            return;
        }

        //Player Direction
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        Debug.Log($"[Enemy] direction: {direction}, chaseSpeed: {chaseSpeed}");

        //Chase Player at all times (no ground check)
        rb.linearVelocity = new Vector2(direction * chaseSpeed, rb.linearVelocity.y);
        Debug.Log($"[Enemy] Setting velocity to: {rb.linearVelocity}");
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize ground check
        Vector2 checkPos = groundCheckPos ? (Vector2)groundCheckPos.position : (Vector2)transform.position + Vector2.down * 0.5f;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(checkPos, groundCheckRadius);
    }

    
}


