using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    public ParticleSystem smokeFX;

    [Header("Movement")]
    public float moveSpeed = 5f;
    float horizontalMovement;
    private bool facingRight = true;

    [Header("Jumping")]
    public float jumpPower = 10f;
    public int maxJumps = 2;
    private int jumpsRemaining;

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.49f, 0.03f);
    public LayerMask groundLayer;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallGravityMult = 2f;

    void Start()
    {
        jumpsRemaining = maxJumps;
    }

    void Update()
    {
        // Flip & animation
        Flip();
        animator.SetFloat("yvelocity", rb.velocity.y);
        animator.SetFloat("Magnitude", rb.velocity.magnitude);
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontalMovement * moveSpeed, rb.velocity.y);

        if (rb.velocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallGravityMult;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && (IsGrounded() || jumpsRemaining > 0))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            jumpsRemaining--;
            animator.SetTrigger("Jump");
            smokeFX.Play();
        }

        if (context.canceled && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }

    private void Flip()
    {
        if ((facingRight && horizontalMovement < 0) || (!facingRight && horizontalMovement > 0))
        {
            transform.Rotate(0f, 180f, 0f);
            facingRight = !facingRight;
            smokeFX.Play();
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0f, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }

    void LateUpdate()
    {
        if (IsGrounded())
        {
            jumpsRemaining = maxJumps;
        }
    }
}
