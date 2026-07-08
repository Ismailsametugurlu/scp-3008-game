using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

// Büyük Can dairesi (sol alt) + 5 küçük rozet (Su/Açlık/Uyku/Kas/Zeka), rozetler
// Can dairesinin 120°-330° yayında (üstten geçen kısa yol) dizilir. Tools > SCP3008 > Survival Gauge HUD Kur
public static class SurvivalStatsHUDSetupEditor
{
    private const float EdgePadding   = 24f;
    private const float HealthRadius  = 56f;  // büyük Can dairesi
    private const float BadgeRadius   = 13f;  // küçük rozetler
    private const float BadgeGap      = 6f;   // Can dairesi kenarı ile rozet arası boşluk
    private const float BackgroundAlpha = 0.75f; // ortası koyu, hafif saydam (görseldeki gibi)
    private const float BorderThickness = 2f;    // ince siyah dış çerçeve

    // Rozetler 120°'den 330°'ye (üstten geçen kısa yol: 105,75,45,15,-15) diziliyor
    private const float ArcStartDeg = 120f;
    private const float ArcEndDeg   = -30f; // 330°'ye eşdeğer

    private static readonly string[] OldObjectNames =
    {
        "HealthSlider", "HungerSlider", "EnergySlider", "WaterSlider", "StatsHUD", "SurvivalStatsHUD"
    };

    private struct BadgeDef
    {
        public string Name;
        public string Label;
        public Color Color;
        public float AngleDeg; // 0 = Can dairesinin sağı (Doğu), 90 = üstü (Kuzey)

        public BadgeDef(string name, string label, Color color, float angleDeg)
        {
            Name = name; Label = label; Color = color; AngleDeg = angleDeg;
        }
    }

