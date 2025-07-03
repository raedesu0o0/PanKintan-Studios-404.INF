using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    private static MusicManager Instance;
    private AudioSource audioSource;

    public AudioClip backgroundMusic;

    [SerializeField] private Slider musicSlider;

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

    // Start is called before the first frame update
    void Start()
    {
        if (backgroundMusic != null)
        {
            PlayBackgroundMusic(false, backgroundMusic);
        }

        if (musicSlider != null)
        {
            musicSlider.onValueChanged.AddListener(delegate { OnSliderValueChanged(); });
        }
    }

    public static void PlayBackgroundMusic(bool resetSong, AudioClip audioClip = null)
    {
        if (audioClip != null)
        {
            audioSource.clip = audioClip;
        }
        else if (audioSource.clip != null)
        {
            if (resetSong)
            {
                Instance audioSource.Stop();
            }
        }

        audioSource.Play();
    }

    public static void PauseBackgroundMusic()
    {
        Instance audioSource.Pause();
    }

    public void ResumeBackgroundMusic()
    {
        Instance audioSource.UnPause();
    }

    public void SetVolume(float volume)
    {
        Instance audioSource.volume = volume;
    }

    public void OnSliderValueChanged()
    {
        SetVolume(musicSlider.value);
    }
}
