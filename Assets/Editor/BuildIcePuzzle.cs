using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class BuildIcePuzzle
{
    const string ScenePath  = "Assets/Scenes/3Forrest.unity";
    const string ConfigPath = "Assets/Scripts/IcePuzzle/IcePuzzleConfig.asset";

    [MenuItem("Tools/Build Ice Puzzle (3Forrest)")]
    public static void Build()
    {
        if (Application.isPlaying)
        {
            EditorUtility.DisplayDialog("Ice Puzzle", "Stop Play mode first.", "OK");
            return;
        }

        // 1. Open scene without losing unsaved work
        Scene scene;
        var active = SceneManager.GetActiveScene();
        if (active.path == ScenePath) scene = active;
        else
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        }

        // 2. Make sure config asset exists
        var config = AssetDatabase.LoadAssetAtPath<IcePuzzleConfig>(ConfigPath);
        if (config == null)
        {
            Directory.CreateDirectory("Assets/Scripts/IcePuzzle");
            config = ScriptableObject.CreateInstance<IcePuzzleConfig>();
            AssetDatabase.CreateAsset(config, ConfigPath);
            AssetDatabase.SaveAssets();
        }

        // 3. Locate scene objects
        var roots = scene.GetRootGameObjects();
        var all = new List<Transform>();
        foreach (var r in roots) Gather(r.transform, all);

        // The puzzle ice surface is the GameObject named "MiniGame" (tag "Ice"), shown
        // only in the lake close-up. The "Ice(interactble)" sprite lives on the walking
        // picture and must NOT have a blocking collider.
        var miniGo      = all.FirstOrDefault(t => t.name == "MiniGame" && t.gameObject.CompareTag("Ice"))?.gameObject
                          ?? all.FirstOrDefault(t => t.gameObject.CompareTag("Ice"))?.gameObject;
        var iceVisualGo = all.FirstOrDefault(t => t.name == "Ice(interactble)")?.gameObject;
        var compasGo    = all.FirstOrDefault(t => t.name == "Compas")?.gameObject;
        var finalCrack  = all.FirstOrDefault(t => t.name == "FinalCrack")?.gameObject;
        var puzzleCam   = all.Select(t => t.GetComponent<Camera>()).FirstOrDefault(c => c != null);

        var breaks = new List<GameObject>();
        for (int i = 1; i <= 6; i++)
        {
            var b = all.FirstOrDefault(t => t.name == $"Break{i}");
            if (b != null) breaks.Add(b.gameObject);
        }

        if (miniGo == null || compasGo == null || finalCrack == null || breaks.Count != 6)
        {
            EditorUtility.DisplayDialog("Ice Puzzle",
                $"Missing scene objects:\n" +
                $"MiniGame (Ice-tagged): {(miniGo != null)}, Compas: {(compasGo != null)}, " +
                $"FinalCrack: {(finalCrack != null)}, Break1..6: {breaks.Count}/6\n\nAborting.",
                "OK");
            return;
        }

        // Clean up: a previous version of this script added a BoxCollider2D to
        // Ice(interactble) on the walking picture, which blocks the player.
        if (iceVisualGo != null)
        {
            foreach (var col in iceVisualGo.GetComponents<Collider2D>())
            {
                Object.DestroyImmediate(col, true);
                Debug.Log($"[BuildIcePuzzle] Removed stray Collider2D from {iceVisualGo.name} so the player can walk past.");
            }
        }

        // Disable old IceMiniGame so it does not fight the new system
        var old = miniGo.GetComponent<IceMiniGame>();
        if (old != null) old.enabled = false;

        // MiniGame must have a Collider2D for raycast (it normally already does as a trigger)
        var iceCollider = miniGo.GetComponent<Collider2D>();
        if (iceCollider == null) iceCollider = miniGo.AddComponent<BoxCollider2D>();

        // 6. Build / find IcePuzzle container with all controllers
        var container = FindOrCreateRoot(roots, scene, "IcePuzzle");

        var compass        = container.GetOrAdd<CompassConditionSystem>();
        var crackCtrl      = container.GetOrAdd<CrackVisualController>();
        var weakDetector   = container.GetOrAdd<WeakPointDetector>();
        var investigation  = container.GetOrAdd<InvestigationModeController>();
        var manager        = container.GetOrAdd<IcePuzzleManager>();

        SetField(compass, "config", config);
        SetField(crackCtrl, "crackObjects", breaks);
        SetField(weakDetector, "config", config);

        // 7. Weak points: create 3 markers as children of container, positioned over the ice
        var bounds = iceCollider != null ? iceCollider.bounds : new Bounds(miniGo.transform.position, Vector3.one * 2f);
        var wpList = new List<WeakPoint>();
        Vector3[] offsets = {
            new Vector3(-bounds.extents.x * 0.4f, +bounds.extents.y * 0.3f, 0),
            new Vector3(+bounds.extents.x * 0.5f, -bounds.extents.y * 0.2f, 0),
            new Vector3( 0f,                       -bounds.extents.y * 0.45f, 0),
        };
        for (int i = 0; i < 3; i++)
        {
            var name = $"WeakPoint_{i + 1}";
            var existing = container.transform.Find(name);
            GameObject markerGo;
            if (existing != null) markerGo = existing.gameObject;
            else
            {
                markerGo = new GameObject(name);
                markerGo.transform.SetParent(container.transform, false);
                markerGo.transform.position = bounds.center + offsets[i];
            }
            var wp = new WeakPoint { marker = markerGo.transform };

            // Optional simple highlight: a small SpriteRenderer disabled by default
            var hlGoName = name + "_Highlight";
            var hlExisting = markerGo.transform.Find(hlGoName);
            GameObject hlGo;
            if (hlExisting != null) hlGo = hlExisting.gameObject;
            else
            {
                hlGo = new GameObject(hlGoName);
                hlGo.transform.SetParent(markerGo.transform, false);
                var sr = hlGo.AddComponent<SpriteRenderer>();
                sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
                sr.color = new Color(0.3f, 1f, 0.4f, 0.7f);
                sr.sortingOrder = 50;
                hlGo.transform.localScale = Vector3.one * 0.6f;
            }
            wp.highlightRenderer = hlGo.GetComponent<SpriteRenderer>();
            wp.highlightRenderer.enabled = false;
            wpList.Add(wp);
        }
        SetField(weakDetector, "weakPoints", wpList);

        // 8. UI: Canvas + condition bar + investigate button
        var uiCanvas = FindOrCreateUiCanvas(roots, scene);
        var (barRoot, fillImg, label) = FindOrCreateConditionBar(uiCanvas);
        var (btnGo, btnComponent)     = FindOrCreateInvestigateButton(uiCanvas);

        var ui = uiCanvas.GetOrAdd<CompassConditionUI>();
        SetField(ui, "source", compass);
        SetField(ui, "fillImage", fillImg);
        SetField(ui, "label", label);
        SetField(ui, "root", barRoot);

        SetField(investigation, "detector", weakDetector);
        SetField(investigation, "toggleButton", btnGo);

        // Hook the button OnClick to InvestigationModeController.Toggle.
        // Clear ALL existing persistent listeners first to avoid duplicates accumulating
        // when this Editor script is re-run.
        var onClick = btnComponent.onClick;
        for (int i = onClick.GetPersistentEventCount() - 1; i >= 0; i--)
            UnityEditor.Events.UnityEventTools.RemovePersistentListener(onClick, i);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(onClick, investigation.Toggle);

        // 9. Wire IcePuzzleManager
        SetField(manager, "config", config);
        SetField(manager, "compass", compass);
        SetField(manager, "cracks", crackCtrl);
        SetField(manager, "weakPoints", weakDetector);
        SetField(manager, "investigation", investigation);
        SetField(manager, "puzzleCam", puzzleCam);
        SetField(manager, "iceCollider", iceCollider);
        SetField(manager, "compasObject", compasGo);
        SetField(manager, "finalCrackVisual", finalCrack);
        SetField(manager, "ui", ui);

        // Hide compass + final crack initially (manager re-applies in Awake at runtime,
        // but doing it here keeps Scene view tidy too).
        compasGo.SetActive(false);
        finalCrack.SetActive(false);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("[BuildIcePuzzle] Wired ice puzzle. Hint dialog & broken dialog NOT auto-set — assign them in IcePuzzleManager Inspector.");
        EditorUtility.DisplayDialog("Ice Puzzle",
            "Ice puzzle wired in 3Forrest.\n\n" +
            "TODO in Inspector:\n" +
            "  • IcePuzzleManager → Hint Dialog → drag a DialogueScr (e.g. LujaDialog or new)\n" +
            "  • IcePuzzleManager → Broken Dialog → optional, for compass-broken branch\n" +
            "  • Tweak weak point positions (3 markers under IcePuzzle GameObject)\n" +
            "  • Tweak Canvas bar position if desired (top-center by default)",
            "OK");
    }

    // === helpers ===

    static void Gather(Transform t, List<Transform> r)
    {
        r.Add(t);
        foreach (Transform c in t) Gather(c, r);
    }

    static GameObject FindOrCreateRoot(GameObject[] roots, Scene scene, string name)
    {
        var existing = roots.FirstOrDefault(r => r.name == name);
        if (existing != null) return existing;
        var go = new GameObject(name);
        SceneManager.MoveGameObjectToScene(go, scene);
        return go;
    }

    static GameObject FindOrCreateUiCanvas(GameObject[] roots, Scene scene)
    {
        var existing = roots.FirstOrDefault(r => r.name == "IcePuzzleUI");
        if (existing != null) return existing;

        var go = new GameObject("IcePuzzleUI");
        SceneManager.MoveGameObjectToScene(go, scene);
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;
        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        go.AddComponent<GraphicRaycaster>();
        return go;
    }

    static (GameObject root, Image fill, TMP_Text label) FindOrCreateConditionBar(GameObject canvasGo)
    {
        var existing = canvasGo.transform.Find("CompassBar");
        GameObject barRoot;
        Image bg = null, fill = null;
        TMP_Text label = null;

        if (existing != null)
        {
            barRoot = existing.gameObject;
            fill  = barRoot.transform.Find("Fill")?.GetComponent<Image>();
            label = barRoot.transform.Find("Label")?.GetComponent<TMP_Text>();
        }
        else
        {
            barRoot = new GameObject("CompassBar", typeof(RectTransform));
            barRoot.transform.SetParent(canvasGo.transform, false);
            var rt = (RectTransform)barRoot.transform;
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot     = new Vector2(0.5f, 1f);
            rt.anchoredPosition = new Vector2(0f, -40f);
            rt.sizeDelta = new Vector2(560f, 70f);
        }

        if (bg == null)
        {
            var bgGo = barRoot.transform.Find("Background")?.gameObject;
            if (bgGo == null)
            {
                bgGo = new GameObject("Background", typeof(RectTransform));
                bgGo.transform.SetParent(barRoot.transform, false);
                bg = bgGo.AddComponent<Image>();
                bg.color = new Color(0f, 0f, 0f, 0.6f);
                var rt = (RectTransform)bgGo.transform;
                rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            }
        }

        if (fill == null)
        {
            var fillGo = new GameObject("Fill", typeof(RectTransform));
            fillGo.transform.SetParent(barRoot.transform, false);
            fill = fillGo.AddComponent<Image>();
            fill.color = new Color(0.4f, 0.9f, 0.4f, 0.95f);
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillOrigin = (int)Image.OriginHorizontal.Left;
            fill.fillAmount = 1f;
            var rt = (RectTransform)fillGo.transform;
            rt.anchorMin = new Vector2(0f, 0f); rt.anchorMax = new Vector2(1f, 1f);
            rt.offsetMin = new Vector2(8f, 8f); rt.offsetMax = new Vector2(-8f, -34f);
        }

        if (label == null)
        {
            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(barRoot.transform, false);
            label = labelGo.AddComponent<TextMeshProUGUI>();
            label.text = "Компас: 100%";
            label.color = Color.white;
            label.fontSize = 22f;
            label.alignment = TextAlignmentOptions.Center;
            var rt = (RectTransform)labelGo.transform;
            rt.anchorMin = new Vector2(0f, 1f); rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot     = new Vector2(0.5f, 1f);
            rt.anchoredPosition = new Vector2(0f, -4f);
            rt.sizeDelta = new Vector2(0f, 30f);
        }

        return (barRoot, fill, label);
    }

    static (GameObject btnGo, Button btn) FindOrCreateInvestigateButton(GameObject canvasGo)
    {
        var existing = canvasGo.transform.Find("InvestigateButton");
        if (existing != null)
            return (existing.gameObject, existing.GetComponent<Button>());

        var btnGo = new GameObject("InvestigateButton", typeof(RectTransform));
        btnGo.transform.SetParent(canvasGo.transform, false);
        var img = btnGo.AddComponent<Image>();
        img.color = new Color(0.2f, 0.4f, 0.7f, 0.9f);
        var btn = btnGo.AddComponent<Button>();
        btn.targetGraphic = img;

        var rt = (RectTransform)btnGo.transform;
        rt.anchorMin = new Vector2(1f, 1f); rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot     = new Vector2(1f, 1f);
        rt.anchoredPosition = new Vector2(-40f, -40f);
        rt.sizeDelta = new Vector2(220f, 70f);

        var labelGo = new GameObject("Label", typeof(RectTransform));
        labelGo.transform.SetParent(btnGo.transform, false);
        var tmp = labelGo.AddComponent<TextMeshProUGUI>();
        tmp.text = "Исследовать";
        tmp.color = Color.white;
        tmp.fontSize = 26f;
        tmp.alignment = TextAlignmentOptions.Center;
        var lrt = (RectTransform)labelGo.transform;
        lrt.anchorMin = Vector2.zero; lrt.anchorMax = Vector2.one;
        lrt.offsetMin = Vector2.zero; lrt.offsetMax = Vector2.zero;

        btnGo.SetActive(false);  // Hidden until hint fires
        return (btnGo, btn);
    }

    static void SetField(Object target, string fieldName, object value)
    {
        var so = new SerializedObject(target);
        var prop = so.FindProperty(fieldName);
        if (prop == null) { Debug.LogWarning($"Field '{fieldName}' not found on {target.GetType().Name}"); return; }
        AssignValue(prop, value);
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    static void AssignValue(SerializedProperty prop, object value)
    {
        if (value == null) { prop.objectReferenceValue = null; return; }

        if (value is List<GameObject> goList)
        {
            prop.arraySize = goList.Count;
            for (int i = 0; i < goList.Count; i++)
                prop.GetArrayElementAtIndex(i).objectReferenceValue = goList[i];
            return;
        }

        if (value is List<WeakPoint> wpList)
        {
            prop.arraySize = wpList.Count;
            for (int i = 0; i < wpList.Count; i++)
            {
                var el = prop.GetArrayElementAtIndex(i);
                el.FindPropertyRelative("marker").objectReferenceValue = wpList[i].marker;
                el.FindPropertyRelative("highlightRenderer").objectReferenceValue = wpList[i].highlightRenderer;
                el.FindPropertyRelative("consumed").boolValue = wpList[i].consumed;
            }
            return;
        }

        if (value is Object uo) { prop.objectReferenceValue = uo; return; }
        Debug.LogWarning($"Don't know how to assign {value.GetType()} to {prop.propertyPath}");
    }
}

internal static class GameObjectExt
{
    public static T GetOrAdd<T>(this GameObject go) where T : Component
    {
        var c = go.GetComponent<T>();
        return c != null ? c : go.AddComponent<T>();
    }
}
