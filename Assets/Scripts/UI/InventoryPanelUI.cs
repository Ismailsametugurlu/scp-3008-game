using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

// Tab ile açılan çanta paneli. Açıkken fare serbest + oyuncu girdisi kilitli.
// Slotlar arası drag-drop taşımayı koordine eder.
public class InventoryPanelUI : MonoBehaviour
{
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private GameObject panelRoot;   // açılıp kapanan görsel kısım
    [SerializeField] private InventorySlotUI[] slots; // ilk pocketSlots = cep, gerisi çanta
    [SerializeField] private int pocketSlotCount = 3;
    [SerializeField] private RectTransform dragGhost; // sürüklerken fareyi takip eden etiket
    [SerializeField] private Text dragGhostLabel;

    private bool isOpen;
    private int draggedIndex = -1;

    private void OnEnable() => inventory.OnInventoryChanged += RefreshSlots;
    private void OnDisable() => inventory.OnInventoryChanged -= RefreshSlots;

    private void Start()
    {
        for (int i = 0; i < slots.Length; i++)
            slots[i].Init(this, inventory, i, i < pocketSlotCount);

        panelRoot.SetActive(false);
        dragGhost.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame) Toggle();

        // Sürükleme sırasında ghost etiketi fareyi takip eder
        if (draggedIndex >= 0)
            dragGhost.position = Mouse.current.position.ReadValue();
    }

    private void Toggle()
    {
        isOpen = !isOpen;
        panelRoot.SetActive(isOpen);

        PlayerInputLock.IsLocked = isOpen;
        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isOpen;

        if (isOpen) RefreshSlots();
        else EndDrag();
    }

    private void RefreshSlots()
    {
        for (int i = 0; i < slots.Length; i++)
            slots[i].Refresh(i < pocketSlotCount);
    }

    // Slot sürüklemeye başlayınca çağrılır
    public void BeginDrag(int index, string labelText)
    {
        draggedIndex = index;
        dragGhostLabel.text = labelText;
        dragGhost.gameObject.SetActive(true);
    }

    // Sürükleme bitince (bırakma başarılı olsa da olmasa da) ghost kapanır
    public void EndDrag()
    {
        draggedIndex = -1;
        dragGhost.gameObject.SetActive(false);
    }

    // Bir slotun üzerine bırakılınca taşıma yapılır
    public void DropOn(int targetIndex)
    {
        if (draggedIndex >= 0)
            inventory.MoveItem(draggedIndex, targetIndex);
    }
}
