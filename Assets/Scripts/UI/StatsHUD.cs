using UnityEngine;
using UnityEngine.UI;

// 3 slider barı PlayerStatsController event'lerine bağlar
public class StatsHUD : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider hungerSlider;
    [SerializeField] private Slider waterSlider;

    [SerializeField] private PlayerStatsController statsController;

    private void OnEnable()
    {
        statsController.OnHealthChanged += SetHealth;
        statsController.OnHungerChanged += SetHunger;
        statsController.OnWaterChanged  += SetWater;
    }

    private void OnDisable()
    {
        statsController.OnHealthChanged -= SetHealth;
        statsController.OnHungerChanged -= SetHunger;
        statsController.OnWaterChanged  -= SetWater;
    }

    private void SetHealth(float v) => healthSlider.value = v;
    private void SetHunger(float v) => hungerSlider.value = v;
    private void SetWater(float v)  => waterSlider.value = v;
}
