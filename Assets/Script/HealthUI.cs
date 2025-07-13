using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HealthUI : MonoBehaviour
{
    [Header("Heart Settings")]
    [SerializeField] private Image heartPrefab;
    [SerializeField] private Sprite fullHeartSprite;
    [SerializeField] private Sprite emptyHeartSprite;
    [SerializeField] private float heartSpacing = 30f;
    [SerializeField] private Color fullHeartColor = Color.red;
    [SerializeField] private Color emptyHeartColor = new Color(0.5f, 0.5f, 0.5f, 0.7f);

    private List<Image> hearts = new List<Image>();
    private Canvas parentCanvas;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null)
        {
            DontDestroyOnLoad(parentCanvas.gameObject);
        }
    }

    public void Initialize(int maxHearts, int currentHealth)
    {
        ClearHearts();
        CreateHearts(maxHearts, currentHealth);
    }

    private void CreateHearts(int maxHearts, int currentHealth)
    {
        float startX = -(maxHearts - 1) * heartSpacing / 2f;

        for (int i = 0; i < maxHearts; i++)
        {
            Image newHeart = Instantiate(heartPrefab, transform);
            newHeart.rectTransform.anchoredPosition = new Vector2(
                startX + i * heartSpacing, 
                0f
            );
            UpdateHeartVisual(newHeart, i < currentHealth);
            hearts.Add(newHeart);
        }
    }

    public void UpdateHealth(int currentHealth)
    {
        if (hearts.Count == 0) return;

        for (int i = 0; i < hearts.Count; i++)
        {
            if (hearts[i] != null)
            {
                bool isFull = i < currentHealth;
                UpdateHeartVisual(hearts[i], isFull);
            }
        }
    }

    private void UpdateHeartVisual(Image heart, bool isFull)
    {
        heart.sprite = isFull ? fullHeartSprite : emptyHeartSprite;
        heart.color = isFull ? fullHeartColor : emptyHeartColor;
    }

    private void ClearHearts()
    {
        foreach (Image heart in hearts)
        {
            if (heart != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(heart.gameObject);
                }
                else
                {
                    DestroyImmediate(heart.gameObject);
                }
            }
        }
        hearts.Clear();
    }

    public void ResetUI()
    {
        ClearHearts();
        if (gameObject != null && gameObject != this)
        {
            if (Application.isPlaying)
            {
                Destroy(gameObject);
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }
    }
}