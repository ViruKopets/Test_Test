using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class BuildGranHouseExit
{
    const string ScenePath = "Assets/Scenes/6GranHouse.unity";
    const string TargetSceneName = "2City";
    const int   PinkHousePosId   = 1;   // GranHouse spawn in 2City.Bootstraper.PlayersPos

    [MenuItem("Tools/Build GranHouse Exit Button (6GranHouse → 2City)")]
    public static void Build()
    {
        if (Application.isPlaying)
        {
            EditorUtility.DisplayDialog("GranHouse Exit", "Stop Play mode first.", "OK");
            return;
        }

        Scene scene;
        var active = SceneManager.GetActiveScene();
        if (active.path == ScenePath) scene = active;
        else
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        }

        var roots = scene.GetRootGameObjects();
        var canvasGo = FindOrCreateExitCanvas(roots, scene);
        var (btnGo, btn) = FindOrCreateExitButton(canvasGo);

        // SceneLoader lives on the button itself.
        var loader = btnGo.GetComponent<SceneLoader>();
        if (loader == null) loader = btnGo.AddComponent<SceneLoader>();
        SetField(loader, "SceneName", TargetSceneName);
        SetField(loader, "NextScenePosId", PinkHousePosId);
        SetField(loader, "LoadByTrigger", false);   // UI button, not a 2D trigger
        SetField(loader, "ByProgress", false);
        SetField(loader, "MenuLoad", false);

        // Re-wire OnClick → SceneLoader.LoadScene, clearing previous listeners.
        var onClick = btn.onClick;
        for (int i = onClick.GetPersistentEventCount() - 1; i >= 0; i--)
            UnityEditor.Events.UnityEventTools.RemovePersistentListener(onClick, i);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(onClick, loader.LoadScene);

        // Hide the button while any DialogueScr is active.
        var hider = canvasGo.GetComponent<HideDuringDialogue>();
        if (hider == null) hider = canvasGo.AddComponent<HideDuringDialogue>();
        SetField(hider, "target", btnGo);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        EditorUtility.DisplayDialog("GranHouse Exit",
            "Exit button wired in 6GranHouse.\n\n" +
            "• Bottom-left UI button → SceneLoader → 2City (PosId 1, GranHouse spawn)\n" +
            "• Hidden while any DialogueScr is active",
            "OK");
    }

    static GameObject FindOrCreateExitCanvas(GameObject[] roots, Scene scene)
    {
        var existing = roots.FirstOrDefault(r => r.name == "ExitToCityUI");
        if (existing != null) return existing;

        var go = new GameObject("ExitToCityUI");
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

    static (GameObject btnGo, Button btn) FindOrCreateExitButton(GameObject canvasGo)
    {
        var existing = canvasGo.transform.Find("ExitButton");
        if (existing != null)
            return (existing.gameObject, existing.GetComponent<Button>());

        var btnGo = new GameObject("ExitButton", typeof(RectTransform));
        btnGo.transform.SetParent(canvasGo.transform, false);
        var img = btnGo.AddComponent<Image>();
        img.color = new Color(0.2f, 0.5f, 0.3f, 0.9f);
        var btn = btnGo.AddComponent<Button>();
        btn.targetGraphic = img;

        // Bottom-left anchor as requested.
        var rt = (RectTransform)btnGo.transform;
        rt.anchorMin = new Vector2(0f, 0f);
        rt.anchorMax = new Vector2(0f, 0f);
        rt.pivot     = new Vector2(0f, 0f);
        rt.anchoredPosition = new Vector2(40f, 40f);
        rt.sizeDelta = new Vector2(240f, 80f);

        var labelGo = new GameObject("Label", typeof(RectTransform));
        labelGo.transform.SetParent(btnGo.transform, false);
        var tmp = labelGo.AddComponent<TextMeshProUGUI>();
        tmp.text = "На улицу";
        tmp.color = Color.white;
        tmp.fontSize = 28f;
        tmp.alignment = TextAlignmentOptions.Center;
        var lrt = (RectTransform)labelGo.transform;
        lrt.anchorMin = Vector2.zero; lrt.anchorMax = Vector2.one;
        lrt.offsetMin = Vector2.zero; lrt.offsetMax = Vector2.zero;

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
        switch (value)
        {
            case string s:  prop.stringValue = s; return;
            case int i:     prop.intValue = i; return;
            case bool b:    prop.boolValue = b; return;
            case float f:   prop.floatValue = f; return;
            case Object uo: prop.objectReferenceValue = uo; return;
        }
        Debug.LogWarning($"Don't know how to assign {value.GetType()} to {prop.propertyPath}");
    }
}
