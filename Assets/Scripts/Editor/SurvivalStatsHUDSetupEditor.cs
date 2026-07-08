using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

// The Forest tarzı 3 dairesel gösterge (can/açlık/su) kurar: Tools > SCP3008 > Survival Gauge HUD Kur
// Eski düz slider'ları (HealthSlider/HungerSlider/EnergySlider/WaterSlider/StatsHUD) otomatik kaldırır.
public static class SurvivalStatsHUDSetupEditor
{
    private const float GaugeSize   = 56f;
    private const float GaugeGap    = 70f;
    private const float EdgePadding = 40f;

    private static readonly string[] OldObjectNames =
    {
        "HealthSlider", "HungerSlider", "EnergySlider", "WaterSlider", "StatsHUD"
    };

    [MenuItem("Tools/SCP3008/Survival Gauge HUD Kur")]
    public static void SetupSurvivalGaugeHUD()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) { Debug.LogError("[SurvivalGaugeHUD] Sahnede Canvas yok."); return; }

        RemoveOldLinearBars(canvas.transform);

        Transform existing = canvas.transform.Find("SurvivalStatsHUD");
        if (existing != null)
        {
            Object.DestroyImmediate(existing.gameObject);
            Debug.Log("[SurvivalGaugeHUD] Var olan SurvivalStatsHUD silindi, yeniden kuruluyor.");
        }

        PlayerStatsController stats = Object.FindFirstObjectByType<PlayerStatsController>();
        if (stats == null) { Debug.LogError("[SurvivalGaugeHUD] Sahnede PlayerStatsController yok."); return; }

        GameObject root = new GameObject("SurvivalStatsHUD", typeof(RectTransform));
        root.transform.SetParent(canvas.transform, false);

        RectTransform rootRT = root.GetComponent<RectTransform>();
        rootRT.anchorMin = new Vector2(0f, 0f);
        rootRT.anchorMax = new Vector2(0f, 0f);
        rootRT.pivot = new Vector2(0f, 0f);
        rootRT.anchoredPosition = new Vector2(EdgePadding, EdgePadding);
        rootRT.sizeDelta = new Vector2(GaugeGap * 3f, GaugeSize);

        SurvivalGaugeUI healthGauge = CreateGauge(root.transform, "HealthGauge", 0, new Color(0.85f, 0.15f, 0.15f), "CAN");
        SurvivalGaugeUI hungerGauge = CreateGauge(root.transform, "HungerGauge", 1, new Color(0.85f, 0.55f, 0.15f), "ACLIK");
        SurvivalGaugeUI waterGauge  = CreateGauge(root.transform, "WaterGauge",  2, new Color(0.2f, 0.55f, 0.85f), "SU");

        SurvivalStatsHUD hud = root.AddComponent<SurvivalStatsHUD>();
        var so = new SerializedObject(hud);
        so.FindProperty("statsController").objectReferenceValue = stats;
        so.FindProperty("healthGauge").objectReferenceValue = healthGauge;
        so.FindProperty("hungerGauge").objectReferenceValue = hungerGauge;
        so.FindProperty("waterGauge").objectReferenceValue = waterGauge;
        so.ApplyModifiedProperties();

        EditorSceneManager.MarkSceneDirty(root.scene);
        Debug.Log("[SurvivalGaugeHUD] SurvivalStatsHUD oluşturuldu.");
    }

    private static void RemoveOldLinearBars(Transform canvasTransform)
    {
        foreach (string name in OldObjectNames)
        {
            Transform t = canvasTransform.Find(name);
            if (t != null)
            {
                Object.DestroyImmediate(t.gameObject);
                Debug.Log($"[SurvivalGaugeHUD] Eski '{name}' kaldırıldı.");
            }
        }
    }

    private static SurvivalGaugeUI CreateGauge(Transform parent, string name, int index, Color fillColor, string label)
    {
        Sprite knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject root = new GameObject(name, typeof(RectTransform));
        root.transform.SetParent(parent, false);

        RectTransform rootRT = root.GetComponent<RectTransform>();
        rootRT.anchorMin = new Vector2(0f, 0f);
        rootRT.anchorMax = new Vector2(0f, 0f);
        rootRT.pivot = new Vector2(0f, 0f);
        rootRT.anchoredPosition = new Vector2(GaugeGap * index, 0f);
        rootRT.sizeDelta = new Vector2(GaugeSize, GaugeSize);

        // Arka plan (boş) halka
        GameObject bg = new GameObject("Background", typeof(RectTransform), typeof(Image));
        bg.transform.SetParent(root.transform, false);
        RectTransform bgRT = bg.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero; bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero; bgRT.offsetMax = Vector2.zero;
        Image bgImg = bg.GetComponent<Image>();
        bgImg.sprite = knob;
        bgImg.color = new Color(0.08f, 0.08f, 0.08f, 0.85f);

        // Dolan renkli halka
        GameObject fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
        fill.transform.SetParent(root.transform, false);
        RectTransform fillRT = fill.GetComponent<RectTransform>();
        fillRT.anchorMin = Vector2.zero; fillRT.anchorMax = Vector2.one;
        fillRT.offsetMin = Vector2.zero; fillRT.offsetMax = Vector2.zero;
        Image fillImg = fill.GetComponent<Image>();
        fillImg.sprite = knob;
        fillImg.color = fillColor;
        fillImg.type = Image.Type.Filled;
        fillImg.fillMethod = Image.FillMethod.Radial360;
        fillImg.fillOrigin = (int)Image.Origin360.Top;
        fillImg.fillClockwise = true;
        fillImg.fillAmount = 1f;

        // Ortadaki etiket (gerçek ikon gelene kadar placeholder metin)
        GameObject labelGO = new GameObject("Label", typeof(RectTransform), typeof(Text));
        labelGO.transform.SetParent(root.transform, false);
        RectTransform labelRT = labelGO.GetComponent<RectTransform>();
        labelRT.anchorMin = Vector2.zero; labelRT.anchorMax = Vector2.one;
        labelRT.offsetMin = Vector2.zero; labelRT.offsetMax = Vector2.zero;
        Text labelText = labelGO.GetComponent<Text>();
        labelText.text = label;
        labelText.font = font;
        labelText.fontSize = 11;
        labelText.alignment = TextAnchor.MiddleCenter;
        labelText.color = Color.white;

        SurvivalGaugeUI gauge = root.AddComponent<SurvivalGaugeUI>();
        var gaugeSO = new SerializedObject(gauge);
        gaugeSO.FindProperty("fillImage").objectReferenceValue = fillImg;
        gaugeSO.ApplyModifiedProperties();

        return gauge;
    }
}
