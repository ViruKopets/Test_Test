using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class BuildNewspaperContent
{
    const string ScenePath = "Assets/Scenes/4Hostel.unity";
    const string TemplateNamePrefix = "заготовка газеты";
    const string GeneratedRootName = "GeneratedText";

    // === Tweak these if text is too small / too big ===
    const float TitleFontSize  = 1.6f;
    const float BodyFontSize   = 2.0f;
    const float AuthorFontSize = 1.3f;
    // Layout fractions of newspaper bounds (don't usually need to change)
    const float InnerWidthFrac   = 0.86f;
    const float InnerHeightFrac  = 0.88f;
    const float TitleWidthFrac   = 0.58f;   // leaves room for the photo on top-right
    const float TitleHeightFrac  = 0.16f;
    const float TitleYFrac       = 0.41f;   // pushed higher into the framed top area
    const float BodyHeightFrac   = 0.55f;
    const float BodyYFrac        = -0.10f;  // pushed lower so it doesn't touch title
    const float AuthorYFrac      = -0.43f;

    class Paper
    {
        public string Title;
        public string Body;
        public string Author;
    }

    static readonly Paper[] Papers = new[]
    {
        new Paper {
            Title  = "Будьте Осторожны!",
            Body   = "В лесу близ города обнаружили следы опасных животных. Не гуляйте в лесу и следите за детьми",
            Author = "А.Чикатлов",
        },
        new Paper {
            Title  = "Тру крайм!",
            Body   = "Главными причинами пропажи людей являются проблемы с физическим и психическим здоровьем.",
            Author = "А.Чикатлов",
        },
        new Paper {
            Title  = "Пропажа!",
            Body   = "Очередное исчезновение спустя 5 лет. Теперь пропал мальчик 12 лет, зовут Лука.\nЕсли вы его видели сообщите полиции или в газету...",
            Author = "А.Чикатлов",
        },
        new Paper {
            Title  = "Бабушкины рецепты!",
            Body   = "Рецепты пирогов от бабули.\nСтакан муки\nСтакан молока\nЯйца 3 штуки\nСахар\nВаренье\nМасло",
            Author = "А.Кристалова",
        },
        new Paper {
            Title  = "Новый офицер!",
            Body   = "Назначение нового окружного успокоило городских жителей. Офицер уже приступила к работе.",
            Author = "А.Чикатлов",
        },
    };

    [MenuItem("Tools/Build Newspaper Content")]
    public static void Build()
    {
        if (Application.isPlaying)
        {
            EditorUtility.DisplayDialog("Newspaper Content",
                "Stop Play mode first — this script edits scene assets.", "OK");
            return;
        }

        Scene scene;
        var active = SceneManager.GetActiveScene();
        if (active.path == ScenePath)
        {
            // 4Hostel is already open — use it as-is so unsaved layout work is preserved.
            scene = active;
        }
        else
        {
            // Another scene is open. Make sure user doesn't lose unsaved work there.
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        }

        var roots = scene.GetRootGameObjects();

        var templates = new List<GameObject>();
        foreach (var root in roots) FindTemplates(root.transform, templates);

        if (templates.Count != Papers.Length)
        {
            EditorUtility.DisplayDialog("Newspaper Content",
                $"Expected {Papers.Length} newspaper templates (name starts with \"{TemplateNamePrefix}\"), found {templates.Count}.\nNothing changed.",
                "OK");
            return;
        }

        templates.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));

        for (int i = 0; i < Papers.Length; i++)
            SetupNewspaper(templates[i], Papers[i], i + 1);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        EditorUtility.DisplayDialog("Newspaper Content",
            $"Added title/body/author text to {Papers.Length} newspapers in {ScenePath}.\n\n" +
            "Tweak font size, position and color in Inspector if needed (each newspaper has a child \"GeneratedText\" with Title/Body/Author).",
            "OK");
    }

    static void FindTemplates(Transform t, List<GameObject> result)
    {
        if (t.name.StartsWith(TemplateNamePrefix))
            result.Add(t.gameObject);
        foreach (Transform child in t)
            FindTemplates(child, result);
    }

    static void SetupNewspaper(GameObject template, Paper paper, int orderFromLeft)
    {
        var existing = template.transform.Find(GeneratedRootName);
        if (existing != null) Object.DestroyImmediate(existing.gameObject);

        var sr = template.GetComponent<SpriteRenderer>();
        Vector2 worldSize = sr != null ? sr.bounds.size : new Vector2(2.5f, 3.5f);

        // Compensate parent scale so text uses world units
        var ps = template.transform.localScale;
        var wrapperScale = new Vector3(
            !Mathf.Approximately(ps.x, 0f) ? 1f / ps.x : 1f,
            !Mathf.Approximately(ps.y, 0f) ? 1f / ps.y : 1f,
            1f);

        var wrapper = new GameObject(GeneratedRootName);
        wrapper.transform.SetParent(template.transform, false);
        wrapper.transform.localPosition = Vector3.zero;
        wrapper.transform.localRotation = Quaternion.identity;
        wrapper.transform.localScale = wrapperScale;

        float innerW = worldSize.x * InnerWidthFrac;
        float innerH = worldSize.y * InnerHeightFrac;

        float titleW = innerW * TitleWidthFrac;
        float titleH = innerH * TitleHeightFrac;
        Vector3 titlePos = new Vector3(-innerW * 0.5f + titleW * 0.5f, innerH * TitleYFrac, -0.01f);

        float bodyW = innerW;
        float bodyH = innerH * BodyHeightFrac;
        Vector3 bodyPos = new Vector3(0f, innerH * BodyYFrac, -0.01f);

        float authorW = innerW * 0.6f;
        float authorH = innerH * 0.10f;
        Vector3 authorPos = new Vector3(-innerW * 0.5f + authorW * 0.5f, innerH * AuthorYFrac, -0.01f);

        int sortingOrder = sr != null ? sr.sortingOrder + 5 : 5;
        int sortingLayer = sr != null ? sr.sortingLayerID : 0;

        MakeText("Title", wrapper.transform, paper.Title,
            fontSize: TitleFontSize, style: FontStyles.Bold, color: Color.black,
            sizeDelta: new Vector2(titleW, titleH), localPos: titlePos,
            align: TextAlignmentOptions.TopLeft,
            sortingLayer: sortingLayer, sortingOrder: sortingOrder);

        MakeText("Body", wrapper.transform, paper.Body,
            fontSize: BodyFontSize, style: FontStyles.Normal, color: Color.black,
            sizeDelta: new Vector2(bodyW, bodyH), localPos: bodyPos,
            align: TextAlignmentOptions.TopLeft,
            sortingLayer: sortingLayer, sortingOrder: sortingOrder);

        MakeText("Author", wrapper.transform, paper.Author,
            fontSize: AuthorFontSize, style: FontStyles.Italic, color: Color.black,
            sizeDelta: new Vector2(authorW, authorH), localPos: authorPos,
            align: TextAlignmentOptions.MidlineLeft,
            sortingLayer: sortingLayer, sortingOrder: sortingOrder);

        Debug.Log($"[BuildNewspaperContent] #{orderFromLeft} \"{template.name}\" populated: \"{paper.Title}\".");
    }

    static GameObject MakeText(
        string name, Transform parent, string text,
        float fontSize, FontStyles style, Color color,
        Vector2 sizeDelta, Vector3 localPos, TextAlignmentOptions align,
        int sortingLayer, int sortingOrder)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPos;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;

        var tmp = go.AddComponent<TextMeshPro>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.fontStyle = style;
        tmp.color = color;
        tmp.alignment = align;
        tmp.enableAutoSizing = false;
        tmp.textWrappingMode = TextWrappingModes.Normal;
        tmp.overflowMode = TextOverflowModes.Overflow;
        tmp.rectTransform.sizeDelta = sizeDelta;
        tmp.rectTransform.pivot = new Vector2(0.5f, 0.5f);

        var rend = go.GetComponent<MeshRenderer>();
        if (rend != null)
        {
            rend.sortingLayerID = sortingLayer;
            rend.sortingOrder = sortingOrder;
        }

        return go;
    }
}
