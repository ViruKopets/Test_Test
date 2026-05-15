using UnityEngine;

public class ClearInventory : MonoBehaviour
{
    [SerializeField] Inventory Invent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Invent == null)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag("Inventory");
            if (objects.Length == 0) return;
            Invent = objects[0].GetComponent<Inventory>();
            if (Invent == null) return;
        }
        Invent.ClearItems();
    }
}
