using UnityEngine;

public class CameraChange : MonoBehaviour
{
    [SerializeField] Camera Cam;
    [SerializeField] Transform PosToBe;
    [SerializeField] bool lockCamera;
    [SerializeField] Player Pla;
    [SerializeField] bool Freeze;
    [SerializeField] GameObject DialoguePan;
    [SerializeField] Camerafollower CamFollow;
    [SerializeField] GameObject ToActivate;
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
            if (DialoguePan.activeSelf)
            {
                Debug.Log("asd");
                return;
            }
            if (hit.collider.CompareTag("ChangeCam"))
            {
                if (CamFollow != null)
                {
                    if (lockCamera)
                    {
                        CamFollow.enabled = false;
                    }
                    else
                    {
                        CamFollow.enabled = true;
                    }
                }
                Cam.transform.position = PosToBe.position;
                Pla.IsFreezed(Freeze);
                if (ToActivate != null)
                {
                    ToActivate.SetActive(true);
                }
            }
            else if (hit.collider.CompareTag("LoadScene"))
            {
                hit.collider.gameObject.GetComponent<SceneLoader>().LoadScene();
            }
            else if (hit.collider.CompareTag("DialogueActivate"))
            {
                hit.collider.gameObject.GetComponent<DialogueScr>().ActivateDialogue();
            }
        }
    }
}
