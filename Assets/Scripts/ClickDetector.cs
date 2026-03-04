using UnityEngine;

public class ClickDetector : MonoBehaviour
{
    [SerializeField] Camera Cam;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ClickCheck();
        }
    }

    void ClickCheck()
    {
        Vector2 mousePosition = Cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            Debug.Log(hit.collider.gameObject);
            if (hit.collider.CompareTag("Collectable"))
            {
                hit.collider.gameObject.GetComponent<PickupItem>().PickUp();
            }
        }
    }
}
