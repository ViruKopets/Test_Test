using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class BuildGranHousePuzzle
{
    const string ScenePath        = "Assets/Scenes/6GranHouse.unity";
    const string AliceSpritePath  = "Assets/Visual/Characters/alice 1.png";
    const string HintText         = "Ммм... Где-то я уже это видела?";
    const string HintName         = "Алиса";

    [MenuItem("Tools/Build GranHouse Picture Puzzle")]
    public static void Build()
    {
        if (Application.isPlaying)
        {
            EditorUtility.DisplayDialog("GranHouse Puzzle",
                "Stop Play mode first.", "OK");
            return;
        }

        Scene scene;
        var active = SceneManager.GetActiveScene();
        if (active.path == ScenePath)
        {
            scene = active;
        }
        else
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        }

        var roots = scene.GetRootGameObjects();
        var all = new List<Transform>();
        foreach (var r in roots) GatherAll(r.transform, all);

        // 4 puzzle pictures: name like "Portrait (N)" but NOT the flipped one
        var portraits = all.Where(t =>
            t.name.StartsWith("Portrait (") && !t.name.ToLower().Contains("flipped"))
            .OrderBy(t => t.position.x)
            .ToList();

        // Reveal: contains "flipped" in name
        var reveal = all.FirstOrDefault(t => t.name.ToLower().Contains("flipped"));

        if (portraits.Count != 4)
        {
            EditorUtility.DisplayDialog("GranHouse Puzzle",
                $"Expected 4 puzzle portraits (Portrait (N) without 'flipped'), found {portraits.Count}.\nAborting.",
                "OK");
            return;
        }
        if (reveal == null)
        {
            EditorUtility.DisplayDialog("GranHouse Puzzle",
                "Reveal portrait (name contains 'flipped') not found.\nAborting.",
                "OK");
            return;
        }

        // Make sure each puzzle picture has a Collider2D so raycast can hit it
        // and tag them ChangeCam so the existing CameraChange script switches
        // to the puzzle close-up when a picture is clicked.
        foreach (var p in portraits)
        {
            if (p.GetComponent<Collider2D>() == null)
                p.gameObject.AddComponent<BoxCollider2D>();
            if (p.gameObject.tag != "ChangeCam")
                p.gameObject.tag = "ChangeCam";
        }

        // Wrap each puzzle picture in a uniform-scale rotation parent at scene root.
        // MinigameBackGround has scale (~17.85, ~-10.24, 1) — rotating any child under
        // such a non-uniform parent causes shear (parallelogram). Wrappers solve this.
        var rotationTargets = new List<Transform>();
        foreach (var p in portraits)
            rotationTargets.Add(EnsureRotationWrapper(p, scene));

        // Reveal picture: hidden initially, appears on success.
        reveal.gameObject.SetActive(false);
        if (reveal.gameObject.tag == "Pic")
            reveal.gameObject.tag = "Untagged";

        // Activate the ChangeCam trigger so the room-to-puzzle camera switch
        // actually fires when the player clicks the pictures area.
        var changeCamTrigger = all.FirstOrDefault(t =>
            t.gameObject.CompareTag("ChangeCam") &&
            t.GetComponent<Collider2D>() != null &&
            !portraits.Contains(t));
        if (changeCamTrigger != null && !changeCamTrigger.gameObject.activeSelf)
        {
            changeCamTrigger.gameObject.SetActive(true);
            Debug.Log($"[BuildGranHousePuzzle] Activated camera-switch trigger '{changeCamTrigger.name}'.");
        }

        // Clone UI refs from any existing DialogueScr in scene
        var existingDialog = all
            .Select(t => t.GetComponent<DialogueScr>())
            .FirstOrDefault(d => d != null);

        if (existingDialog == null)
        {
            EditorUtility.DisplayDialog("GranHouse Puzzle",
                "No existing DialogueScr found in scene — cannot clone UI refs for hint dialog.\nAborting.",
                "OK");
            return;
        }

        var aliceSprite = AssetDatabase.LoadAllAssetsAtPath(AliceSpritePath).OfType<Sprite>().FirstOrDefault()
                          ?? AssetDatabase.LoadAssetAtPath<Sprite>(AliceSpritePath);

        // === Hint dialogue (DialogueScr) ===
        var hintGo = FindOrCreateRoot(roots, scene, "PictureHintThought");
        var hintDlg = hintGo.GetComponent<DialogueScr>() ?? hintGo.AddComponent<DialogueScr>();

        var srcSo = new SerializedObject(existingDialog);
        var dstSo = new SerializedObject(hintDlg);
        dstSo.FindProperty("ImgPlace").objectReferenceValue      = srcSo.FindProperty("ImgPlace").objectReferenceValue;
        dstSo.FindProperty("WordPlace").objectReferenceValue     = srcSo.FindProperty("WordPlace").objectReferenceValue;
        dstSo.FindProperty("NamePlace").objectReferenceValue     = srcSo.FindProperty("NamePlace").objectReferenceValue;
        dstSo.FindProperty("DialoguePanel").objectReferenceValue = srcSo.FindProperty("DialoguePanel").objectReferenceValue;

        var imgs  = dstSo.FindProperty("Imgs");
        var words = dstSo.FindProperty("Words");
        var names = dstSo.FindProperty("Names");
        imgs.arraySize  = 1;
        words.arraySize = 1;
        names.arraySize = 1;
        imgs.GetArrayElementAtIndex(0).objectReferenceValue = aliceSprite;
        words.GetArrayElementAtIndex(0).stringValue         = HintText;
        names.GetArrayElementAtIndex(0).stringValue         = HintName;

        dstSo.FindProperty("ActivateOnStart").boolValue   = false;
        dstSo.FindProperty("ActivateOnTrigger").boolValue = false;
        dstSo.ApplyModifiedPropertiesWithoutUndo();

        // === PictureHint (one-shot listener) ===
        var hintCtrlGo = FindOrCreateRoot(roots, scene, "PictureHintController");
        var hint = hintCtrlGo.GetComponent<PictureHint>() ?? hintCtrlGo.AddComponent<PictureHint>();
        var hintSo = new SerializedObject(hint);
        var hintPics = hintSo.FindProperty("Pictures");
        hintPics.arraySize = portraits.Count;
        for (int i = 0; i < portraits.Count; i++)
            hintPics.GetArrayElementAtIndex(i).objectReferenceValue = portraits[i];
        hintSo.FindProperty("HintDialogue").objectReferenceValue = hintDlg;
        hintSo.ApplyModifiedPropertiesWithoutUndo();

        // === PictureCodePuzzle ===
        var puzzleGo = FindOrCreateRoot(roots, scene, "PictureCodePuzzleController");
        var puzzle = puzzleGo.GetComponent<PictureCodePuzzle>() ?? puzzleGo.AddComponent<PictureCodePuzzle>();
        var puzSo = new SerializedObject(puzzle);
        var puzPics = puzSo.FindProperty("Pictures");
        puzPics.arraySize = rotationTargets.Count;
        for (int i = 0; i < rotationTargets.Count; i++)
            puzPics.GetArrayElementAtIndex(i).objectReferenceValue = rotationTargets[i];
        puzSo.FindProperty("RevealPicture").objectReferenceValue = reveal;
        puzSo.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        var orderStr = string.Join("\n  ", portraits.Select((p, i) => $"#{i+1} ({(int)(p.position.x*100)/100f}, {(int)(p.position.y*100)/100f}) — {p.name}"));
        EditorUtility.DisplayDialog("GranHouse Puzzle",
            $"Wired puzzle in {ScenePath}.\n\n" +
            $"Left-to-right order (sorted by world X):\n  {orderStr}\n\n" +
            $"Reveal picture: {reveal.name}\n" +
            $"Hint dialogue: PictureHintThought (clone of '{existingDialog.gameObject.name}')\n\n" +
            "If the order doesn't match what you want (small/big-top/big-bottom/big-top), reorder the slots in PictureCodePuzzleController → Pictures in Inspector.",
            "OK");
    }

    static Transform EnsureRotationWrapper(Transform pic, Scene scene)
    {
        string wrapName = pic.name + "_RotWrap";
        if (pic.parent != null && pic.parent.name == wrapName)
            return pic.parent;

        var wrap = new GameObject(wrapName).transform;
        SceneManager.MoveGameObjectToScene(wrap.gameObject, scene);
        wrap.SetParent(null);                 // scene root — no inherited non-uniform scale
        wrap.position = pic.position;         // match picture's current world transform
        wrap.rotation = pic.rotation;
        wrap.localScale = Vector3.one;        // critical: uniform scale → clean rotation
        pic.SetParent(wrap, true);            // worldPositionStays — picture stays visually
        return wrap;
    }

    static GameObject FindOrCreateRoot(GameObject[] roots, Scene scene, string name)
    {
        var existing = roots.FirstOrDefault(r => r.name == name);
        if (existing != null) return existing;
        var go = new GameObject(name);
        SceneManager.MoveGameObjectToScene(go, scene);
        return go;
    }

    static void GatherAll(Transform t, List<Transform> result)
    {
        result.Add(t);
        foreach (Transform child in t) GatherAll(child, result);
    }
}
