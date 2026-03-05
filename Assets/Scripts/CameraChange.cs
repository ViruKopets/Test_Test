using UnityEngine;

public class CameraChange : MonoBehaviour
{
    [SerializeField] Camera Cam;
    [SerializeField] Transform PosToBe;
    [SerializeField] bool lockCamera;
    [SerializeField] Player Pla;
    [SerializeField] bool Freeze;
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
            if (hit.collider.CompareTag("ChangeCam"))
            {
                Cam.transform.position = PosToBe.position;
                Pla.IsFreezed(Freeze);
            }
        }
    }
}
