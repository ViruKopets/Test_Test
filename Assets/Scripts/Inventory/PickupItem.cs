using UnityEngine;
using UnityEngine.UI;

public class PickupItem : MonoBehaviour
{
    [SerializeField] Inventory Invent;
    [SerializeField] Sprite MyPic;
    [SerializeField] string MyPurpose;
    public void PickUp()
    {
        Invent.TakeNewItem(MyPic, MyPurpose, this);
    }

    public void ItemTaken()
    {
        Destroy(this.gameObject);
    }
}
