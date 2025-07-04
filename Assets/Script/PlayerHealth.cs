using UnityEngine;
using System;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
public int maxHealth = 3;
    private int currentHealth;

    public HealthUI healthUI;

    public SpriteRenderer spriteRenderer; // Reference to the player's sprite renderer

    public static event Action OnPlayerDied;

    void Start()
    {
        currentHealth = maxHealth;
        healthUI.SetMaxHearts(maxHealth);
        healthUI.UpdateHealth(currentHealth);
        ResetHealth();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            TakeDamage(1);
            SoundEffectsManager.Play("Player Hit"); // Play sound effect when taking damage
        }
        Traps trap = collision.GetComponent<Traps>();
        if (trap)
        {
            TakeDamage(trap.damage);
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, trap.bounceForce);
            }
        }
    }

    private void ResetHealth()
    {
        currentHealth = maxHealth;
        healthUI.SetMaxHearts(maxHealth);
        healthUI.UpdateHealth(currentHealth);
        Debug.Log("Player health reset to max.");
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        healthUI.UpdateHealth(currentHealth);

        if (currentHealth == 0)
        {
            // Handle player death (e.g., reload scene, show game over screen, etc.)
            Debug.Log("Player has died.");
        }
    }

    private IEnumerator Respawn()
    {
        // Implement respawn logic here, e.g., wait for a few seconds and reset position
        yield return new WaitForSeconds(2f);
        transform.position = Vector3.zero; // Reset to starting position
        currentHealth = maxHealth;
        healthUI.UpdateHealth(currentHealth);
    }

    private IEnumerator FlashRed()
    {
        // Implement flashing effect when taking damage
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color originalColor = spriteRenderer.color;
        for (int i = 0; i < 3; i++)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    } 
}
