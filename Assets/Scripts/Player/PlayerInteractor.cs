using UnityEngine;
using UnityEngine.InputSystem;

// Yakındaki yenilebilir/içilebilir eşyaları bulur; E'ye basılı tutunca tüketir
public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float interactRange = 2.5f;
    [SerializeField] private float interactHoldDuration = 1.5f; // TASLAK, sonra ayarlanacak (kitap okuma da bunu kullanacak)

    private PlayerStatsController stats;
    private ConsumableItem currentTarget;
    private float holdTimer;

    private void Awake()
    {
        stats = GetComponent<PlayerStatsController>();
    }

    private void Update()
    {
        ConsumableItem nearest = FindNearestConsumable();

        // Hedef değiştiyse sayaç sıfırlanır (başka eşyaya geçince baştan başlar)
        if (nearest != currentTarget)
        {
            currentTarget = nearest;
            holdTimer = 0f;
        }

        if (currentTarget == null) return;

        if (Keyboard.current.eKey.isPressed)
        {
            holdTimer += Time.deltaTime;
            if (holdTimer >= interactHoldDuration)
            {
                currentTarget.Consume(stats);
                currentTarget = null;
                holdTimer = 0f;
            }
        }
        else
        {
            holdTimer = 0f;
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
