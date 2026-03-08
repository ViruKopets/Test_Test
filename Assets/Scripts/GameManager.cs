using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] List<bool> Progress;
    [SerializeField] List<bool> ItemProgress;
    int GoesTo;
    bool GoodEnding;
    //public bool SomeDone;

    private void Awake()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("GameManager");
        if (objects.Length > 1)
        {
            Destroy(objects[1]);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void SetTransitionInfo(int PosId)
    {
        GoesTo = PosId;
    }

    public void TakeInfo(Bootstraper Boot, string SceneName)
    {
        if (SceneName == "2City")
        {
            Boot.SetPlayerPos(GoesTo);
            if (Progress[0])
            {
                Boot.TurnOnObjByProgress();
            }
        }
        else if(SceneName == "3Forest")
        {
            if (Progress[0])
            {
                Boot.TurnOffProgress();
            }
            if (ItemProgress[0])
            {
                Boot.TurnOffItems();
            }
        }
        else if (SceneName == "4Hostel")
        {
            if (ItemProgress[1])
            {
                Boot.TurnOffItems();
            }
            if (Progress[1])
            {
                Boot.TurnOnById(0);
                Boot.TurnOnById(1);
                Boot.TurnOffById(0);
            }
            if (Progress[2])
            {
                Boot.TurnOnById(2);
                Boot.TurnOffById(1);
            }
            if (Progress[3])
            {
                Boot.TurnOnById(3);
                Boot.TurnOffById(2);
            }
            if (Progress[4])
            {
                Boot.TurnOnById(4);
                Boot.TurnOffById(3);
            }
            if (Progress[5])
            {
                Boot.TurnOnById(5);
                Boot.TurnOffById(4);
            }
        }
        else if (SceneName == "5Bar")
        {

        }
        else if (SceneName == "6GranHouse")
        {

        }
    }

    public void Progressed(int ProgressId)
    {
        Progress[ProgressId] = true;
    }

    public void ProgressedItem(int ItemProgressId)
    {
        ItemProgress[ItemProgressId] = true;
    }

    public void SetEnding(bool IsGood)
    {
        GoodEnding = IsGood;
    }
}
