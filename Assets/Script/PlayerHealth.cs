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

    private Color originalColor;

    void Start()
    {
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        ResetHealth();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Enemy hit
        if (collision.CompareTag("Enemy"))
        {
            TakeDamage(1);
            SoundEffectsManager.Instance?.Play("Player Hit");
            return;
        }

        // Trap hit (but not bounce pad)
        if (collision.CompareTag("Trap") && !collision.CompareTag("BouncePad"))
        {
            Traps trap = collision.GetComponent<Traps>();
            if (trap != null)
            {
                TakeDamage(trap.damage);
                SoundEffectsManager.Instance?.Play("Player Hit");

                // Bounce logic
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                if (rb != null)
                    rb.velocity = new Vector2(rb.velocity.x, trap.bounceForce);
            }
        }

        // BouncePad (launch only, no damage)
        if (collision.CompareTag("BouncePad"))
        {
            Traps trap = collision.GetComponent<Traps>();
            if (trap != null)
            {
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                if (rb != null)
                    rb.velocity = new Vector2(rb.velocity.x, trap.bounceForce);
            }
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;

        if (healthUI != null)
            healthUI.Initialize(maxHealth, currentHealth);

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        Debug.Log($"[PlayerHealth] Reset health to {currentHealth}");
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        if (healthUI != null)
            healthUI.UpdateHealth(currentHealth);

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
            healthUI.Initialize(maxHealth, currentHealth);

        Debug.Log($"[PlayerHealth] Max health increased to {maxHealth}");
    }

    private IEnumerator FlashRed()
    {
        if (spriteRenderer == null)
        {
            Debug.LogWarning("[PlayerHealth] SpriteRenderer not assigned!");
            yield break;
        }

        for (int i = 0; i < 2; i++)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
