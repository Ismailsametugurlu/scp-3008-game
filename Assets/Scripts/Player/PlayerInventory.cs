using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Envanterdeki tek bir slotun içeriği (şimdilik sadece tüketilebilir; ileride malzeme/silah eklenir)
[Serializable]
public struct ItemStack
{
    public bool isEmpty;
    public ConsumableType type;
    public float amount;

    public static ItemStack Empty => new ItemStack { isEmpty = true };
}

// Slot-tabanlı envanter: ilk 'pocketSlots' = hotbar (cep, elde tutulur), geri kalan = çanta (depo).
// Çanta ancak takılınca (hasBackpack) açılır. Seçili hotbar slotu elde tutulan eşyadır.
public class PlayerInventory : MonoBehaviour
{
    [Header("Kapasite")]
    [SerializeField] private int pocketSlots = 3;   // oyun başı cep
    [SerializeField] private int backpackSlots = 6; // çanta takılınca eklenen

    private ItemStack[] slots;
    private bool hasBackpack;
    private int selectedIndex; // sadece hotbar (cep) içinde: 0 .. pocketSlots-1

    // UI bu event'leri dinler
    public event Action OnInventoryChanged;      // slot içerikleri değişti
    public event Action<int> OnSelectionChanged; // seçili hotbar slotu değişti

    public int HotbarSize => pocketSlots;
    public int TotalSize => pocketSlots + (hasBackpack ? backpackSlots : 0);
    public int SelectedIndex => selectedIndex;

    public ItemStack GetSlot(int i) => slots[i];
    public ItemStack HeldItem => slots[selectedIndex];

    private void Awake()
    {
        // Başlangıçta sadece cep açık, hepsi boş
        slots = new ItemStack[pocketSlots + backpackSlots];
        for (int i = 0; i < slots.Length; i++) slots[i] = ItemStack.Empty;
    }

    private void Update()
    {
        HandleSelectionInput();
    }

    // 1-2-3 tuşları ve fare tekeri ile hotbar slotu seç
    private void HandleSelectionInput()
    {
        var kb = Keyboard.current;
        if (kb.digit1Key.wasPressedThisFrame) SetSelected(0);
        if (kb.digit2Key.wasPressedThisFrame) SetSelected(1);
        if (kb.digit3Key.wasPressedThisFrame) SetSelected(2);

        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll > 0.01f) SetSelected((selectedIndex + 1) % pocketSlots);
        else if (scroll < -0.01f) SetSelected((selectedIndex - 1 + pocketSlots) % pocketSlots);
    }

    private void SetSelected(int index)
    {
        if (index < 0 || index >= pocketSlots || index == selectedIndex) return;
        selectedIndex = index;
        OnSelectionChanged?.Invoke(selectedIndex);
    }

    // Eşya al: ilk boş slota koy (hotbar önce, sonra çanta). Yer yoksa false.
    public bool Add(ConsumableType type, float amount)
    {
        for (int i = 0; i < TotalSize; i++)
        {
            if (slots[i].isEmpty)
            {
                slots[i] = new ItemStack { isEmpty = false, type = type, amount = amount };
                OnInventoryChanged?.Invoke();
                return true;
            }
        }
        return false; // envanter dolu
    }

    // Seçili hotbar slotundaki eşyayı tüket (stats'a uygula, slotu boşalt)
    public bool ConsumeSelected(PlayerStatsController stats)
    {
        ItemStack held = slots[selectedIndex];
        if (held.isEmpty) return false;

        if (held.type == ConsumableType.Food) stats.Eat(held.amount);
        else stats.Drink(held.amount);

        slots[selectedIndex] = ItemStack.Empty;
        OnInventoryChanged?.Invoke();
        return true;
    }

    // Çanta craftlanıp takılınca çağrılır — ekstra slotları açar (ileride crafting bağlar)
    public void AttachBackpack()
    {
        if (hasBackpack) return;
        hasBackpack = true;
        OnInventoryChanged?.Invoke();
    }
}
