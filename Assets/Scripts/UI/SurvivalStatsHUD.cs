using UnityEngine;

// PlayerStatsController event'lerini 3 dairesel göstergeye (can/açlık/su) bağlar
public class SurvivalStatsHUD : MonoBehaviour
{
    [SerializeField] private PlayerStatsController statsController;
    [SerializeField] private SurvivalGaugeUI healthGauge;
    [SerializeField] private SurvivalGaugeUI hungerGauge;
    [SerializeField] private SurvivalGaugeUI waterGauge;

    private void OnEnable()
    {
        statsController.OnHealthChanged += healthGauge.SetValue;
        statsController.OnHungerChanged += hungerGauge.SetValue;
        statsController.OnWaterChanged  += waterGauge.SetValue;
    }

    private void OnDisable()
    {
        statsController.OnHealthChanged -= healthGauge.SetValue;
        statsController.OnHungerChanged -= hungerGauge.SetValue;
        statsController.OnWaterChanged  -= waterGauge.SetValue;
    }
}
