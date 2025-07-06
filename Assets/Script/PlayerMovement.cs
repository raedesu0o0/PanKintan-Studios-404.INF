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
        if (Mathf.Abs(rb.linearVelocity.y) < 0.05f)
        {
            jumpsRemaining = maxJumps;
        }

        // Animator updates
        animator.SetFloat("yvelocity", rb.linearVelocity.y);
        animator.SetFloat("Magnitude", Mathf.Abs(horizontalMovement));
    }

    void FixedUpdate()
    {
        // Move horizontally
        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);

        // Custom gravity when falling
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallGravityMult;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
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
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            jumpsRemaining--;
            animator.SetTrigger("Jump");
            smokeFX.Play();
        }

        // Optional short-hop cancellation
        if (context.canceled && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
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
