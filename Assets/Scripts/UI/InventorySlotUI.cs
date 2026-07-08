using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Çanta panelindeki tek slot. Drag ile başka slota bırakılınca eşya taşınır/takas edilir.
public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image background;
    [SerializeField] private Text label;

    private InventoryPanelUI panel;
    private PlayerInventory inventory;
    private int slotIndex;

    // Renkler: cep slotu / çanta slotu / kilitli (çanta yokken)
    private static readonly Color PocketColor = new Color(0.18f, 0.18f, 0.22f, 0.9f);
    private static readonly Color BackpackColor = new Color(0.12f, 0.12f, 0.14f, 0.9f);
    private static readonly Color LockedColor = new Color(0.08f, 0.08f, 0.08f, 0.4f);

    public void Init(InventoryPanelUI panel, PlayerInventory inventory, int slotIndex, bool isPocket)
    {
        this.panel = panel;
        this.inventory = inventory;
        this.slotIndex = slotIndex;
        Refresh(isPocket);
    }

    // Slot içeriğini ve durumunu (aktif/kilitli) günceller
    public void Refresh(bool isPocket)
    {
        bool active = slotIndex < inventory.TotalSize;
        ItemStack stack = inventory.GetSlot(slotIndex);

        label.text = (active && !stack.isEmpty) ? Label(stack.type) : "";
        background.color = !active ? LockedColor : (isPocket ? PocketColor : BackpackColor);
    }

    public void OnBeginDrag(PointerEventData e)
    {
        if (slotIndex >= inventory.TotalSize) return;
        if (inventory.GetSlot(slotIndex).isEmpty) return;
        panel.BeginDrag(slotIndex, label.text);
    }

    public void OnDrag(PointerEventData e) { } // ghost taşımayı panel yapıyor

    public void OnEndDrag(PointerEventData e) => panel.EndDrag();

    public void OnDrop(PointerEventData e)
    {
        if (slotIndex < inventory.TotalSize) panel.DropOn(slotIndex);
    }

    private string Label(ConsumableType type) => type == ConsumableType.Water ? "SU" : "YEM";
}
