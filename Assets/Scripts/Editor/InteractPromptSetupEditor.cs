using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

// Nişangahın sağ-alt kısmında beliren dairesel "ye/iç" göstergesini kurar (PEAK tarzı).
// Tools > SCP3008 > Interact Prompt UI Kur
public static class InteractPromptSetupEditor
{
    private const float Radius = 20f;
    private const float OffsetX = 46f; // nişangah merkezinden sağa
    private const float OffsetY = -46f; // nişangah merkezinden aşağı

    [MenuItem("Tools/SCP3008/Interact Prompt UI Kur")]
    public static void SetupInteractPrompt()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) { Debug.LogError("[InteractPromptSetup] Sahnede Canvas yok."); return; }

        Transform existing = canvas.transform.Find("InteractPrompt");
        if (existing != null) Object.DestroyImmediate(existing.gameObject);

        PlayerInteractor interactor = Object.FindFirstObjectByType<PlayerInteractor>();
        if (interactor == null) { Debug.LogError("[InteractPromptSetup] Sahnede PlayerInteractor yok. Önce Player'a ekle."); return; }

        Sprite knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject root = new GameObject("InteractPrompt", typeof(RectTransform), typeof(CanvasGroup));
        root.transform.SetParent(canvas.transform, false);
        RectTransform rootRT = root.GetComponent<RectTransform>();
        rootRT.anchorMin = new Vector2(0.5f, 0.5f);
        rootRT.anchorMax = new Vector2(0.5f, 0.5f);
        rootRT.pivot = new Vector2(0.5f, 0.5f);
        rootRT.anchoredPosition = new Vector2(OffsetX, OffsetY);
        rootRT.sizeDelta = new Vector2(Radius * 2f, Radius * 2f);
        root.GetComponent<CanvasGroup>().alpha = 0f;

        GameObject bg = new GameObject("Background", typeof(RectTransform), typeof(Image));
        bg.transform.SetParent(root.transform, false);
        RectTransform bgRT = bg.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero; bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero; bgRT.offsetMax = Vector2.zero;
        Image bgImg = bg.GetComponent<Image>();
        bgImg.sprite = knob;
        bgImg.color = new Color(0.05f, 0.05f, 0.05f, 0.75f);

        GameObject fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
        fill.transform.SetParent(root.transform, false);
        RectTransform fillRT = fill.GetComponent<RectTransform>();
        fillRT.anchorMin = Vector2.zero; fillRT.anchorMax = Vector2.one;
        fillRT.offsetMin = Vector2.zero; fillRT.offsetMax = Vector2.zero;
        Image fillImg = fill.GetComponent<Image>();
        fillImg.sprite = knob;
        fillImg.color = new Color(1f, 0.85f, 0.2f);
        fillImg.type = Image.Type.Filled;
        fillImg.fillMethod = Image.FillMethod.Radial360;
        fillImg.fillOrigin = (int)Image.Origin360.Top;
        fillImg.fillClockwise = true;
        fillImg.fillAmount = 0f;

        GameObject labelGO = new GameObject("Label", typeof(RectTransform), typeof(Text));
        labelGO.transform.SetParent(root.transform, false);
        RectTransform labelRT = labelGO.GetComponent<RectTransform>();
        labelRT.anchorMin = Vector2.zero; labelRT.anchorMax = Vector2.one;
        labelRT.offsetMin = Vector2.zero; labelRT.offsetMax = Vector2.zero;
        Text labelText = labelGO.GetComponent<Text>();
        labelText.text = "YE";
        labelText.font = font;
        labelText.fontSize = 10;
        labelText.alignment = TextAnchor.MiddleCenter;
        labelText.color = Color.white;

        InteractPromptUI prompt = root.AddComponent<InteractPromptUI>();
        var so = new SerializedObject(prompt);
        so.FindProperty("interactor").objectReferenceValue = interactor;
        so.FindProperty("fillImage").objectReferenceValue = fillImg;
        so.FindProperty("canvasGroup").objectReferenceValue = root.GetComponent<CanvasGroup>();
        so.FindProperty("label").objectReferenceValue = labelText;
        so.ApplyModifiedProperties();

        EditorSceneManager.MarkSceneDirty(root.scene);
        Debug.Log("[InteractPromptSetup] InteractPrompt UI oluşturuldu.");
    }
}
