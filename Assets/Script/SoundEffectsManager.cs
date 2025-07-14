using UnityEngine;
using UnityEngine.UI;

public class SoundEffectsManager : MonoBehaviour
{
    public static SoundEffectsManager Instance { get; private set; }

    private AudioSource audioSource;
    private SoundEffectLib soundEffectLib;

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

    // ✅ Make this an instance method (remove "static")
    public void Play(string soundName)
    {
        if (soundEffectLib == null || audioSource == null)
        {
            Debug.LogWarning("[SoundEffectsManager] Missing components.");
            return;
        }

        AudioClip audioClip = soundEffectLib.GetRandomClip(soundName);
        if (audioClip != null)
        {
            audioSource.PlayOneShot(audioClip);
        }
    }

    private void Start()
    {
        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(delegate { OnValueChanged(); });
    }

    // ✅ Also made non-static
    public void SetVolume(float volume)
    {
        if (audioSource != null)
            audioSource.volume = volume;
    }

    public void OnValueChanged()
    {
        SetVolume(sfxSlider.value);
    }
}
