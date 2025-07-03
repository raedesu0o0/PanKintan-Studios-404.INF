
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectsLib : MonoBehaviour
{
    [SerializeField] private SoundEffectGroup[] soundEffectGroups;
    private Dictionary<string, List<AudioClip>> soundDictionary;

    private void Awake()
    {
        InitializeSoundDictionary();
    }

    private void InitializeSoundDictionary()
    {
        soundDictionary = new Dictionary<string, List<AudioClip>>();
        foreach (var group in soundEffectGroups)
        {
            if (!soundDictionary.ContainsKey(group.name))
            {
                soundDictionary[group.name] = new List<AudioClip>();
            }
            soundDictionary[group.name].AddRange(group.audioClips);
        }
    }
    
    public AudioClip GetRandomSound(string groupName)
    {
        if (soundDictionary.ContainsKey(groupName) && soundDictionary[groupName].Count > 0)
        {
            var clips = soundDictionary[groupName];
            return clips[Random.Range(0, clips.Count)];
        }
        Debug.LogWarning($"Sound group '{groupName}' not found or has no audio clips.");
        return null;
    }
}

[System.Serializable]
public struct SoundEffectGroup
{
    public string name;
    public List<AudioClip> audioClips;
}