    [MenuItem("Tools/SCP3008/Survival Gauge HUD Kur")]
    public static void SetupSurvivalGaugeHUD()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) { Debug.LogError("[SurvivalGaugeHUD] Sahnede Canvas yok."); return; }

        RemoveOldObjects(canvas.transform);

        PlayerStatsController stats = Object.FindFirstObjectByType<PlayerStatsController>();
        if (stats == null) { Debug.LogError("[SurvivalGaugeHUD] Sahnede PlayerStatsController yok."); return; }

        GameObject root = new GameObject("SurvivalStatsHUD", typeof(RectTransform));
        root.transform.SetParent(canvas.transform, false);
        RectTransform rootRT = root.GetComponent<RectTransform>();
        rootRT.anchorMin = Vector2.zero;
        rootRT.anchorMax = Vector2.zero;
        rootRT.pivot = Vector2.zero;
        rootRT.anchoredPosition = Vector2.zero;
        rootRT.sizeDelta = Vector2.zero;

        Vector2 healthCenter = new Vector2(EdgePadding + HealthRadius, EdgePadding + HealthRadius);

        SurvivalGaugeUI healthGauge = CreateCircle(root.transform, "HealthGauge", healthCenter, HealthRadius,
            new Color(0.85f, 0.15f, 0.15f), "CAN");

        // 120°'den 330°'ye (üstten geçen kısa yol) 5 eş aralıklı rozet: 105,75,45,15,-15
        BadgeDef[] badges =
        {
            new BadgeDef("WaterGauge",  "SU",    new Color(0.2f, 0.55f, 0.85f),  105f),
            new BadgeDef("HungerGauge", "ACLIK", new Color(0.85f, 0.55f, 0.15f), 75f),
            new BadgeDef("SleepGauge",  "UYKU",  new Color(0.55f, 0.35f, 0.85f), 45f),
            new BadgeDef("MuscleGauge", "KAS",   new Color(0.7f, 0.3f, 0.2f),    15f),
            new BadgeDef("IntGauge",    "ZEKA",  new Color(0.2f, 0.75f, 0.75f),  -15f),
        };

        float distFromCenter = HealthRadius + BadgeGap + BadgeRadius;
        SurvivalGaugeUI waterGauge = null, hungerGauge = null, sleepGauge = null;
        LeveledGaugeUI muscleGauge = null, intGauge = null;

        foreach (BadgeDef b in badges)
        {
            float rad = b.AngleDeg * Mathf.Deg2Rad;
            Vector2 pos = healthCenter + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * distFromCenter;

            if (b.Name == "MuscleGauge")
            {
                muscleGauge = CreateLeveledCircle(root.transform, b.Name, pos, BadgeRadius, b.Color, b.Color, b.Label, 5);
            }
            else if (b.Name == "IntGauge")
            {
                Color gold = new Color(1f, 0.85f, 0.2f);
                intGauge = CreateLeveledCircle(root.transform, b.Name, pos, BadgeRadius, b.Color, gold, b.Label, 5);
            }
            else
            {
                SurvivalGaugeUI gauge = CreateCircle(root.transform, b.Name, pos, BadgeRadius, b.Color, b.Label);
                if (b.Name == "WaterGauge") waterGauge = gauge;
                else if (b.Name == "HungerGauge") hungerGauge = gauge;
                else if (b.Name == "SleepGauge") sleepGauge = gauge;
            }
        }

        SurvivalStatsHUD hud = root.AddComponent<SurvivalStatsHUD>();
        var so = new SerializedObject(hud);
        so.FindProperty("statsController").objectReferenceValue = stats;
        so.FindProperty("healthGauge").objectReferenceValue = healthGauge;
        so.FindProperty("waterGauge").objectReferenceValue = waterGauge;
        so.FindProperty("hungerGauge").objectReferenceValue = hungerGauge;
        so.FindProperty("sleepGauge").objectReferenceValue = sleepGauge;
        so.FindProperty("muscleGauge").objectReferenceValue = muscleGauge;
        so.FindProperty("intelligenceGauge").objectReferenceValue = intGauge;
        so.ApplyModifiedProperties();

        EditorSceneManager.MarkSceneDirty(root.scene);
        Debug.Log("[SurvivalGaugeHUD] Büyük Can dairesi + 5 rozet oluşturuldu.");
    }

    private static void RemoveOldObjects(Transform canvasTransform)
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

    // Basit sürekli (0-1) dolan daire: arka plan (yarı opak) + renkli halka + etiket
    private static SurvivalGaugeUI CreateCircle(Transform parent, string name, Vector2 center, float radius, Color color, string label)
    {
        GameObject root = BuildCircleBase(parent, name, center, radius, color, label, out Image fillImg);
        SurvivalGaugeUI gauge = root.AddComponent<SurvivalGaugeUI>();
        var so = new SerializedObject(gauge);
        so.FindProperty("fillImage").objectReferenceValue = fillImg;
        so.ApplyModifiedProperties();
        return gauge;
    }

    // Seviyeli (Zeka/Kas) daire: aynı görünüm, max seviyede renk değişir
    private static LeveledGaugeUI CreateLeveledCircle(Transform parent, string name, Vector2 center, float radius,
        Color normalColor, Color maxColor, string label, int maxLevel)
    {
        GameObject root = BuildCircleBase(parent, name, center, radius, normalColor, label, out Image fillImg);
        LeveledGaugeUI gauge = root.AddComponent<LeveledGaugeUI>();
        var so = new SerializedObject(gauge);
        so.FindProperty("fillImage").objectReferenceValue = fillImg;
        so.FindProperty("normalColor").colorValue = normalColor;
        so.FindProperty("maxLevelColor").colorValue = maxColor;
        so.FindProperty("maxLevel").intValue = maxLevel;
        so.ApplyModifiedProperties();
        return gauge;
    }

    // Ortak daire yapısı: dış halka doluyor/azalıyor, merkez sabit maske + ikon (durum çevreden anlaşılır)
    private static GameObject BuildCircleBase(Transform parent, string name, Vector2 center, float radius, Color color, string label, out Image fillImg)
    {
        Sprite knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject root = new GameObject(name, typeof(RectTransform));
        root.transform.SetParent(parent, false);
        RectTransform rootRT = root.GetComponent<RectTransform>();
        rootRT.anchorMin = Vector2.zero;
        rootRT.anchorMax = Vector2.zero;
        rootRT.pivot = new Vector2(0.5f, 0.5f);
        rootRT.anchoredPosition = center;
        rootRT.sizeDelta = new Vector2(radius * 2f, radius * 2f);

        // İnce siyah dış çerçeve (biraz daha büyük, en altta)
        GameObject border = new GameObject("Border", typeof(RectTransform), typeof(Image));
        border.transform.SetParent(root.transform, false);
        RectTransform borderRT = border.GetComponent<RectTransform>();
        borderRT.anchorMin = Vector2.zero; borderRT.anchorMax = Vector2.one;
        borderRT.offsetMin = new Vector2(-BorderThickness, -BorderThickness);
        borderRT.offsetMax = new Vector2(BorderThickness, BorderThickness);
        Image borderImg = border.GetComponent<Image>();
        borderImg.sprite = knob;
        borderImg.color = Color.black;

        GameObject bg = new GameObject("Background", typeof(RectTransform), typeof(Image));
        bg.transform.SetParent(root.transform, false);
        RectTransform bgRT = bg.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero; bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero; bgRT.offsetMax = Vector2.zero;
        Image bgImg = bg.GetComponent<Image>();
        bgImg.sprite = knob;
        bgImg.color = new Color(0.05f, 0.05f, 0.05f, BackgroundAlpha);

        GameObject fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
        fill.transform.SetParent(root.transform, false);
        RectTransform fillRT = fill.GetComponent<RectTransform>();
        fillRT.anchorMin = Vector2.zero; fillRT.anchorMax = Vector2.one;
        fillRT.offsetMin = Vector2.zero; fillRT.offsetMax = Vector2.zero;
        fillImg = fill.GetComponent<Image>();
        fillImg.sprite = knob;
        fillImg.color = color;
        fillImg.type = Image.Type.Filled;
        fillImg.fillMethod = Image.FillMethod.Radial360;
        fillImg.fillOrigin = (int)Image.Origin360.Top;
        fillImg.fillClockwise = true;
        fillImg.fillAmount = 1f;

        // Merkez maskesi: dolum ortada değil, dış halkada görünsün diye ortayı kapatır
        float innerRadius = radius * 0.62f;
        GameObject mask = new GameObject("CenterMask", typeof(RectTransform), typeof(Image));
        mask.transform.SetParent(root.transform, false);
        RectTransform maskRT = mask.GetComponent<RectTransform>();
        maskRT.anchorMin = new Vector2(0.5f, 0.5f);
        maskRT.anchorMax = new Vector2(0.5f, 0.5f);
        maskRT.pivot = new Vector2(0.5f, 0.5f);
        maskRT.anchoredPosition = Vector2.zero;
        maskRT.sizeDelta = new Vector2(innerRadius * 2f, innerRadius * 2f);
        Image maskImg = mask.GetComponent<Image>();
        maskImg.sprite = knob;
        maskImg.color = new Color(0.05f, 0.05f, 0.05f, 0.9f);

        GameObject labelGO = new GameObject("Label", typeof(RectTransform), typeof(Text));
        labelGO.transform.SetParent(root.transform, false);
        RectTransform labelRT = labelGO.GetComponent<RectTransform>();
        labelRT.anchorMin = Vector2.zero; labelRT.anchorMax = Vector2.one;
        labelRT.offsetMin = Vector2.zero; labelRT.offsetMax = Vector2.zero;
        Text labelText = labelGO.GetComponent<Text>();
        labelText.text = label;
        labelText.font = font;
        labelText.fontSize = Mathf.Max(8, (int)(radius * 0.28f));
        labelText.alignment = TextAnchor.MiddleCenter;
        labelText.color = Color.white;

        return root;
    }
}
