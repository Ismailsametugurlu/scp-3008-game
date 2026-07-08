using UnityEngine;
using UnityEngine.InputSystem;

// E: yakındaki eşyayı anında envantere alır. Sağ tık (basılı tut): seçili hotbar eşyasını tüketir.
public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float interactRange = 2.5f;
    [SerializeField] private float eatHoldDuration = 1.5f; // TASLAK (kitap okuma da bunu kullanacak)

    private PlayerStatsController stats;
    private PlayerInventory inventory;
    private float holdTimer;

    // UI (dairesel gösterge) bunları okur
    public bool IsEating { get; private set; }
    public float EatProgress01 => Mathf.Clamp01(holdTimer / eatHoldDuration);
    public ConsumableType? CurrentEatType { get; private set; }

    private void Awake()
    {
        stats = GetComponent<PlayerStatsController>();
        inventory = GetComponent<PlayerInventory>();
    }

    private void Update()
    {
        if (PlayerInputLock.IsLocked) { IsEating = false; return; } // panel açıkken etkileşim durur

        HandlePickup();
        HandleEat();
    }

    // E'ye basınca en yakın eşyayı anında envantere alır (dolu değilse)
    private void HandlePickup()
    {
        if (!Keyboard.current.eKey.wasPressedThisFrame) return;

        ConsumableItem item = FindNearestConsumable();
        if (item != null) item.PickUp(inventory);
    }

    // Sağ tık basılı tutulunca seçili hotbar eşyası tüketilir
    private void HandleEat()
    {
        ItemStack held = inventory.HeldItem;
        bool wantsEat = Mouse.current.rightButton.isPressed && !held.isEmpty;

        if (!wantsEat)
        {
            holdTimer = 0f;
            IsEating = false;
            CurrentEatType = null;
            return;
        }

        CurrentEatType = held.type;
        IsEating = true;
        holdTimer += Time.deltaTime;

        if (holdTimer >= eatHoldDuration)
        {
            inventory.ConsumeSelected(stats);
            holdTimer = 0f;
            IsEating = false;
        }
    }

    private ConsumableItem FindNearestConsumable()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactRange);
        ConsumableItem nearest = null;
        float nearestDist = float.MaxValue;

        foreach (Collider hit in hits)
        {
            ConsumableItem item = hit.GetComponent<ConsumableItem>();
            if (item == null) continue;

            float dist = (hit.transform.position - transform.position).sqrMagnitude;
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = item;
            }
        }

        return nearest;
    }
}
