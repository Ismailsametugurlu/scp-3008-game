using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

// Stamina bar UI hiyerarşisini tek tıkla oluşturur: Tools > SCP3008 > Stamina Bar UI Kur
public static class StaminaBarSetupEditor
{
    private const float BorderThickness = 1f; // siyah çerçeve kalınlığı (px)

    [MenuItem("Tools/SCP3008/Stamina Bar UI Kur")]
    public static void SetupStaminaBar()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) { Debug.LogError("[StaminaBarSetup] Sahnede Canvas yok."); return; }

        // Zaten varsa sil, güncel haliyle yeniden kur
        Transform existing = canvas.transform.Find("StaminaBar");
        if (existing != null)
        {
            Object.DestroyImmediate(existing.gameObject);
            Debug.Log("[StaminaBarSetup] Var olan StaminaBar silindi, güncel haliyle yeniden kuruluyor.");
        }

        PlayerStatsController stats = Object.FindFirstObjectByType<PlayerStatsController>();
        if (stats == null) { Debug.LogError("[StaminaBarSetup] Sahnede PlayerStatsController yok."); return; }

        GameObject barRoot = new GameObject("StaminaBar", typeof(RectTransform), typeof(CanvasGroup));
        barRoot.transform.SetParent(canvas.transform, false);

        RectTransform rootRT = barRoot.GetComponent<RectTransform>();
        rootRT.anchorMin = new Vector2(0.5f, 0f);
        rootRT.anchorMax = new Vector2(0.5f, 0f);
        rootRT.pivot = new Vector2(0.5f, 0f);
        rootRT.anchoredPosition = new Vector2(0f, 40f);
        rootRT.sizeDelta = new Vector2(0f, 3f);

        RectTransform leftBar  = CreateBarHalf(barRoot.transform, "LeftBar", pivotX: 1f);
        RectTransform rightBar = CreateBarHalf(barRoot.transform, "RightBar", pivotX: 0f);

        StaminaBarUI barUI = barRoot.AddComponent<StaminaBarUI>();
        var so = new SerializedObject(barUI);
        so.FindProperty("statsController").objectReferenceValue = stats;
        so.FindProperty("leftBar").objectReferenceValue = leftBar;
        so.FindProperty("rightBar").objectReferenceValue = rightBar;
        so.FindProperty("canvasGroup").objectReferenceValue = barRoot.GetComponent<CanvasGroup>();
        so.ApplyModifiedProperties();

        EditorSceneManager.MarkSceneDirty(barRoot.scene);
        Debug.Log("[StaminaBarSetup] StaminaBar UI oluşturuldu.");
    }

    // Dış obje siyah (çerçeve), içine anchor-stretch ile inset edilmiş beyaz "Fill" çocuğu konur.
    // Böylece dış obje büyüyüp küçülürken çerçeve kalınlığı her zaman sabit kalır (vertex patlaması yok).
    private static RectTransform CreateBarHalf(Transform parent, string name, float pivotX)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(pivotX, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(0f, 3f);

        go.GetComponent<Image>().color = Color.black;

        GameObject fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
        fill.transform.SetParent(go.transform, false);

        RectTransform fillRT = fill.GetComponent<RectTransform>();
        fillRT.anchorMin = Vector2.zero;
        fillRT.anchorMax = Vector2.one;
        fillRT.offsetMin = new Vector2(BorderThickness, BorderThickness);
        fillRT.offsetMax = new Vector2(-BorderThickness, -BorderThickness);

        fill.GetComponent<Image>().color = Color.white;

        return rt;
    }
}
