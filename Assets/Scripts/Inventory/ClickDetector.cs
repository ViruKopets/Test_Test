using System.Collections.Generic;
using UnityEngine;

public class ClickDetector : MonoBehaviour
{
    [SerializeField] Camera Cam;
    [SerializeField] List<InventItem> Slots;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ClickCheck();
        }
    }

    void ClickCheck()
    {
        if (Cam == null)
        {
            Cam = Camera.main;
            Slots[0].SetNewSceneCam();
            Slots[1].SetNewSceneCam();
            Slots[2].SetNewSceneCam();
            Slots[3].SetNewSceneCam();
        }
        Vector2 mousePosition = Cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Collectable"))
            {
                hit.collider.gameObject.GetComponent<PickupItem>().PickUp();
            }
        }
    }
}
