using System;
using UnityEngine;

public class InvestigationModeController : MonoBehaviour
{
    [SerializeField] WeakPointDetector detector;
    [Tooltip("UI button GameObject. Hidden until EnableButton() is called by the puzzle hint.")]
    [SerializeField] GameObject toggleButton;

    public bool IsActive { get; private set; }
    public event Action<bool> OnToggled;

    void Awake()
    {
        // Hide initially. IcePuzzleManager calls EnableButton() once the player
        // first interacts with the ice (i.e. is in the lake close-up).
        if (toggleButton != null) toggleButton.SetActive(false);
    }

    /// <summary>Called by IcePuzzleManager once the player has earned the hint.</summary>
    public void EnableButton()
    {
        if (toggleButton != null) toggleButton.SetActive(true);
    }

    /// <summary>Hook this to the UI Button.OnClick event.</summary>
    public void Toggle()
    {
        IsActive = !IsActive;
        if (detector != null)
        {
            if (IsActive) detector.ShowHighlights();
            else          detector.HideHighlights();
        }
        OnToggled?.Invoke(IsActive);
    }

    /// <summary>Reset to pre-hint state: hide button, drop highlights, clear active flag.</summary>
    public void HideButton()
    {
        if (detector != null) detector.HideHighlights();
        IsActive = false;
        if (toggleButton != null) toggleButton.SetActive(false);
    }
}
