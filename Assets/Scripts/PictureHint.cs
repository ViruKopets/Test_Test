using UnityEngine;

public class PictureHint : MonoBehaviour
{
    [Header("Dialogue to show once on the first click in this scene")]
    [SerializeField] DialogueScr HintDialogue;

    bool shown;

    void Update()
    {
        if (shown) return;
        if (!Input.GetMouseButtonDown(0)) return;
        if (HintDialogue == null) return;

        HintDialogue.ActivateDialogue();
        shown = true;
    }
}
