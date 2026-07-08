using UnityEngine;
using UnityEngine.UI;

// Alt-orta hotbar: cep slotlarını gösterir, seçili slotu vurgular, içeriği etiketler
public class HotbarUI : MonoBehaviour
{
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private Image[] slotBackgrounds; // her slotun çerçeve/arka planı
    [SerializeField] private Text[] slotLabels;       // her slotun içerik yazısı (şimdilik ikon yerine)

    [SerializeField] private Color selectedColor = new Color(1f, 0.85f, 0.2f, 0.9f);
    [SerializeField] private Color normalColor   = new Color(0.1f, 0.1f, 0.1f, 0.75f);

    private void OnEnable()
    {
        inventory.OnInventoryChanged += Refresh;
        inventory.OnSelectionChanged += OnSelectionChanged;
    }

    private void OnDisable()
    {
        inventory.OnInventoryChanged -= Refresh;
        inventory.OnSelectionChanged -= OnSelectionChanged;
    }

    private void OnSelectionChanged(int _) => Refresh();

    private void Start() => Refresh();

    // Tüm hotbar slotlarını envanter durumuna göre günceller
    private void Refresh()
    {
        for (int i = 0; i < slotBackgrounds.Length; i++)
        {
            bool selected = i == inventory.SelectedIndex;
            slotBackgrounds[i].color = selected ? selectedColor : normalColor;

            ItemStack stack = inventory.GetSlot(i);
            slotLabels[i].text = stack.isEmpty ? "" : Label(stack.type);
        }
    }

    private string Label(ConsumableType type) => type == ConsumableType.Water ? "SU" : "YEM";
}
