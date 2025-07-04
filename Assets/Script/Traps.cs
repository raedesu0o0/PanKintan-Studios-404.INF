using UnityEngine;

public class Traps : MonoBehaviour
{
    public float bounceForce = 10f;
    public int damage = 1;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce);
                }
                if (animator != null)
                {
                    animator.SetTrigger("Bounce pad"); // Make sure your Animator has a trigger named "Bounce pad"
                }
            }
        }
    }
}
