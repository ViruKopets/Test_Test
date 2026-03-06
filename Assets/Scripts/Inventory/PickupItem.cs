using UnityEngine;
using UnityEngine.UI;

public class PickupItem : MonoBehaviour
{
    [SerializeField] Inventory Invent;
    [SerializeField] Sprite MyPic;
    [SerializeField] string MyPurpose;
    [SerializeField] GameObject BackArrow;
    [SerializeField] bool IsProgressable;
    [SerializeField] int ItemProgressId;
    public void PickUp()
    {
        if (Invent == null)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag("Inventory");
            Invent = objects[0].GetComponent<Inventory>();
        }
        Invent.TakeNewItem(MyPic, MyPurpose, this);
        if (BackArrow != null) BackArrow.SetActive(true);
    }

    public void ItemTaken()
    {
        if (IsProgressable)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag("GameManager");
            objects[0].GetComponent<GameManager>().ProgressedItem(ItemProgressId);
        }
        Destroy(this.gameObject);
    }
}
