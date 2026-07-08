using UnityEngine;

public enum ConsumableType { Food, Water }

// Sahnedeki yenilebilir/içilebilir eşya. Küpe eklenir, Inspector'dan tip ve miktar ayarlanır.
public class ConsumableItem : MonoBehaviour
{
    [SerializeField] private ConsumableType type;
    [SerializeField] private float amount = 20f;

    public void Consume(PlayerStatsController stats)
    {
        if (type == ConsumableType.Food)
            stats.Eat(amount);
        else
            stats.Drink(amount);

        Destroy(gameObject);
    }
}
