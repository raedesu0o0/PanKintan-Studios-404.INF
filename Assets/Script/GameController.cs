using UnityEngine;
using UnityEngine.UI;
using TMPro; // You forgot to import this
using System;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    int progressAmount;
    public Slider progressSlider;

    public GameObject player;
    public GameObject loadCanvas;
    public List<GameObject> levels;

    public GameObject gameOverScreen;
    public TMP_Text survivedText;
    private int survivedLevelsCount;

    public static event Action OnReset;


    private int currentLevelIndex = 0;

    void Start()
    {
        progressAmount = 0;
        progressSlider.value = 0;

        Gems.OnGemCollect += IncreaseProgressAmount;
        HoldToLoadLevel.OnHoldComplete += LoadNextLevel;
        PlayerHealth.OnPlayerDied += ShowGameOverScreen;

        loadCanvas.SetActive(false);
        gameOverScreen.SetActive(false);
    }

    void ShowGameOverScreen()
    {
        gameOverScreen.SetActive(true);
        MusicManager.PauseBackgroundMusic();
        survivedText.text = "You Survived " + survivedLevelsCount + " Level" + (survivedLevelsCount == 1 ? "" : "s");
        Time.timeScale = 0; // Pause the game
    }

    public void ResetGame()
    {
        gameOverScreen.SetActive(false);
        MusicManager.PlayBackgroundMusic();
        survivedLevelsCount = 0;
        LoadLevel(0, false);
        OnReset?.Invoke(); // Notify subscribers that the game has been reset
        Time.timeScale = 1; // Resume the game

        // Only reset the spawner if no UI transitions or animations are active
        if (!IsBusyWithUIOrAnimation())
        {
            var spawner = FindObjectOfType<ObjectSpawner>();
            if (spawner != null) spawner.ResetSpawner();
        }
    }

    void IncreaseProgressAmount(int amount)
    {
        progressAmount += amount;
        progressSlider.value = progressAmount;

        if (progressAmount >= 100)
        {
            loadCanvas.SetActive(true);
            Debug.Log("Level Complete");
        }
    }

    void LoadLevel(int level, bool wantSurvivedIncrease)
    {
        loadCanvas.SetActive(false);

        levels[currentLevelIndex].SetActive(false);
        levels[level].SetActive(true);

        player.transform.position = new Vector3(0, 0, 0);

        currentLevelIndex = level;
        progressAmount = 0;
        progressSlider.value = 0;
        if (wantSurvivedIncrease) survivedLevelsCount++;

        // Only reset the spawner if no UI transitions or animations are active
        if (!IsBusyWithUIOrAnimation())
        {
            var spawner = FindObjectOfType<ObjectSpawner>();
            if (spawner != null) spawner.ResetSpawner();
        }
    }

    // Dummy check for UI/animation/coroutine activity. Replace with your own logic if needed.
    private bool IsBusyWithUIOrAnimation()
    {
        // Example: return AnimatorIsPlaying() || UIIsTransitioning();
        // For now, always return false (not busy)
        return false;
    }

    void LoadNextLevel()
    {
        int nextLevelIndex = (currentLevelIndex == levels.Count - 1) ? 0 : currentLevelIndex + 1;
        LoadLevel(nextLevelIndex, true);
    }
}
