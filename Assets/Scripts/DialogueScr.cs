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
    [SerializeField] float TimeToLetter = 0.1f;
    [SerializeField] bool ActivateOnStart = false;
    [SerializeField] bool ActivateOnTrigger = false;
    [SerializeField] List<GameObject> ObjToOff;
    [SerializeField] List<GameObject> ObjToOn;
    [SerializeField] GameObject Invent;
    [SerializeField] Player Pla;
    [SerializeField] Animator MommyAnimator;

    bool Active;
    int Index = 0;
    Coroutine WordAppCor;

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
        WordPlace.text = "";
        //WordPlace.text = Words[Index];
        NamePlace.text = Names[Index];
        WordAppCor = StartCoroutine(WordAppear(0));
        Index = Index + 1;
        Active = true;
        Invent.SetActive(false);
        Pla.IsFreezed(true);
    }

    private void Update()
    {
        if (!Active) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (WordAppCor != null)
            {
                StopCoroutine(WordAppCor);
                WordAppCor = null;
                WordPlace.text = Words[Index - 1];
                return;
            }
            if (Index == Imgs.Count)
            {
                DialogueDone();
                return;
            }
            ImgPlace.sprite = Imgs[Index];
            NamePlace.text = Names[Index];
            WordPlace.text = "";
            WordAppCor = StartCoroutine(WordAppear(Index));
            Index = Index + 1;
        }
    }

    void DialogueDone()
    {
        Index = 0;
        Active = false;
        DialoguePanel.SetActive(false);
        if (ObjToOff != null)
        {
            for (int i = 0; i < ObjToOff.Count; i++)
            {
                ObjToOff[i].SetActive(false);
            }
            
        }
        if (ObjToOn != null)
        {
            for (int i = 0; i < ObjToOn.Count; i++)
            {
                ObjToOn[i].SetActive(true);
            }
        }
        Invent.SetActive(true);
        Pla.IsFreezed(false);
        if (MommyAnimator != null)
        {
            MommyAnimator.SetTrigger("Leave");
        }
    }
    IEnumerator WordAppear(int Ind)
    {
        char[] letters = Words[Ind].ToCharArray();
        for (int i = 0; i < letters.Length; i++)
        {
            WordPlace.text += letters[i];
            yield return new WaitForSeconds(TimeToLetter);
        }
        WordAppCor = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (ActivateOnTrigger)
            ActivateDialogue();
    }
}
