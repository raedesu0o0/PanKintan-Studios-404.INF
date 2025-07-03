using UnityEngine;
using System;


public class Gems : MonoBehaviour, IITEM
{
    public static event Action<int> OnGemCollect;
    public int worth = 5;
    public void Collect()
    {
        OnGemCollect.Invoke(worth);
        //SoundEffectsManager.Play("Memory Shards");
        Destroy(gameObject);
    }
}
