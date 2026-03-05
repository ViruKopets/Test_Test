using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] List<Image> ImgPlaces;
    [SerializeField] List<bool> Taken;
    [SerializeField] List<string> ItemPurp;
    [SerializeField] List<InventItem> Items;
    [SerializeField] Sprite EmptySlot;
    [SerializeField] GameObject Layout;

    private void Awake()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Inventory");
        if (objects.Length > 1)
        {
            // ≈сли нашли больше одного - уничтожаем этот
            Destroy(gameObject);
            Destroy(Layout);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            DontDestroyOnLoad(Layout);
        }
    }
    public void TakeNewItem(Sprite NewPic, string Purpose, PickupItem Item)
    {
        for (int i = 0; i < Taken.Count; i++)
        {
            if (!Taken[i])
            {
                ImgPlaces[i].sprite = NewPic;
                ItemPurp[i] = Purpose;
                Taken[i] = true;
                Items[i].TakeInfo();
                Item.ItemTaken();
                break;
            }
        }
    }

    public void CompareCompat(string Lock, int Id)
    {
        if (Lock == ItemPurp[Id])
        {
            Items[Id].Used();
            Taken[Id] = false;
            ImgPlaces[Id].sprite = EmptySlot;
        }
    }
}
