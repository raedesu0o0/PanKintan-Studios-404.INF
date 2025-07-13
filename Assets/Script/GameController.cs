using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class GameController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Slider progressSlider;
    [SerializeField] private GameObject loadCanvas;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private HealthUI healthUIPrefab;

    [Header("Gameplay")]
    public GameObject playerPrefab;
    public int firstLevelSceneIndex = 2;
    public int lastLevelSceneIndex = 4;
    
    private int currentLevelIndex;
    private int progressAmount;
    private HealthUI healthUIInstance;
    private GameObject currentPlayer;

    public static event Action OnReset;

    private void Awake()
    {
        // Singleton pattern
        var existing = FindAnyObjectByType<GameController>();
        if (existing != null && existing != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        InitializePersistentUI();
    }

    private void Start()
    {
        InitializeGameState();
        SubscribeToEvents();
    }

    private void InitializeGameState()
    {
        Time.timeScale = 1f;
        progressAmount = 0;
        if (progressSlider != null) progressSlider.value = 0;
        if (loadCanvas != null) loadCanvas.SetActive(false);
        if (gameOverScreen != null) gameOverScreen.SetActive(false);

        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.sceneLoaded += OnSceneLoaded;

        SpawnPlayer();
    }

    private void SubscribeToEvents()
    {
        Gems.OnGemCollect += IncreaseProgressAmount;
        HoldToLoadLevel.OnHoldComplete += LoadNextLevel;
        PlayerHealth.OnPlayerDied += ShowGameOverScreen;
    }

    private void InitializePersistentUI()
    {
        // Initialize health UI
        healthUIInstance = FindAnyObjectByType<HealthUI>();
        if (healthUIInstance == null && healthUIPrefab != null)
        {
            healthUIInstance = Instantiate(healthUIPrefab);
            DontDestroyOnLoad(healthUIInstance.gameObject);
        }

        ReparentUIToMainCanvas();
    }

    private void ReparentUIToMainCanvas()
    {
        Canvas mainCanvas = FindAnyObjectByType<Canvas>();
        if (mainCanvas == null) return;

        if (progressSlider != null) 
        {
            progressSlider.transform.SetParent(mainCanvas.transform, false);
            progressSlider.gameObject.SetActive(true);
        }
        
        if (loadCanvas != null) 
        {
            loadCanvas.transform.SetParent(mainCanvas.transform, false);
            loadCanvas.SetActive(false);
        }
        
        if (gameOverScreen != null) 
        {
            gameOverScreen.transform.SetParent(mainCanvas.transform, false);
            gameOverScreen.SetActive(false);
        }
        
        if (healthUIInstance != null) 
        {
            healthUIInstance.transform.SetParent(mainCanvas.transform, false);
            healthUIInstance.gameObject.SetActive(true);
        }
    }

    private void SpawnPlayer()
    {
        if (currentPlayer != null) return;
        if (playerPrefab == null) return;

        currentPlayer = Instantiate(playerPrefab);
        var playerHealth = currentPlayer.GetComponent<PlayerHealth>();
        if (playerHealth != null && healthUIInstance != null)
        {
            playerHealth.healthUI = healthUIInstance;
            playerHealth.ResetHealth();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentLevelIndex = scene.buildIndex;
        ReparentUIToMainCanvas();
        
        if (progressSlider != null) 
        {
            progressSlider.value = progressAmount;
        }

        SpawnPlayer();
    }

    private void ShowGameOverScreen()
    {
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
            Time.timeScale = 0f;
            SoundManager.PlaySound("GameOver");
        }
    }

    public void ResetGame()
    {
        if (gameOverScreen != null) gameOverScreen.SetActive(false);
        Time.timeScale = 1f;

        currentLevelIndex = firstLevelSceneIndex;
        progressAmount = 0;
        if (progressSlider != null) progressSlider.value = 0;
        
        SceneManager.LoadScene(currentLevelIndex);
        OnReset?.Invoke();
    }

    private void IncreaseProgressAmount(int amount)
    {
        if (progressSlider == null) return;

        progressAmount = Mathf.Clamp(progressAmount + amount, 0, 100);
        progressSlider.value = progressAmount;

        if (progressAmount >= 100 && loadCanvas != null)
        {
            loadCanvas.SetActive(true);
        }
    }

    private void LoadNextLevel()
    {
        int nextIndex = currentLevelIndex + 1;
        if (nextIndex > lastLevelSceneIndex)
            nextIndex = firstLevelSceneIndex;

        currentLevelIndex = nextIndex;
        progressAmount = 0;
        if (progressSlider != null) progressSlider.value = 0;
        if (loadCanvas != null) loadCanvas.SetActive(false);

        SceneManager.LoadScene(currentLevelIndex);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Gems.OnGemCollect -= IncreaseProgressAmount;
        HoldToLoadLevel.OnHoldComplete -= LoadNextLevel;
        PlayerHealth.OnPlayerDied -= ShowGameOverScreen;

        // Clean up health UI if this is the last GameController
        if (healthUIInstance != null && FindAnyObjectByType<GameController>() == null)
        {
            Destroy(healthUIInstance.gameObject);
        }
    }
}