using UnityEngine;
using System;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 7;
    private int currentHealth;

    [Header("References")]
    public HealthUI healthUI;
    public SpriteRenderer spriteRenderer;

    public static event Action OnPlayerDied;

    void Start()
    {
        ResetHealth();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            TakeDamage(1);
            SoundEffectsManager.Play("Player Hit");
        }

        Traps trap = collision.GetComponent<Traps>();
        if (trap)
        {
            TakeDamage(trap.damage);

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = new Vector2(rb.velocity.x, trap.bounceForce);
            }

            SoundEffectsManager.Play("Player Hit");
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;

        if (healthUI != null)
        {
            healthUI.SetMaxHearts(maxHealth, currentHealth);
            healthUI.UpdateHealth(currentHealth);
        }
        else
        {
            Debug.LogWarning("[PlayerHealth] Health UI not assigned!");
        }

        Debug.Log($"[PlayerHealth] Health reset to {currentHealth}");
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        if (healthUI != null)
        {
            healthUI.UpdateHealth(currentHealth);
        }

        StartCoroutine(FlashRed());

        if (currentHealth == 0)
        {
            Debug.Log("[PlayerHealth] Player has died.");
            OnPlayerDied?.Invoke();
        }
    }

    public void AddMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth = maxHealth;

        if (healthUI != null)
        {
            healthUI.SetMaxHearts(maxHealth, currentHealth);
            healthUI.UpdateHealth(currentHealth);
        }

        Debug.Log($"[PlayerHealth] Max health increased to {maxHealth}");
    }

    private IEnumerator FlashRed()
    {
        if (spriteRenderer == null)
        {
            Debug.LogWarning("[PlayerHealth] SpriteRenderer not assigned!");
            yield break;
        }

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
