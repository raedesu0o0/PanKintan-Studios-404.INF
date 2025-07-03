using UnityEngine;
using UnityEngine.UI;


public class SoundEffectsManager : MonoBehaviour
{
    private static SoundEffectsManager Instance;
    private static AudioSource audioSource;
    private static SoundEffectLib soundEffectLib;

    [SerializeField] private Slider sfxSlider;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
            soundEffectLib = GetComponent<SoundEffectLib>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public static void Play(string soundName)
    {
        AudioClip audioClip = soundEffectLib.GetRandomClip(soundName);
        if (audioClip != null)
        {
            audioSource.PlayOneShot(audioClip);
        }
    }

    void Start()
    {
        sfxSlider.onValueChanged.AddListener(delegate { OnValueChanged(); });
    }

    public static void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public void OnValueChanged()
    {
        SetVolume(sfxSlider.value);
    }
}