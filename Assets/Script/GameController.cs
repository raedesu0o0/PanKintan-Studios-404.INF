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
        SoundEffectsManager.Play("GameOver");
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
        Debug.Log("Level Complete");

        // ✅ Show the level complete menu
        LevelCompleteMenu.Show();

        // Optional: hide or disable loadCanvas if not needed
        loadCanvas.SetActive(false); // or true, if it’s a transition background
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

    // ✅ Disable the current level if it exists
    if (currentLevelIndex >= 0 && currentLevelIndex < levels.Count)
    {
        levels[currentLevelIndex].SetActive(false);
    }

    // ✅ Activate the new level
    levels[level].SetActive(true);
    currentLevelIndex = level;

    progressAmount = 0;
    progressSlider.value = 0;

    if (wantSurvivedIncrease)
    {
        survivedLevelsCount++;
    }

    // Reset player position
    player.transform.position = Vector3.zero;

    // Reset enemies, spawners, etc.
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
