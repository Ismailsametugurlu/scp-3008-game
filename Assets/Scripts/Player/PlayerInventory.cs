using System;
using UnityEngine;

// Basit envanter: yemek/su tek slot (stacklemez). Görsel hotbar ileride eklenecek.
public class PlayerInventory : MonoBehaviour
{
    public bool  HasFood { get; private set; }
    public float HeldFoodAmount { get; private set; }
    public bool  HasWater { get; private set; }
    public float HeldWaterAmount { get; private set; }

    public event Action<bool> OnFoodSlotChanged;
    public event Action<bool> OnWaterSlotChanged;

    // Eşya alınınca çağrılır; aynı türden zaten varsa üzerine yazar (stacklemez)
    public void Add(ConsumableType type, float amount)
    {
        if (type == ConsumableType.Food)
        {
            HasFood = true;
            HeldFoodAmount = amount;
            OnFoodSlotChanged?.Invoke(true);
        }
        else
        {
            HasWater = true;
            HeldWaterAmount = amount;
            OnWaterSlotChanged?.Invoke(true);
        }
    }

    // Elde tutulan eşyayı tüketir (stats'a uygular), slotu boşaltır
    public bool TryConsume(ConsumableType type, PlayerStatsController stats)
    {
        if (type == ConsumableType.Food && HasFood)
        {
            stats.Eat(HeldFoodAmount);
            HasFood = false;
            OnFoodSlotChanged?.Invoke(false);
            return true;
        }

        if (type == ConsumableType.Water && HasWater)
        {
            stats.Drink(HeldWaterAmount);
            HasWater = false;
            OnWaterSlotChanged?.Invoke(false);
            return true;
        }

        return false;
    }
}
