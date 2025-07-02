using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    public ParticleSystem smokeFX; // Fixed: no space in variable name

    [Header("Movement")]
    public float moveSpeed = 5f;
    float horizontalMovement;
    private bool facingRight = true; // Added for flip logic

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
        rb.velocity = new Vector2(horizontalMovement * moveSpeed, rb.velocity.y);

        // Falling gravity
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallGravityMult;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }

        Flip(); // Call flip method here

        animator.SetFloat("yvelocity", rb.velocity.y);
        animator.SetFloat("Magnitude", rb.velocity.magnitude);
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            animator.SetTrigger("Jump");
            smokeFX.Play(); // Particle effect
        }
        else if (context.canceled && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            animator.SetTrigger("Jump");
            smokeFX.Play();
        }
    }

    private void Flip()
    {
        if ((facingRight && horizontalMovement < 0) || (!facingRight && horizontalMovement > 0))
        {
            transform.Rotate(0f, 180f, 0f);
            facingRight = !facingRight; // Flip the bool
            smokeFX.Play();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }
}
