using UnityEngine;

public class Collector : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IITEM item = collision.GetComponent<IITEM>();
        if(item != null)
        {
            item.Collect();
        }
    }
}
