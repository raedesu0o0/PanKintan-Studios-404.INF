using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    private static MusicManager Instance;
    private AudioSource audioSource;

    public AudioClip backgroundMusic;

    [SerializeField] private string musicSliderTag = "MusicSlider"; // Add this: assign tag to slider
    private Slider musicSlider;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (backgroundMusic != null)
        {
            PlayBackgroundMusic(false, backgroundMusic);
        }

        FindAndAssignSlider();
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        FindAndAssignSlider();
    }

    private void FindAndAssignSlider()
    {
        GameObject sliderObject = GameObject.FindGameObjectWithTag(musicSliderTag);
        if (sliderObject != null)
        {
            musicSlider = sliderObject.GetComponent<Slider>();
            musicSlider.onValueChanged.RemoveAllListeners();
            musicSlider.onValueChanged.AddListener(delegate { OnSliderValueChanged(); });
            musicSlider.value = audioSource.volume;
        }
        else
        {
            Debug.LogWarning("[MusicManager] No music slider found in scene with tag: " + musicSliderTag);
        }
    }

    public static void PlayBackgroundMusic(bool resetSong, AudioClip audioClip = null)
    {
        if (Instance == null) return;

        if (audioClip != null)
        {
            Instance.audioSource.clip = audioClip;
        }
        else if (resetSong && Instance.audioSource.clip != null)
        {
            Instance.audioSource.Stop();
        }

        if (!Instance.audioSource.isPlaying)
            Instance.audioSource.Play();
    }

    public static void PauseBackgroundMusic()
    {
        if (Instance == null) return;
        Instance.audioSource.Pause();
    }

    public static void ResumeBackgroundMusic()
    {
        if (Instance == null) return;
        Instance.audioSource.UnPause();
    }

    public static void SetVolume(float volume)
    {
        if (Instance != null && Instance.audioSource != null)
        {
            Instance.audioSource.volume = volume;
        }
    }

    public static float GetVolume()
    {
        return Instance != null ? Instance.audioSource.volume : 1f;
    }

    private void OnSliderValueChanged()
    {
        SetVolume(musicSlider.value);
    }
}
