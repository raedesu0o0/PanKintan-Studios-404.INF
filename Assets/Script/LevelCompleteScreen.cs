
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteMenu : MonoBehaviour
{
    [SerializeField] GameObject levelCompleteMenu;

    public static LevelCompleteMenu Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }

        Instance = this;

        if (levelCompleteMenu != null)
            levelCompleteMenu.SetActive(false); // Hide menu at start
        else
            Debug.LogWarning("[LevelCompleteMenu] 'levelCompleteMenu' is not assigned!");
    }

    public void ShowLevelComplete()
    {
        if (levelCompleteMenu != null)
        {
            levelCompleteMenu.SetActive(true);
            Time.timeScale = 0f;
            Debug.Log("[LevelCompleteMenu] Shown successfully.");
        }
        else
        {
            Debug.LogError("[LevelCompleteMenu] Cannot show: 'levelCompleteMenu' is null!");
        }
        SoundEffectsManager.Play("LevelComplete");
    }

    public static void Show()
    {
        if (Instance != null)
        {
            Instance.ShowLevelComplete();
        }
        else
        {
            Debug.LogError("[LevelCompleteMenu] Instance is null. Make sure it's in the scene.");
        }
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            SceneManager.LoadScene("StartScene"); // Fallback or game complete
        }
    }
}
