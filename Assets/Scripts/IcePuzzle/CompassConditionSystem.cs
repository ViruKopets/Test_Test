using System;
using UnityEngine;

public enum CompassTier
{
    Pristine,   // 100
    Light,      // 80
    Damaged,    // 60
    Severe,     // 40
    Critical,   // 20
    Broken      // 0
}

public class CompassConditionSystem : MonoBehaviour
{
    [SerializeField] IcePuzzleConfig config;
    [SerializeField] int currentForInspector = -1;   // visible in Inspector for debugging

    public int Current { get; private set; }
    public CompassTier Tier => GetTier(Current);

    /// <summary>Fires after Current changes. Args: newValue, newTier.</summary>
    public event Action<int, CompassTier> OnConditionChanged;

    void Awake()
    {
        Current = config != null ? config.initialCondition : 100;
        currentForInspector = Current;
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;
        var prev = Current;
        Current = Mathf.Max(0, Current - amount);
        currentForInspector = Current;
        if (Current != prev) OnConditionChanged?.Invoke(Current, Tier);
    }

    public void ResetToInitial()
    {
        Current = config != null ? config.initialCondition : 100;
        currentForInspector = Current;
        OnConditionChanged?.Invoke(Current, Tier);
    }

    public static CompassTier GetTier(int v)
    {
        if (v >= 100) return CompassTier.Pristine;
        if (v >= 80)  return CompassTier.Light;
        if (v >= 60)  return CompassTier.Damaged;
        if (v >= 40)  return CompassTier.Severe;
        if (v >= 20)  return CompassTier.Critical;
        return CompassTier.Broken;
    }
}
