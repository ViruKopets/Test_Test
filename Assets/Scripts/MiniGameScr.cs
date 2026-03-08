using System.Collections.Generic;
using UnityEngine;

public class MiniGameScr : MonoBehaviour
{
    [SerializeField] Camera Cam;
    [SerializeField] List<GameObject> Pics;
    [SerializeField] List<Transform> PicsDefPos;
    [SerializeField] Transform FPlace;
    [SerializeField] Transform SPlace;
    [SerializeField] int PickedIndex1 = -1;
    [SerializeField] int PickedIndex2 = -1;
    [SerializeField] DialogueScr UselessDialogue;
    [SerializeField] DialogueScr BadDialogue;
    [SerializeField] DialogueScr GoodDialogue;
    GameManager Gm;

    private void Start()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("GameManager");
        Gm = objects[0].GetComponent<GameManager>();
    }

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
            if (hit.collider.CompareTag("MagazinePic"))
            {
                Checks(hit.collider.gameObject);
                if (PickedIndex1 != -1 && PickedIndex2 != -1)
                {
                    DialogueCheck();
                }
            }
        }
    }

    void Checks(GameObject Pic)
    {

        for (int i = 0; i < Pics.Count; i++)
        {
            if (Pics[i] == Pic)
            {
                if (i == PickedIndex1)
                {
                    Pic.transform.position = PicsDefPos[i].position;
                    PickedIndex1 = -1;
                    return;
                }
                else if (i == PickedIndex2)
                {
                    Pic.transform.position = PicsDefPos[i].position;
                    PickedIndex2 = -1;
                    return;
                }
                if (PickedIndex1 == -1)
                {
                    Pic.transform.position = FPlace.position;
                    PickedIndex1 = i;
                }
                else if (PickedIndex2 == -1)
                {
                    Pic.transform.position = SPlace.position;
                    PickedIndex2 = i;
                }
                return;
            }
        }
    }

    void DialogueCheck()
    {
        if (PickedIndex1 == 0 || PickedIndex2 == 0)
        {
            UselessDialogue.ActivateDialogue();
            Gm.SetEnding(false);
            return;
        }
        bool goodEnding = (PickedIndex1 == 2 || PickedIndex1 == 3 || PickedIndex1 == 4) && (PickedIndex2 == 2 || PickedIndex2 == 3 || PickedIndex2 == 4);
        if (goodEnding)
        {
            GoodDialogue.ActivateDialogue();
            Gm.SetEnding(true);
        }
        else
        {
            BadDialogue.ActivateDialogue();
            Gm.SetEnding(false);
        }
    }
}
