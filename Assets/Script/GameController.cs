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

    private ObjectSpawner spawner;

    private void Start()
    {
        Time.timeScale = 1f;

        progressAmount = 0;
        progressSlider.value = 0;

        Gems.OnGemCollect += IncreaseProgressAmount;
        HoldToLoadLevel.OnHoldComplete += LoadNextLevel;
        PlayerHealth.OnPlayerDied += ShowGameOverScreen;

        loadCanvas.SetActive(false);
        gameOverScreen.SetActive(false);

        spawner = FindObjectOfType<ObjectSpawner>();

        LoadLevel(0);
    }

    private void OnDestroy()
    {
        Gems.OnGemCollect -= IncreaseProgressAmount;
        HoldToLoadLevel.OnHoldComplete -= LoadNextLevel;
        PlayerHealth.OnPlayerDied -= ShowGameOverScreen;
    }

    private void ShowGameOverScreen()
    {
        gameOverScreen.SetActive(true);
        Time.timeScale = 0f;
        SoundEffectsManager.Play("GameOver");

        // You can assign a message to survivedText manually in the Unity Editor
    }

    public void ResetGame()
    {
        gameOverScreen.SetActive(false);
        MusicManager.ResumeBackgroundMusic();
        Time.timeScale = 1f;

        LoadLevel(0);
        OnReset?.Invoke();

        if (spawner != null)
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

    private void LoadLevel(int levelIndex)
    {
        loadCanvas.SetActive(false);

        if (levelIndex < 0 || levelIndex >= levels.Count)
        {
            Debug.LogWarning("Invalid level index!");
            return;
        }

        foreach (var level in levels)
        {
            level.SetActive(false);
        }

        levels[levelIndex].SetActive(true);
        currentLevelIndex = levelIndex;

        progressAmount = 0;
        progressSlider.value = 0;

        player.transform.position = Vector3.zero;

        if (spawner != null)
        {
            spawner.ResetSpawner();
        }
    }

    private void LoadNextLevel()
    {
        int nextLevelIndex = (currentLevelIndex + 1) % levels.Count;
        LoadLevel(nextLevelIndex);
    }
}
