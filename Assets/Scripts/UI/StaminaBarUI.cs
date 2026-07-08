using UnityEngine;

// Ekran alt-orta merkezinde ince beyaz çizgi. Tam doluyken gizli, azalınca görünür.
// Sol ve sağ yarı, merkezden aynalı şekilde aynı anda büyüyüp küçülür.
public class StaminaBarUI : MonoBehaviour
{
    [SerializeField] private PlayerStatsController statsController;
    [SerializeField] private RectTransform leftBar;   // merkezden sola uzanan yarı
    [SerializeField] private RectTransform rightBar;  // merkezden sağa uzanan yarı
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Görünüm")]
    [SerializeField] private float maxHalfWidth = 220f; // tam dolu haldeki tek yarının genişliği

    private void OnEnable()
    {
        statsController.OnStaminaChanged += UpdateBar;
    }

    private void OnDisable()
    {
        statsController.OnStaminaChanged -= UpdateBar;
    }

    // normalized: 0 (boş) - 1 (dolu)
    private void UpdateBar(float normalized)
    {
        float width = maxHalfWidth * normalized;
        SetWidth(leftBar, width);
        SetWidth(rightBar, width);

        // Tam doluyken gizle, azalır azalmaz göster
        canvasGroup.alpha = normalized >= 0.999f ? 0f : 1f;
    }

    private void SetWidth(RectTransform rt, float width)
    {
        Vector2 size = rt.sizeDelta;
        size.x = width;
        rt.sizeDelta = size;
    }
}
