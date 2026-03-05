using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueScr : MonoBehaviour
{
    [Header("Places")]
    [SerializeField] Image ImgPlace;
    [SerializeField] TMP_Text WordPlace;
    [SerializeField] TMP_Text NamePlace;
    [SerializeField] GameObject DialoguePanel;

    [Header("DialogSettings")]
    [SerializeField] List<Sprite> Imgs;
    [SerializeField] List<string> Words;
    [SerializeField] List<string> Names;

    [Header("Other")]
    [SerializeField] float TimeToLetter;
    [SerializeField] bool ActivateOnStart;
    [SerializeField] GameObject ObjToOff;
    [SerializeField] List<GameObject> ObjToOn;

    bool Active;
    int Index = 0;
    private void Start()
    {
        if (ActivateOnStart)
        {
            ActivateDialogue();
        }
    }

    void ActivateDialogue()
    {
        DialoguePanel.SetActive(true);
        ImgPlace.sprite = Imgs[Index];
        //WordPlace.text = " ";
        WordPlace.text = Words[Index];
        NamePlace.text = Names[Index];
        Index = Index + 1;
        Active = true;
    }

    private void Update()
    {
        if (!Active) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (Index == Imgs.Count)
            {
                Index = 0;
                Active = false;
                DialoguePanel.SetActive(false);
                if (ObjToOff != null) ObjToOff.SetActive(false);
                if (ObjToOn != null)
                {
                    for (int i = 0; i < ObjToOn.Count; i++)
                    {
                        ObjToOn[i].SetActive(true);
                    }
                }
                return;
            }
            ImgPlace.sprite = Imgs[Index];
            WordPlace.text = Words[Index];
            NamePlace.text = Names[Index];
            Index = Index + 1;
        }
    }

    IEnumerator WordAppear()
    {
        return null;
    }
}
