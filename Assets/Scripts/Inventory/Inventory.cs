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
    [SerializeField] GameObject Canvasik;
    [SerializeField] GameObject Visuals;

    private void Awake()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Inventory");
        if (objects.Length > 1)
        {
            Destroy(Canvasik);
        }
        else
        {
            DontDestroyOnLoad(Canvasik);
        }
    }
    public void TakeNewItem(Sprite NewPic, string Purpose, PickupItem Item, bool IsBrakeOnUse)
    {
        for (int i = 0; i < Taken.Count; i++)
        {
            if (!Taken[i])
            {
                ImgPlaces[i].sprite = NewPic;
                ItemPurp[i] = Purpose;
                Taken[i] = true;
                Items[i].TakeInfo(IsBrakeOnUse);
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
            if (Items[Id].DestroyItem)
            {
                Taken[Id] = false;
                ImgPlaces[Id].sprite = EmptySlot;
            }
        }
    }

    public void ClearItems()
    {
        for (int i = 0; i < Items.Count; i++)
        {
            Taken[i] = false;
            ImgPlaces[i].sprite = EmptySlot;
            Items[i].Clear();
        }
    }

    public void SetVisuals(bool IsActive)
    {
        Visuals.SetActive(IsActive);
    }
}
