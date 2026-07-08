using UnityEngine;

public enum ConsumableType { Food, Water }

// Sahnedeki alınabilir yemek/su. E ile anında envantere eklenir (elde tutmak için).
public class ConsumableItem : MonoBehaviour
{
    [SerializeField] private ConsumableType type;
    [SerializeField] private float amount = 20f;

    public void PickUp(PlayerInventory inventory)
    {
        // Envanter doluysa alınmaz, eşya sahnede kalır
        if (inventory.Add(type, amount))
            Destroy(gameObject);
    }
}
