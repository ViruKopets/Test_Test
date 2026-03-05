using UnityEngine;
using UnityEngine.UI;

public class PickupItem : MonoBehaviour
{
    [SerializeField] Inventory Invent;
    [SerializeField] Sprite MyPic;
    [SerializeField] string MyPurpose;
    public void PickUp()
    {
        if (Invent == null)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag("Inventory");
            Invent = objects[0].GetComponent<Inventory>();
        }
        Invent.TakeNewItem(MyPic, MyPurpose, this);
    }

    public void ItemTaken()
    {
        Destroy(this.gameObject);
    }
}
