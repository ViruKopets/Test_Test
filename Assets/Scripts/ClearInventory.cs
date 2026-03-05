using UnityEngine;

public class ClearInventory : MonoBehaviour
{
    [SerializeField] Inventory Invent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Invent.ClearItems();
    }
}
