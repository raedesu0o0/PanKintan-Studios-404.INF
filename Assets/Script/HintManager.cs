using UnityEngine;

public class HintManager : MonoBehaviour
{
    [Header("Assign your hint panel UI")]
    public GameObject hintPanel;

    void Start()
    {
        // Ensure hint screen is hidden at game start
        if (hintPanel != null)
            hintPanel.SetActive(false);
    }

    // Call this from the Hint button's OnClick event
    public void ShowHint()
    {
        if (hintPanel != null)
            hintPanel.SetActive(true);
    }

    // Optional: Call this from a Close button inside the panel
    public void HideHint()
    {
        if (hintPanel != null)
            hintPanel.SetActive(false);
    }
}
