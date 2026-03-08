using UnityEngine;

public class FlipAPic : MonoBehaviour
{
    [SerializeField] Camera Cam;
    [SerializeField] DialogueScr AfterDialogue;
    [SerializeField] Transform Target;

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
            if (hit.collider.CompareTag("Pic"))
            {
                //Animation or Something
                AfterSeeingPic();
            }
        }
    }

    void AfterSeeingPic()
    {
        Cam.transform.position = Target.position;
        AfterDialogue.ActivateDialogue();
    } 
}
