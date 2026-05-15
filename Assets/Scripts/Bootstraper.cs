using System.Collections.Generic;
using UnityEngine;

public class Bootstraper : MonoBehaviour
{
    [SerializeField] List<DialogueScr> Dialogs;
    [SerializeField] List<GameObject> ObjToOff;
    [SerializeField] List<GameObject> ObjToOn;
    [SerializeField] List<Transform> ItemObjToOff;
    [SerializeField] List<Transform> PlayersPos;
    [SerializeField] bool IsSity;
    [SerializeField] string SceneName;
    [SerializeField] GameObject Player;
    [SerializeField] MiniGameScr MiniGame;
    Vector3 PosOut = new Vector3(0, -1000, 0);
    GameManager Gm;

    public void Start()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("GameManager");
        if (objects.Length == 0) return;
        Gm = objects[0].GetComponent<GameManager>();
        Gm.TakeInfo(this, SceneName);
    }

    public void SetPlayerPos(int PosId)
    {
        Player.transform.position = PlayersPos[PosId].position;
        if (PosId == 2)
        {
            Dialogs[0].ActivateDialogue();
        }
        if (Gm.CheckProgress(6)&&PosId==4)
        {
            Dialogs[1].ActivateDialogue();
        }
    }

    public void TurnOnDialog(int Index)
    {
        Dialogs[Index].ActivateDialogue();
    }

    public void TurnOffProgress()
    {
        for (int i = 0; i < ObjToOff.Count; i++)
        {
            ObjToOff[i].SetActive(false);
        }
    }

    public void TurnOffItems()
    {
        for (int i = 0; i < ItemObjToOff.Count; i++)
        {
            ItemObjToOff[i].transform.position = PosOut;
            if (MiniGame != null)
            {
                MiniGame.SetAllDialogsNotKick();
            }
        }
    }

    public void TurnOnObjByProgress()
    {
        for (int i = 0; i < ObjToOn.Count; i++)
        {
            ObjToOn[i].SetActive(true);
        }
    }

    public void TurnOnById(int Index)
    {
        ObjToOn[Index].SetActive(true);
    }

    public void TurnOffById(int Index)
    {
        ObjToOff[Index].SetActive(false);

    }
}
