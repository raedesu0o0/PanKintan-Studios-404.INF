using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Rigidbody2D rb;
    public Animator animator;
    public Transform groundCheckPos;

    [Header("Movement Settings")]
    public float chaseSpeed = 2f;
    public float jumpForce = 7f;
    public float jumpHeightThreshold = 1.5f;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Debug")]
    public bool debugLogs = false;

    private Vector3 originalScale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
    }

    private void FixedUpdate()
    {
        if (player == null)
        {
            Debug.LogWarning("[Enemy] Player reference is not set!");
            return;
        }

        float direction = Mathf.Sign(player.position.x - transform.position.x);

        if (direction != 0)
        {
            transform.localScale = new Vector3(originalScale.x * direction, originalScale.y, originalScale.z);
        }

        rb.velocity = new Vector2(direction * chaseSpeed, rb.velocity.y);

        bool playerIsAbove = player.position.y - transform.position.y > jumpHeightThreshold;

        if (playerIsAbove && IsGrounded())
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            animator.SetTrigger("jump");

            if (debugLogs)
                Debug.Log("[Enemy] Jumping to reach higher platform.");
        }

        animator.SetFloat("magnitude", Mathf.Abs(rb.velocity.x));
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheckPos.position, groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPos != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheckPos.position, groundCheckRadius);
        }

        if (player != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}
