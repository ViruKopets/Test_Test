using UnityEngine;

public class HideDuringDialogue : MonoBehaviour
{
    [Tooltip("UI element to hide while any DialogueScr is active.")]
    [SerializeField] GameObject target;

    DialogueScr[] _dialogues;
    bool _lastVisible = true;

    void Start()
    {
        _dialogues = FindObjectsByType<DialogueScr>(FindObjectsSortMode.None);
        Apply(true);
    }

    void Update()
    {
        if (target == null) return;
        bool anyActive = false;
        for (int i = 0; i < _dialogues.Length; i++)
        {
            var d = _dialogues[i];
            if (d != null && d.IsDialogueActive()) { anyActive = true; break; }
        }
        Apply(!anyActive);
    }

    void Apply(bool visible)
    {
        if (visible == _lastVisible) return;
        _lastVisible = visible;
        target.SetActive(visible);
    }
}
