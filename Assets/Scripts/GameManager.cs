using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] List<bool> Progress;
    [SerializeField] List<bool> ItemProgress;
    int GoesTo;
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
}
