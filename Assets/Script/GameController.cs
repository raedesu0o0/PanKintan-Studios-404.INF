using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider progressSlider;
    public GameObject loadCanvas;
    public GameObject gameOverScreen;
    public TMP_Text survivedText;

    [Header("Gameplay Elements")]
    public GameObject player;
    public List<GameObject> levels;

    public static event Action OnReset;

    private int progressAmount;
    private int currentLevelIndex = 0;
    private int survivedLevelsCount;

    private ObjectSpawner spawner;

    private void Start()
    {
        // Ensure game isn't frozen
        Time.timeScale = 1f;

        progressAmount = 0;
        progressSlider.value = 0;

        // Subscribe to events
        Gems.OnGemCollect += IncreaseProgressAmount;
        HoldToLoadLevel.OnHoldComplete += LoadNextLevel;
        PlayerHealth.OnPlayerDied += ShowGameOverScreen;

        // Initial UI setup
        loadCanvas.SetActive(false);
        gameOverScreen.SetActive(false);

        // Cache spawner reference early
        spawner = FindObjectOfType<ObjectSpawner>();

        // Ensure first level is active
        LoadLevel(0, false);
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        Gems.OnGemCollect -= IncreaseProgressAmount;
        HoldToLoadLevel.OnHoldComplete -= LoadNextLevel;
        PlayerHealth.OnPlayerDied -= ShowGameOverScreen;
    }

    private void ShowGameOverScreen()
    {
        gameOverScreen.SetActive(true);
        survivedText.text = "You Survived " + survivedLevelsCount + " Level" + (survivedLevelsCount == 1 ? "" : "s");
        Time.timeScale = 0f; // Freeze the game
    }

    public void ResetGame()
    {
        gameOverScreen.SetActive(false);
        MusicManager.ResumeBackgroundMusic(); // Resume music if paused
        Time.timeScale = 1f;
        survivedLevelsCount = 0;

        LoadLevel(0, false);
        OnReset?.Invoke();

        if (!IsBusyWithUIOrAnimation() && spawner != null)
        {
            spawner.ResetSpawner();
        }
    }

    private void IncreaseProgressAmount(int amount)
    {
        progressAmount += amount;
        progressSlider.value = progressAmount;

        if (progressAmount >= 100)
        {
            loadCanvas.SetActive(true);
            Debug.Log("Level Complete");
        }
    }

    private void LoadLevel(int level, bool wantSurvivedIncrease)
    {
        loadCanvas.SetActive(false);

        if (level < 0 || level >= levels.Count)
        {
            Debug.LogWarning("Invalid level index!");
            return;
        }

        levels[currentLevelIndex].SetActive(false);
        levels[level].SetActive(true);

        currentLevelIndex = level;
        progressAmount = 0;
        progressSlider.value = 0;

        if (wantSurvivedIncrease)
        {
            survivedLevelsCount++;
        }

        // Reset player position to origin or safe spawn point
        player.transform.position = Vector3.zero;

        if (!IsBusyWithUIOrAnimation() && spawner != null)
        {
            spawner.ResetSpawner();
        }
    }

    private void LoadNextLevel()
    {
        int nextLevelIndex = (currentLevelIndex + 1) % levels.Count;
        LoadLevel(nextLevelIndex, true);
    }

    // Stub for checking if game is mid-transition or busy
    private bool IsBusyWithUIOrAnimation()
    {
        return false; // Replace with actual UI/animation status if needed
    }
}
