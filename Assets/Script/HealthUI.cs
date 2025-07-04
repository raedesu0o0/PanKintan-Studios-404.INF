using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HealthUI : MonoBehaviour
{
    public Image heartPrefab;
    public Sprite fullHeartSprite;
    public Sprite emptyHeartSprite;

    private List<Image> hearts = new List<Image>();

    public void SetMaxHearts(int maxHearts, int currentHealth)
    {
        foreach (Image heart in hearts)
        {
            Destroy(heart.gameObject);
        }

        hearts.Clear();

        for (int i = 0; i < maxHearts; i++)
        {
            Image heart = Instantiate(heartPrefab, transform);
            heart.sprite = i < currentHealth ? fullHeartSprite : emptyHeartSprite;
            heart.color = i < currentHealth ? Color.red : Color.gray;
            hearts.Add(heart);
        }
    }

    public void UpdateHealth(int currentHealth)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            hearts[i].sprite = i < currentHealth ? fullHeartSprite : emptyHeartSprite;
            hearts[i].color = i < currentHealth ? Color.red : Color.gray;
        }
    }
}
