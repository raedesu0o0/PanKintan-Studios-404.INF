using UnityEngine;

public class Collector : MonoBehaviour
{
    private int totalGems = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IITEM item = collision.GetComponent<IITEM>();
        if (item != null)
        {
            item.Collect();
        }
    }

    public void AddGems(int amount)
    {
        totalGems += amount;
        Debug.Log($"[Collector] Collected {amount} gems. Total: {totalGems}");
    }
}
