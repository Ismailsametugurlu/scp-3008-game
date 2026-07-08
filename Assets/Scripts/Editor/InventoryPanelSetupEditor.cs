using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

// Tab ile açılan çanta panelini kurar (3 cep + 6 çanta slotu, drag-drop).
// Tools > SCP3008 > Envanter Paneli Kur
public static class InventoryPanelSetupEditor
{
    private const int PocketSlots = 3;
    private const int BackpackSlots = 6;
    private const int Columns = 3;
    private const float SlotSize = 64f;
    private const float SlotGap = 10f;

    [MenuItem("Tools/SCP3008/Envanter Paneli Kur")]
    public static void SetupPanel()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) { Debug.LogError("[EnvanterPaneli] Sahnede Canvas yok."); return; }

        Transform existing = canvas.transform.Find("InventoryPanel");
        if (existing != null) Object.DestroyImmediate(existing.gameObject);

        PlayerInventory inventory = Object.FindFirstObjectByType<PlayerInventory>();
        if (inventory == null) { Debug.LogError("[EnvanterPaneli] Sahnede PlayerInventory yok. Önce Player'a ekle."); return; }

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        int totalSlots = PocketSlots + BackpackSlots;

        // Kök (her zaman aktif, panelRoot'u ve ghost'u tutar)
        GameObject root = new GameObject("InventoryPanel", typeof(RectTransform));
        root.transform.SetParent(canvas.transform, false);
        StretchFull(root.GetComponent<RectTransform>());

        // Açılıp kapanan görsel kısım (koyu arka plan + slotlar)
        GameObject panel = new GameObject("PanelRoot", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(root.transform, false);
        RectTransform panelRT = panel.GetComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(0.5f, 0.5f);
        panelRT.anchorMax = new Vector2(0.5f, 0.5f);
        panelRT.pivot = new Vector2(0.5f, 0.5f);
        int rows = Mathf.CeilToInt(totalSlots / (float)Columns);
        float gridW = Columns * SlotSize + (Columns - 1) * SlotGap;
        float gridH = rows * SlotSize + (rows - 1) * SlotGap;
        float padding = 28f;
        panelRT.sizeDelta = new Vector2(gridW + padding * 2f, gridH + padding * 2f + 30f);
        panelRT.anchoredPosition = Vector2.zero;
        panel.GetComponent<Image>().color = new Color(0.06f, 0.06f, 0.08f, 0.92f);

        // Başlık
        GameObject title = new GameObject("Title", typeof(RectTransform), typeof(Text));
        title.transform.SetParent(panel.transform, false);
        RectTransform titleRT = title.GetComponent<RectTransform>();
        titleRT.anchorMin = new Vector2(0f, 1f); titleRT.anchorMax = new Vector2(1f, 1f);
        titleRT.pivot = new Vector2(0.5f, 1f);
        titleRT.anchoredPosition = new Vector2(0f, -8f);
        titleRT.sizeDelta = new Vector2(0f, 28f);
        Text titleText = title.GetComponent<Text>();
        titleText.text = "ÇANTA";
        titleText.font = font; titleText.fontSize = 16; titleText.fontStyle = FontStyle.Bold;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = new Color(1f, 0.85f, 0.2f);

        var slotUIs = new List<InventorySlotUI>();
        float startX = -gridW * 0.5f + SlotSize * 0.5f;
        float startY = gridH * 0.5f - SlotSize * 0.5f - 12f; // başlık için biraz aşağı

        for (int i = 0; i < totalSlots; i++)
        {
            int col = i % Columns;
            int rowIdx = i / Columns;
            float x = startX + col * (SlotSize + SlotGap);
            float y = startY - rowIdx * (SlotSize + SlotGap);

            InventorySlotUI slotUI = CreateSlot(panel.transform, i, font, new Vector2(x, y));
            slotUIs.Add(slotUI);
        }

        // Sürükleme ghost'u (kök altında, en üstte çizilsin diye en sonda)
        GameObject ghost = new GameObject("DragGhost", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
        ghost.transform.SetParent(root.transform, false);
        RectTransform ghostRT = ghost.GetComponent<RectTransform>();
        ghostRT.sizeDelta = new Vector2(SlotSize, SlotSize);
        ghost.GetComponent<Image>().color = new Color(1f, 0.85f, 0.2f, 0.7f);
        ghost.GetComponent<CanvasGroup>().blocksRaycasts = false; // drop'u engellemesin
        GameObject ghostLbl = new GameObject("Label", typeof(RectTransform), typeof(Text));
        ghostLbl.transform.SetParent(ghost.transform, false);
        StretchFull(ghostLbl.GetComponent<RectTransform>());
        Text ghostText = ghostLbl.GetComponent<Text>();
        ghostText.font = font; ghostText.fontSize = 16; ghostText.alignment = TextAnchor.MiddleCenter;
        ghostText.color = Color.black;

        // Panel script'i bağla
        InventoryPanelUI panelUI = root.AddComponent<InventoryPanelUI>();
        var so = new SerializedObject(panelUI);
        so.FindProperty("inventory").objectReferenceValue = inventory;
        so.FindProperty("panelRoot").objectReferenceValue = panel;
        so.FindProperty("pocketSlotCount").intValue = PocketSlots;
        so.FindProperty("dragGhost").objectReferenceValue = ghostRT;
        so.FindProperty("dragGhostLabel").objectReferenceValue = ghostText;
        SerializedProperty slotsProp = so.FindProperty("slots");
        slotsProp.arraySize = slotUIs.Count;
        for (int i = 0; i < slotUIs.Count; i++)
            slotsProp.GetArrayElementAtIndex(i).objectReferenceValue = slotUIs[i];
        so.ApplyModifiedProperties();

        EditorSceneManager.MarkSceneDirty(root.scene);
        Debug.Log("[EnvanterPaneli] InventoryPanel oluşturuldu (Tab ile aç/kapa).");
    }

    private static InventorySlotUI CreateSlot(Transform parent, int index, Font font, Vector2 pos)
    {
        GameObject slot = new GameObject($"Slot{index}", typeof(RectTransform), typeof(Image), typeof(InventorySlotUI));
        slot.transform.SetParent(parent, false);
        RectTransform rt = slot.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f); rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(SlotSize, SlotSize);
        Image bg = slot.GetComponent<Image>();
        bg.color = new Color(0.18f, 0.18f, 0.22f, 0.9f);

        GameObject label = new GameObject("Label", typeof(RectTransform), typeof(Text));
        label.transform.SetParent(slot.transform, false);
        StretchFull(label.GetComponent<RectTransform>());
        Text labelText = label.GetComponent<Text>();
        labelText.font = font; labelText.fontSize = 15; labelText.alignment = TextAnchor.MiddleCenter;
        labelText.color = Color.white;
        labelText.raycastTarget = false; // slot'un drop'unu engellemesin

        InventorySlotUI slotUI = slot.GetComponent<InventorySlotUI>();
        var so = new SerializedObject(slotUI);
        so.FindProperty("background").objectReferenceValue = bg;
        so.FindProperty("label").objectReferenceValue = labelText;
        so.ApplyModifiedProperties();
        return slotUI;
    }

    private static void StretchFull(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
    }
}
