using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeakPoint
{
    [Tooltip("Empty Transform marking the weak-point world position on the ice.")]
    public Transform marker;
    [Tooltip("If set, this SpriteRenderer is enabled when investigation mode is on (visible hint).")]
    public SpriteRenderer highlightRenderer;
    [HideInInspector] public bool consumed;
}

public class WeakPointDetector : MonoBehaviour
{
    [SerializeField] IcePuzzleConfig config;
    [SerializeField] List<WeakPoint> weakPoints = new List<WeakPoint>();
    [Tooltip("Used only if a WeakPoint has no highlightRenderer. Optional spawned highlight prefab.")]
    [SerializeField] GameObject highlightPrefab;

    readonly List<GameObject> _spawnedHighlights = new List<GameObject>();
    bool _highlightsVisible;

    void Awake()
    {
        // Make sure pre-set highlights start hidden.
        foreach (var wp in weakPoints)
            if (wp != null && wp.highlightRenderer != null)
                wp.highlightRenderer.enabled = false;
    }

    public bool TryHit(Vector2 worldPos, out WeakPoint hit)
    {
        hit = null;
        float r = config != null ? config.weakPointHitRadius : 0.5f;
        float r2 = r * r;
        foreach (var wp in weakPoints)
        {
            if (wp == null || wp.marker == null || wp.consumed) continue;
            Vector2 p = wp.marker.position;
            if ((p - worldPos).sqrMagnitude <= r2) { hit = wp; return true; }
        }
        return false;
    }

    public void ShowHighlights()
    {
        if (_highlightsVisible) return;
        _highlightsVisible = true;

        foreach (var wp in weakPoints)
        {
            if (wp == null || wp.marker == null || wp.consumed) continue;
            if (wp.highlightRenderer != null)
            {
                wp.highlightRenderer.enabled = true;
            }
            else if (highlightPrefab != null)
            {
                var h = Instantiate(highlightPrefab, wp.marker.position, Quaternion.identity, transform);
                _spawnedHighlights.Add(h);
            }
        }
    }

    public void HideHighlights()
    {
        if (!_highlightsVisible) return;
        _highlightsVisible = false;

        foreach (var wp in weakPoints)
            if (wp != null && wp.highlightRenderer != null) wp.highlightRenderer.enabled = false;

        foreach (var h in _spawnedHighlights) if (h != null) Destroy(h);
        _spawnedHighlights.Clear();
    }

    public void ResetAll()
    {
        HideHighlights();
        foreach (var wp in weakPoints)
            if (wp != null) wp.consumed = false;
    }
}
