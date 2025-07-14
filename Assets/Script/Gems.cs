using UnityEngine;
using System;

public class Gems : MonoBehaviour
{
    public static event Action<int> OnGemCollect;

    [SerializeField] private int gemValue = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnGemCollect?.Invoke(gemValue);
            Destroy(gameObject);
        }
    }
}

