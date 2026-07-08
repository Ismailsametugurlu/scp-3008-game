using UnityEngine;
using UnityEngine.InputSystem;

// E: yakındaki eşyayı anında envantere alır. Sağ tık (basılı tut): elde tutulanı tüketir.
public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float interactRange = 2.5f;
    [SerializeField] private float eatHoldDuration = 1.5f; // TASLAK, sonra ayarlanacak (kitap okuma da bunu kullanacak)

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
        HandlePickup();
        HandleEat();
    }

    // E'ye basınca (basılı tutmadan) en yakın eşyayı anında envantere alır
    private void HandlePickup()
    {
        if (!Keyboard.current.eKey.wasPressedThisFrame) return;

        ConsumableItem item = FindNearestConsumable();
        if (item != null) item.PickUp(inventory);
    }

    // Sağ tık basılı tutulunca elde tutulan eşya (önce yemek, sonra su) tüketilir
    private void HandleEat()
    {
        bool hasAnything = inventory.HasFood || inventory.HasWater;
        bool wantsEat = Mouse.current.rightButton.isPressed && hasAnything;

        if (!wantsEat)
        {
            holdTimer = 0f;
            IsEating = false;
            return;
        }

        CurrentEatType = inventory.HasFood ? ConsumableType.Food : ConsumableType.Water;
        IsEating = true;
        holdTimer += Time.deltaTime;

        if (holdTimer >= eatHoldDuration)
        {
            inventory.TryConsume(CurrentEatType.Value, stats);
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
