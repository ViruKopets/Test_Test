using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.Rendering.VolumeComponent;

public class InventItem : MonoBehaviour, IDragHandler, IDropHandler
{
    [SerializeField] int MyId;
    [SerializeField] Inventory Invent;
    [SerializeField] Canvas canvas;
    [SerializeField] RectTransform MyRect;
    [SerializeField] Camera Cam;
    [SerializeField] Vector2 DefaulPos;
    [SerializeField] bool IsSomething = false;
    [SerializeField] Chest LastItem;

    public void SetNewSceneCam()
    {
        Cam = Camera.main;
    }
    //void Start()
    //{
    //    DefaulPos = MyRect.anchoredPosition;
    //}

    public void OnDrag(PointerEventData eventData)
    {
        if (IsSomething)
        {
            MyRect.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!IsSomething) return;

        Vector2 mousePosition = Cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("InterByItem"))
            {
                LastItem = hit.collider.GetComponent<Chest>();
                Invent.CompareCompat(LastItem.PassInfo(), MyId);
                MyRect.anchoredPosition = DefaulPos;
            }
            else
            {
                MyRect.anchoredPosition = DefaulPos;
            }
        }
        else
        {
            MyRect.anchoredPosition = DefaulPos;
        }
    }

    public void TakeInfo()
    {
        IsSomething = true;
    }

    public void Used()
    {
        LastItem.ItemFit();
        IsSomething = false;
    }
    public void Clear()
    {

    }
}
