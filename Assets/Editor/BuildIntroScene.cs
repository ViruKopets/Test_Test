using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class BuildIntroScene
{
    const string ScenePath = "Assets/Scenes/1aGrandHouseIntro.unity";
    const string NextSceneName = "1MainHeroHome";
    const string BackgroundSpritePath = "Assets/Visual/BackGrounds/locations/GranHouse.PNG";

    static readonly string[] Names =
    {
        "Лука",
        "Бабушка Агата",
        "Алиса",
        "Бабушка Агата",
    };

    static readonly string[] Words =
    {
        "Бабуля Агата, а расскажите какую-нибудь тайну этого города. Что-то жуткое и загадочное!",
        "В нашем лесу… ходят слухи. Говорят, он забирает тех, кто слишком глубоко зашел.",
        "Выбирает? Как это?",
        "Но это всё старые сказки. Забудьте. И запомните одно: не ходите в лес.",
    };

    static readonly string[] SpriteNames = { "friend 1", "babka 1", "alice 1", "babka 1" };

    [MenuItem("Tools/Build Intro Scene")]
    public static void Build()
    {
        Sprite[] sprites = LoadSprites();
        if (sprites == null) return;

        Directory.CreateDirectory("Assets/Scenes");
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        var camGo = GameObject.Find("Main Camera");
        if (camGo != null)
        {
            var cam = camGo.GetComponent<Camera>();
            if (cam != null)
            {
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = new Color(0.05f, 0.05f, 0.08f, 1f);
                cam.orthographic = true;
            }
        }

        // EventSystem (needed for UI/clicks if any)
        var es = new GameObject("EventSystem");
        es.AddComponent<UnityEngine.EventSystems.EventSystem>();
        es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        // Canvas
        var canvasGo = new GameObject("Canvas");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGo.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
        canvasGo.AddComponent<GraphicRaycaster>();

        // Background image (full-screen, behind dialogue panel)
        var bgSprite = AssetDatabase.LoadAllAssetsAtPath(BackgroundSpritePath).OfType<Sprite>().FirstOrDefault()
                       ?? AssetDatabase.LoadAssetAtPath<Sprite>(BackgroundSpritePath);
        if (bgSprite != null)
        {
            var bgGo = new GameObject("Background");
            bgGo.transform.SetParent(canvasGo.transform, false);
            var bgImg = bgGo.AddComponent<Image>();
            bgImg.sprite = bgSprite;
            bgImg.preserveAspect = true;
            bgImg.raycastTarget = false;
            var bgRT = bgGo.GetComponent<RectTransform>();
            bgRT.anchorMin = Vector2.zero;
            bgRT.anchorMax = Vector2.one;
            bgRT.offsetMin = Vector2.zero;
            bgRT.offsetMax = Vector2.zero;
        }
        else
        {
            Debug.LogWarning($"[BuildIntroScene] Background sprite not found at {BackgroundSpritePath}, scene will use camera clear color.");
        }

        // DialoguePanel (background bar at bottom)
        var panelGo = new GameObject("DialoguePanel");
        panelGo.transform.SetParent(canvasGo.transform, false);
        var panelImg = panelGo.AddComponent<Image>();
        panelImg.color = new Color(0f, 0f, 0f, 0.75f);
        var panelRT = panelGo.GetComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(0f, 0f);
        panelRT.anchorMax = new Vector2(1f, 0.35f);
        panelRT.offsetMin = Vector2.zero;
        panelRT.offsetMax = Vector2.zero;

        // Portrait Image
        var imgGo = new GameObject("ImgPlace");
        imgGo.transform.SetParent(panelGo.transform, false);
        var portrait = imgGo.AddComponent<Image>();
        portrait.preserveAspect = true;
        var imgRT = imgGo.GetComponent<RectTransform>();
        imgRT.anchorMin = new Vector2(0f, 0f);
        imgRT.anchorMax = new Vector2(0f, 1f);
        imgRT.pivot = new Vector2(0f, 0.5f);
        imgRT.anchoredPosition = new Vector2(40f, 0f);
        imgRT.sizeDelta = new Vector2(360f, -40f);

        // Name label
        var nameGo = new GameObject("NamePlace");
        nameGo.transform.SetParent(panelGo.transform, false);
        var nameTmp = nameGo.AddComponent<TextMeshProUGUI>();
        nameTmp.fontSize = 42f;
        nameTmp.color = new Color(1f, 0.85f, 0.4f, 1f);
        nameTmp.alignment = TextAlignmentOptions.TopLeft;
        var nameRT = nameGo.GetComponent<RectTransform>();
        nameRT.anchorMin = new Vector2(0f, 1f);
        nameRT.anchorMax = new Vector2(1f, 1f);
        nameRT.pivot = new Vector2(0f, 1f);
        nameRT.anchoredPosition = new Vector2(440f, -20f);
        nameRT.sizeDelta = new Vector2(-480f, 60f);

        // Word text
        var wordGo = new GameObject("WordPlace");
        wordGo.transform.SetParent(panelGo.transform, false);
        var wordTmp = wordGo.AddComponent<TextMeshProUGUI>();
        wordTmp.fontSize = 36f;
        wordTmp.color = Color.white;
        wordTmp.enableWordWrapping = true;
        wordTmp.alignment = TextAlignmentOptions.TopLeft;
        var wordRT = wordGo.GetComponent<RectTransform>();
        wordRT.anchorMin = new Vector2(0f, 0f);
        wordRT.anchorMax = new Vector2(1f, 1f);
        wordRT.offsetMin = new Vector2(440f, 30f);
        wordRT.offsetMax = new Vector2(-40f, -90f);

        // DialogueScr host
        var dlgGo = new GameObject("IntroDialog");
        var dlg = dlgGo.AddComponent<DialogueScr>();
        var so = new SerializedObject(dlg);
        so.FindProperty("ImgPlace").objectReferenceValue = portrait;
        so.FindProperty("WordPlace").objectReferenceValue = wordTmp;
        so.FindProperty("NamePlace").objectReferenceValue = nameTmp;
        so.FindProperty("DialoguePanel").objectReferenceValue = panelGo;

        var imgsProp = so.FindProperty("Imgs");
        var wordsProp = so.FindProperty("Words");
        var namesProp = so.FindProperty("Names");
        imgsProp.arraySize = sprites.Length;
        wordsProp.arraySize = Words.Length;
        namesProp.arraySize = Names.Length;
        for (int i = 0; i < sprites.Length; i++)
        {
            imgsProp.GetArrayElementAtIndex(i).objectReferenceValue = sprites[i];
            wordsProp.GetArrayElementAtIndex(i).stringValue = Words[i];
            namesProp.GetArrayElementAtIndex(i).stringValue = Names[i];
        }

        so.FindProperty("ActivateOnStart").boolValue = true;
        so.FindProperty("LoadSceneAfter").stringValue = NextSceneName;
        so.FindProperty("TimeToLetter").floatValue = 0.04f;
        so.FindProperty("minDisplayTime").floatValue = 0.2f;
        so.ApplyModifiedPropertiesWithoutUndo();

        // CRITICAL: panel must start INACTIVE — DialogueScr.ActivateDialogue
        // exits early if DialoguePanel.activeSelf is true at start.
        panelGo.SetActive(false);

        EditorSceneManager.SaveScene(scene, ScenePath);
        AddSceneToBuild(ScenePath);

        Debug.Log($"[BuildIntroScene] Created {ScenePath} and added to Build Settings.");
        EditorUtility.DisplayDialog("Intro Scene", $"Scene built at:\n{ScenePath}\n\nAdded to Build Settings.\nPlay from 0MainMenu to test.", "OK");
    }

    static Sprite[] LoadSprites()
    {
        var result = new Sprite[SpriteNames.Length];
        for (int i = 0; i < SpriteNames.Length; i++)
        {
            string path = $"Assets/Visual/Characters/{SpriteNames[i]}.png";
            // Works for both Single and Multiple sprite modes
            var s = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().FirstOrDefault()
                    ?? AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (s == null)
            {
                EditorUtility.DisplayDialog("Missing sprite",
                    $"Sprite not found or not imported as Sprite:\n{path}\n\nIn Project window, select the texture and set Texture Type = Sprite (2D and UI), then run again.",
                    "OK");
                return null;
            }
            result[i] = s;
        }
        return result;
    }

    static void AddSceneToBuild(string path)
    {
        var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        for (int i = 0; i < scenes.Count; i++)
        {
            if (scenes[i].path == path)
            {
                scenes[i] = new EditorBuildSettingsScene(path, true);
                EditorBuildSettings.scenes = scenes.ToArray();
                return;
            }
        }
        // Try to insert right after 0MainMenu so the build order matches the runtime flow
        int insertAt = scenes.Count;
        for (int i = 0; i < scenes.Count; i++)
        {
            if (scenes[i].path.EndsWith("0MainMenu.unity"))
            {
                insertAt = i + 1;
                break;
            }
        }
        scenes.Insert(insertAt, new EditorBuildSettingsScene(path, true));
        EditorBuildSettings.scenes = scenes.ToArray();
    }
}
