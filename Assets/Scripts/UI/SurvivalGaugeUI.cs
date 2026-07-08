using UnityEngine;
using UnityEngine.UI;

// Tek bir dairesel gösterge (can/açlık/su). Fill Image'ı Radial360 doldurur/boşaltır.
public class SurvivalGaugeUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    public void SetValue(float normalized)
    {
        fillImage.fillAmount = Mathf.Clamp01(normalized);
    }
}
