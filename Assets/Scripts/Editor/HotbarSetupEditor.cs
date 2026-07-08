using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

// Alt-orta hotbar UI'ını kurar (cep slot sayısı kadar kutu). Tools > SCP3008 > Hotbar UI Kur
public static class HotbarSetupEditor
{
    private const int SlotCount = 3; // PlayerInventory.pocketSlots ile aynı olmalı
    private const float SlotSize = 56f;
    private const float SlotGap = 8f;
    private const float BottomOffset = 90f; // stamina çizgisinin (y~40) üstünde kalsın

    [MenuItem("Tools/SCP3008/Hotbar UI Kur")]
    public static void SetupHotbar()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) { Debug.LogError("[HotbarSetup] Sahnede Canvas yok."); return; }

        Transform existing = canvas.transform.Find("Hotbar");
        if (existing != null) Object.DestroyImmediate(existing.gameObject);

        PlayerInventory inventory = Object.FindFirstObjectByType<PlayerInventory>();
        if (inventory == null) { Debug.LogError("[HotbarSetup] Sahnede PlayerInventory yok. Önce Player'a ekle."); return; }

        Sprite knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject root = new GameObject("Hotbar", typeof(RectTransform));
        root.transform.SetParent(canvas.transform, false);
        RectTransform rootRT = root.GetComponent<RectTransform>();
        rootRT.anchorMin = new Vector2(0.5f, 0f);
        rootRT.anchorMax = new Vector2(0.5f, 0f);
        rootRT.pivot = new Vector2(0.5f, 0f);
        rootRT.anchoredPosition = new Vector2(0f, BottomOffset);
        float totalWidth = SlotCount * SlotSize + (SlotCount - 1) * SlotGap;
        rootRT.sizeDelta = new Vector2(totalWidth, SlotSize);

        var backgrounds = new List<Image>();
        var labels = new List<Text>();

        for (int i = 0; i < SlotCount; i++)
        {
            float x = -totalWidth * 0.5f + SlotSize * 0.5f + i * (SlotSize + SlotGap);

            GameObject slot = new GameObject($"Slot{i}", typeof(RectTransform), typeof(Image));
            slot.transform.SetParent(root.transform, false);
            RectTransform slotRT = slot.GetComponent<RectTransform>();
            slotRT.anchorMin = new Vector2(0.5f, 0.5f);
            slotRT.anchorMax = new Vector2(0.5f, 0.5f);
            slotRT.pivot = new Vector2(0.5f, 0.5f);
            slotRT.anchoredPosition = new Vector2(x, 0f);
            slotRT.sizeDelta = new Vector2(SlotSize, SlotSize);
            Image slotImg = slot.GetComponent<Image>();
            slotImg.color = new Color(0.1f, 0.1f, 0.1f, 0.75f);
            backgrounds.Add(slotImg);

            // Slot numarası (küçük, sol üst köşe)
            GameObject num = new GameObject("Num", typeof(RectTransform), typeof(Text));
            num.transform.SetParent(slot.transform, false);
            RectTransform numRT = num.GetComponent<RectTransform>();
            numRT.anchorMin = new Vector2(0f, 1f);
            numRT.anchorMax = new Vector2(0f, 1f);
            numRT.pivot = new Vector2(0f, 1f);
            numRT.anchoredPosition = new Vector2(4f, -2f);
            numRT.sizeDelta = new Vector2(16f, 16f);
            Text numText = num.GetComponent<Text>();
            numText.text = (i + 1).ToString();
            numText.font = font;
            numText.fontSize = 11;
            numText.color = new Color(1f, 1f, 1f, 0.6f);

            // İçerik etiketi (ortada — ileride ikon gelecek)
            GameObject label = new GameObject("Label", typeof(RectTransform), typeof(Text));
            label.transform.SetParent(slot.transform, false);
            RectTransform labelRT = label.GetComponent<RectTransform>();
            labelRT.anchorMin = Vector2.zero; labelRT.anchorMax = Vector2.one;
            labelRT.offsetMin = Vector2.zero; labelRT.offsetMax = Vector2.zero;
            Text labelText = label.GetComponent<Text>();
            labelText.text = "";
            labelText.font = font;
            labelText.fontSize = 14;
            labelText.alignment = TextAnchor.MiddleCenter;
            labelText.color = Color.white;
            labels.Add(labelText);
        }

        HotbarUI hud = root.AddComponent<HotbarUI>();
        var so = new SerializedObject(hud);
        so.FindProperty("inventory").objectReferenceValue = inventory;

        SerializedProperty bgProp = so.FindProperty("slotBackgrounds");
        bgProp.arraySize = backgrounds.Count;
        for (int i = 0; i < backgrounds.Count; i++)
            bgProp.GetArrayElementAtIndex(i).objectReferenceValue = backgrounds[i];

        SerializedProperty lblProp = so.FindProperty("slotLabels");
        lblProp.arraySize = labels.Count;
        for (int i = 0; i < labels.Count; i++)
            lblProp.GetArrayElementAtIndex(i).objectReferenceValue = labels[i];

        so.ApplyModifiedProperties();

        EditorSceneManager.MarkSceneDirty(root.scene);
        Debug.Log("[HotbarSetup] Hotbar UI oluşturuldu.");
    }
}
