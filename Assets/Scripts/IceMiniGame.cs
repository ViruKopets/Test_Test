using System.Collections.Generic;
using UnityEngine;

public class IceMiniGame : MonoBehaviour
{
    [SerializeField] Camera Cam;
    [SerializeField] List<GameObject> Cracs;
    [SerializeField] GameObject FinalCrack;
    [SerializeField] GameObject Compas;

    int Counter;

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
            if (hit.collider.CompareTag("Ice"))
            {
                if (Counter == Cracs.Count)
                {
                    Debug.Log("out");
                    this.enabled = false;
                    return;
                }
                Cracs[Counter].transform.position = mousePosition;
                Counter++;
                if (Counter == Cracs.Count)
                {
                    for (int i = 0; i < Cracs.Count; i++)
                    {
                        Destroy(Cracs[i]);
                    }
                    FinalCrack.SetActive(true);
                    Compas.SetActive(true);
                }
            }
        }
    }
}
