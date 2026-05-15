using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompassConditionUI : MonoBehaviour
{
    [SerializeField] CompassConditionSystem source;
    [Tooltip("Image with Image Type = Filled, Fill Method = Horizontal.")]
    [SerializeField] Image fillImage;
    [SerializeField] TMP_Text label;
    [Tooltip("Optional gradient: low condition → red, high → green.")]
    [SerializeField] Gradient fillGradient;
    [SerializeField] GameObject root;

    void OnEnable()
    {
        if (source != null) source.OnConditionChanged += OnChanged;
        Refresh();
    }
    void OnDisable()
    {
        if (source != null) source.OnConditionChanged -= OnChanged;
    }

    void OnChanged(int v, CompassTier t) => Refresh();

    void Refresh()
    {
        if (source == null) return;
        float t = Mathf.Clamp01(source.Current / 100f);
        if (fillImage != null)
        {
            fillImage.fillAmount = t;
            if (fillGradient != null) fillImage.color = fillGradient.Evaluate(t);
        }
        if (label != null) label.text = $"Компас: {source.Current}%  ({TierLabel(source.Tier)})";
    }

    public void Show()
    {
        var target = root != null ? root : (fillImage != null ? fillImage.gameObject : null);
        if (target != null) target.SetActive(true);
    }
    public void Hide()
    {
        var target = root != null ? root : (fillImage != null ? fillImage.gameObject : null);
        if (target != null) target.SetActive(false);
    }

    static string TierLabel(CompassTier t)
    {
        switch (t)
        {
            case CompassTier.Pristine: return "идеален";
            case CompassTier.Light:    return "царапины";
            case CompassTier.Damaged:  return "повреждён";
            case CompassTier.Severe:   return "сильно повреждён";
            case CompassTier.Critical: return "почти сломан";
            case CompassTier.Broken:   return "сломан";
        }
        return "?";
    }
}
