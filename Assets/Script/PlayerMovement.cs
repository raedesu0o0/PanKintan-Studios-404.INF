using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    public ParticleSystem smokeFX;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpPower = 10f;
    public int maxJumps = 2;
    private int jumpsRemaining;

    [Header("Gravity Settings")]
    public float baseGravity = 2f;
    public float fallGravityMult = 2f;
    public float maxFallSpeed = 18f;

    private float horizontalMovement;

    void Start()
    {
        jumpsRemaining = maxJumps;
    }

    void Update()
    {
        // Flip direction based on input
        Flip();

        // Reset jumps when vertical velocity is near zero (landing)
        if (Mathf.Abs(rb.velocity.y) < 0.05f)
        {
            jumpsRemaining = maxJumps;
        }

        // Animator updates
        animator.SetFloat("yvelocity", rb.velocity.y);
        animator.SetFloat("Magnitude", Mathf.Abs(horizontalMovement));
    }

    void FixedUpdate()
    {
        // Move horizontally
        rb.velocity = new Vector2(horizontalMovement * moveSpeed, rb.velocity.y);

        // Custom gravity when falling
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
        if (context.performed && jumpsRemaining > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            jumpsRemaining--;
            animator.SetTrigger("Jump");
            smokeFX.Play();
        }

        // Optional short-hop cancellation
        if (context.canceled && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }

    private void Flip()
    {
        if ((horizontalMovement < 0 && transform.localScale.x > 0) ||
            (horizontalMovement > 0 && transform.localScale.x < 0))
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;

            smokeFX.Play(); // Optional flip FX
        }
    }
}
