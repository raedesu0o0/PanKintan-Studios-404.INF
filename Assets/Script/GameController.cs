using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Collections;

public class GameController : MonoBehaviour
{
    [Header("UI Elements - Assign in Each Scene")]
    [SerializeField] private Slider progressSlider;
    [SerializeField] private GameObject loadCanvas;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private HealthUI healthUI;

    [Header("Gameplay Elements")]
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;

    [Header("Scene Index Config")]
    public int firstLevelSceneIndex = 2;
    public int lastLevelSceneIndex = 4;

    private int currentLevelIndex;
    private int progressAmount;
    private GameObject currentPlayer;
    private bool hasLoadedNext = false; // Prevents multiple scene loads

    public static event Action OnReset;

    private void Start()
    {
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex; // Ensure it's set
        SubscribeToEvents();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Assign scene-specific references
        progressSlider = GameObject.Find("ProgressSlider")?.GetComponent<Slider>();
        loadCanvas = GameObject.Find("LoadCanvas");
        gameOverScreen = GameObject.Find("GameOverScreen");
        healthUI = FindObjectOfType<HealthUI>();

        // Log if something is missing
        if (progressSlider == null) Debug.LogError("[GameController] Progress Slider not found in scene!");
        if (loadCanvas == null) Debug.LogError("[GameController] Load Canvas not found in scene!");
        if (gameOverScreen == null) Debug.LogError("[GameController] Game Over Screen not found in scene!");
        if (healthUI == null) Debug.LogError("[GameController] Health UI not found in scene!");

        InitializeGameState();
    }

    private void InitializeGameState()
    {
        Time.timeScale = 1f;
        progressAmount = 0;
        hasLoadedNext = false;
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;

        if (progressSlider != null) progressSlider.value = 0;
        if (loadCanvas != null) loadCanvas.SetActive(false);
        if (gameOverScreen != null) gameOverScreen.SetActive(false);

        SpawnPlayer();
    }

    private void SubscribeToEvents()
    {
        Gems.OnGemCollect += IncreaseProgressAmount;
        PlayerHealth.OnPlayerDied += ShowGameOverScreen;
    }

    private void SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogWarning("[GameController] Player prefab is not assigned.");
            return;
        }

        if (currentPlayer != null)
            Destroy(currentPlayer);

        Vector3 spawnPosition = playerSpawnPoint != null ? playerSpawnPoint.position : Vector3.zero;
        currentPlayer = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

        var playerHealth = currentPlayer.GetComponent<PlayerHealth>();
        if (playerHealth != null && healthUI != null)
        {
            playerHealth.healthUI = healthUI;
            playerHealth.ResetHealth();
        }
    }

    private void IncreaseProgressAmount(int amount)
    {
        if (progressSlider == null || hasLoadedNext) return;

        progressAmount = Mathf.Clamp(progressAmount + amount, 0, 100);
        progressSlider.value = progressAmount;

        Debug.Log($"[GameController] Progress now at {progressAmount}%");

        if (progressAmount >= 100)
        {
            hasLoadedNext = true;
            StartCoroutine(LoadNextLevelAfterDelay(1.5f));
        }
    }

    private IEnumerator LoadNextLevelAfterDelay(float delay)
    {
        if (loadCanvas != null)
            loadCanvas.SetActive(true); // Optional UI feedback

        yield return new WaitForSeconds(delay);

        LoadNextLevel();
    }

    private void LoadNextLevel()
    {
        // Ensure we're only loading from valid levels
        if (currentLevelIndex < firstLevelSceneIndex || currentLevelIndex > lastLevelSceneIndex)
        {
            Debug.LogWarning($"[GameController] Current scene ({currentLevelIndex}) is not a gameplay level.");
            return;
        }

        int nextIndex = currentLevelIndex + 1;

        if (nextIndex > lastLevelSceneIndex)
        {
            Debug.Log("[GameController] Reached last level. Looping to first gameplay level.");
            nextIndex = firstLevelSceneIndex;
        }

        Debug.Log($"[GameController] Loading scene index {nextIndex}");
        SceneManager.LoadScene(nextIndex, LoadSceneMode.Single);
    }

    private void ShowGameOverScreen()
    {
        Debug.Log("[GameController] Game Over triggered");
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
            Time.timeScale = 0f;

            if (SoundEffectsManager.Instance != null)
                SoundEffectsManager.Instance.Play("GameOver");
        }
        else
        {
            Debug.LogWarning("[GameController] GameOver screen not assigned.");
        }
    }

    public void ResetGame()
    {
        Time.timeScale = 1f;
        if (gameOverScreen != null) gameOverScreen.SetActive(false);
        SceneManager.LoadScene(firstLevelSceneIndex);
        OnReset?.Invoke();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Gems.OnGemCollect -= IncreaseProgressAmount;
        PlayerHealth.OnPlayerDied -= ShowGameOverScreen;
    }
}
