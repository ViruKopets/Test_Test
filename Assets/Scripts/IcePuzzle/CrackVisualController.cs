using System.Collections.Generic;
using UnityEngine;

public class CrackVisualController : MonoBehaviour
{
    [Tooltip("The 6 pre-placed Break# GameObjects. They start inactive.")]
    [SerializeField] List<GameObject> crackObjects = new List<GameObject>();

    [SerializeField] bool randomize = true;

    void Awake()
    {
        foreach (var c in crackObjects)
            if (c != null) c.SetActive(false);
    }

    /// <returns>true if a crack was placed; false if the pool is exhausted.</returns>
    public bool ShowCrackAt(Vector2 worldPos)
    {
        var available = new List<GameObject>();
        foreach (var c in crackObjects)
            if (c != null && !c.activeSelf) available.Add(c);
        if (available.Count == 0) return false;

        int idx = randomize ? Random.Range(0, available.Count) : 0;
        var crack = available[idx];
        crack.transform.position = new Vector3(worldPos.x, worldPos.y, crack.transform.position.z);
        crack.SetActive(true);
        return true;
    }

    public void ResetAll()
    {
        foreach (var c in crackObjects)
            if (c != null) c.SetActive(false);
    }

    public int RemainingSlots()
    {
        int n = 0;
        foreach (var c in crackObjects) if (c != null && !c.activeSelf) n++;
        return n;
    }
}
