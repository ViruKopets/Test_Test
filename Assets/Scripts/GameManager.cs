using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] List<bool> Progress;
    [SerializeField] List<bool> ItemProgress;
    int GoesTo;
    bool GoodEnding;
    public int PicIndex;
    public int PicIndex2;
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
            bool GoneFurther = (Progress[1] || Progress[2] || Progress[3] || Progress[4] || Progress[5]);
            if (Progress[0] && !GoneFurther)
            {
                Boot.TurnOnById(0);
            }
            if (Progress[6])
            {
                Boot.TurnOffById(0);
                Boot.TurnOnById(1);
            }
        }
        else if (SceneName == "3Forest")
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
            if (Progress[1])
            {
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
            if (ItemProgress[1])
            {
                Boot.TurnOffItems();
                Boot.TurnOnById(0);
            }
        }
        else if (SceneName == "5Bar")
        {

        }
        else if (SceneName == "6GranHouse")
        {
            if (!ItemProgress[2])
            {
                Boot.TurnOnById(0);
                Boot.TurnOnDialog(0);
            }
            else
            {
                Boot.TurnOnById(1);
                Boot.TurnOnDialog(1);
            }
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

    public bool CheckProgress(int Index)
    {
        if (Index == 1)
        {
            bool GoneFurther = (Progress[1] || Progress[2] || Progress[3] || Progress[4] || Progress[5]);
            return GoneFurther;
        }
        else
        {
            return Progress[Index];
        }
    }

    public bool IsGoodEnding()
    {
        return GoodEnding;
    }
}
