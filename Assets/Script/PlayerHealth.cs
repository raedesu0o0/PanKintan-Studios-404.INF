using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 7;
    public float invincibleDuration = 2f;
    public SpriteRenderer spriteRenderer;

    [Header("References")]
    [SerializeField] private HealthUI healthUIPrefab;  // For prefab assignment
    [NonSerialized] public HealthUI healthUI;          // Single runtime reference

    private int currentHealth;
    private bool isInvincible = false;
    private float invincibleTimer = 0f;

    public static event Action OnPlayerDied;

    private void Awake()
    {
        InitializeHealthUI();
        ResetHealth();
    }

    private void InitializeHealthUI()
    {
        // Find existing or create new HealthUI
        healthUI = FindAnyObjectByType<HealthUI>();
        if (healthUI == null && healthUIPrefab != null)
        {
            healthUI = Instantiate(healthUIPrefab);
            var canvas = FindAnyObjectByType<Canvas>();
            if (canvas != null)
            {
                healthUI.transform.SetParent(canvas.transform, false);
                healthUI.transform.localPosition = Vector3.zero; 
            }
        }
    }
    private void Update()
    {
        HandleInvincibility();
    }

    private void HandleInvincibility()
    {
        if (!isInvincible) return;

        invincibleTimer -= Time.deltaTime;
        spriteRenderer.color = (Mathf.Floor(invincibleTimer * 10f) % 2 == 0) ? Color.clear : Color.white;
        
        if (invincibleTimer <= 0f)
        {
            isInvincible = false;
            spriteRenderer.color = Color.white;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isInvincible)
        {
            TakeDamage(1);
            SoundManager.PlaySound("PlayerHit");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleTrapDamage(other);
        HandleBouncePad(other);
    }

    private void HandleTrapDamage(Collider2D other)
    {
        if (!other.CompareTag("BouncePad") && !isInvincible)
        {
            Traps trap = other.GetComponent<Traps>();
            if (trap != null)
            {
                TakeDamage(trap.damage);
                ApplyBounceForce(trap.bounceForce);
            }
        }
    }

    private void HandleBouncePad(Collider2D other)
    {
        if (other.CompareTag("BouncePad"))
        {
            Traps bouncePad = other.GetComponent<Traps>();
            if (bouncePad != null)
            {
                ApplyBounceForce(bouncePad.bounceForce);
            }
        }
    }

    private void ApplyBounceForce(float force)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = new Vector2(rb.velocity.x, force);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isInvincible) return;

        currentHealth = Mathf.Max(currentHealth - amount, 0);
        healthUI?.UpdateHealth(currentHealth);

        isInvincible = true;
        invincibleTimer = invincibleDuration;

        if (currentHealth <= 0)
        {
            OnPlayerDied?.Invoke();
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        healthUI?.Initialize(maxHealth, currentHealth);
        isInvincible = false;
        spriteRenderer.color = Color.white;
    }

    private void OnDestroy()
{
    // Clean up when player is destroyed (e.g., new scene load)
    if (healthUI != null && FindAnyObjectByType<PlayerHealth>() == this)
    {
        Destroy(healthUI.gameObject);
    }
}
}