using System;
using UnityEngine;
using UnityEngine.EventSystems;

public enum IcePuzzleState
{
    Initial,
    HintShown,
    Solved,
    Failed
}

public class IcePuzzleManager : MonoBehaviour
{
    [Header("Config & subsystems")]
    [SerializeField] IcePuzzleConfig config;
    [SerializeField] CompassConditionSystem compass;
    [SerializeField] CrackVisualController cracks;
    [SerializeField] WeakPointDetector weakPoints;
    [SerializeField] InvestigationModeController investigation;

    [Header("Scene refs")]
    [SerializeField] Camera puzzleCam;
    [Tooltip("Collider2D of the ice surface. If empty, falls back to objects tagged 'Ice'.")]
    [SerializeField] Collider2D iceCollider;
    [SerializeField] GameObject compasObject;       // Collectable Compas, hidden until extracted
    [SerializeField] GameObject finalCrackVisual;   // FinalCrack sprite

    [Header("UI")]
    [Tooltip("Compass condition bar. Hidden until the first click on the ice.")]
    [SerializeField] CompassConditionUI ui;

    [Header("Dialogues")]
    [Tooltip("Fires when the player has hit hitsBeforeHint times — 'tension lines' hint.")]
    [SerializeField] DialogueScr hintDialog;
    [Tooltip("Fires when the compass reaches 0 (broken).")]
    [SerializeField] DialogueScr brokenDialog;

    [Header("Replay")]
    [Tooltip("Replay button. Hidden until the puzzle ends (Solved or Failed).")]
    [SerializeField] GameObject replayButton;

    public IcePuzzleState State { get; private set; } = IcePuzzleState.Initial;
    public event Action<CompassTier> OnSolved;
    public event Action OnFailed;

    int _wrongHits;
    bool _hintFired;
    bool _buttonShown;

    void Awake()
    {
        if (compasObject != null) compasObject.SetActive(false);
        if (finalCrackVisual != null) finalCrackVisual.SetActive(false);
        if (ui != null) ui.Hide();   // Bar appears only on the lake close-up, not on the walking scene.
        if (replayButton != null) replayButton.SetActive(false);
    }

    void Update()
    {
        if (State == IcePuzzleState.Solved || State == IcePuzzleState.Failed) return;
        if (!Input.GetMouseButtonDown(0)) return;

        // If the click landed on a UI element (button, etc.), do NOT also strike the ice.
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        // While a dialogue is showing, this click is for the dialogue — do not also damage the ice.
        if (hintDialog != null && hintDialog.IsDialogueActive()) return;
        if (brokenDialog != null && brokenDialog.IsDialogueActive()) return;

        var cam = puzzleCam != null ? puzzleCam : Camera.main;
        if (cam == null) return;

        Vector2 mp = cam.ScreenToWorldPoint(Input.mousePosition);
        var hit = Physics2D.Raycast(mp, Vector2.zero);
        if (hit.collider == null) return;

        bool isIce = iceCollider != null
            ? hit.collider == iceCollider
            : hit.collider.CompareTag("Ice");
        if (!isIce) return;

        OnIceClicked(mp);
    }

    void OnIceClicked(Vector2 worldPos)
    {
        // First click on the ice — the puzzle is now active, surface the bar.
        // The Investigate button is shown later (after hitsBeforeButton wrong strikes).
        if (ui != null) ui.Show();

        if (investigation != null && investigation.IsActive
            && weakPoints != null && weakPoints.TryHit(worldPos, out var wp))
        {
            wp.consumed = true;
            Solve();
            return;
        }

        // Wrong strike: damage compass + place a random crack
        if (compass != null) compass.TakeDamage(config != null ? config.damagePerWrongStrike : 20);
        cracks?.ShowCrackAt(worldPos);
        _wrongHits++;

        // Hint dialog after N wrong strikes (default 2)
        int hintAt = config != null ? config.hitsBeforeHint : 2;
        if (!_hintFired && _wrongHits >= hintAt)
        {
            _hintFired = true;
            State = IcePuzzleState.HintShown;
            if (hintDialog != null)
            {
                hintDialog.gameObject.SetActive(true);
                hintDialog.ActivateDialogue();
            }
        }

        // Investigate button after one more wrong strike (default 3)
        int btnAt = config != null ? config.hitsBeforeButton : 3;
        if (!_buttonShown && _wrongHits >= btnAt)
        {
            _buttonShown = true;
            if (investigation != null) investigation.EnableButton();
        }

        // Compass broken — fail.
        if (compass != null && compass.Current <= 0)
            Fail();
    }

    void Solve()
    {
        State = IcePuzzleState.Solved;
        if (finalCrackVisual != null) finalCrackVisual.SetActive(true);
        if (compasObject != null) compasObject.SetActive(true);
        if (investigation != null && investigation.IsActive) investigation.Toggle();   // hide highlights
        if (replayButton != null) replayButton.SetActive(true);
        OnSolved?.Invoke(compass != null ? compass.Tier : CompassTier.Pristine);
    }

    void Fail()
    {
        State = IcePuzzleState.Failed;
        if (brokenDialog != null)
        {
            brokenDialog.gameObject.SetActive(true);
            brokenDialog.ActivateDialogue();
        }
        if (replayButton != null) replayButton.SetActive(true);
        OnFailed?.Invoke();
    }

    /// <summary>Hook this to the Replay button's OnClick. Resets to fresh-scene state.</summary>
    public void Restart()
    {
        _wrongHits = 0;
        _hintFired = false;
        _buttonShown = false;
        State = IcePuzzleState.Initial;

        if (compass != null) compass.ResetToInitial();
        if (cracks != null) cracks.ResetAll();
        if (weakPoints != null) weakPoints.ResetAll();
        if (investigation != null) investigation.HideButton();

        if (compasObject != null) compasObject.SetActive(false);
        if (finalCrackVisual != null) finalCrackVisual.SetActive(false);

        if (brokenDialog != null && brokenDialog.IsDialogueActive())
            brokenDialog.gameObject.SetActive(false);
        if (hintDialog != null && hintDialog.IsDialogueActive())
            hintDialog.gameObject.SetActive(false);

        if (ui != null) ui.Hide();
        if (replayButton != null) replayButton.SetActive(false);
    }
}
