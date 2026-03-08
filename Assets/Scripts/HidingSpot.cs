using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    [SerializeField] Camera Cam;
    [SerializeField] DialogueScr CantHideDialogue;
    [SerializeField] DialogueScr AbleHideDialogue;
    [SerializeField] GameObject TurnOnHide;
    [SerializeField] Player player;
    [SerializeField] public bool CanHide;

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
            if (hit.collider.CompareTag("HidingSpot"))
            {

                CheckOnClick();
            }
        }
    }

    public void CheckOnClick()
    {
        if (!CanHide)
        {
            CantHideDialogue.ActivateDialogue();
        }
        else
        {
            TurnOnHide.SetActive(true);
            player.Hide(true);
            AbleHideDialogue.ActivateDialogue();
        }
    }
}
