using UnityEngine;

// PlayerStatsController event'lerini büyük Can dairesine ve 5 küçük rozete (Su/Açlık/Uyku/Kas/Zeka) bağlar
public class SurvivalStatsHUD : MonoBehaviour
{
    [SerializeField] private PlayerStatsController statsController;
    [SerializeField] private SurvivalGaugeUI healthGauge;
    [SerializeField] private SurvivalGaugeUI waterGauge;
    [SerializeField] private SurvivalGaugeUI hungerGauge;
    [SerializeField] private SurvivalGaugeUI sleepGauge;
    [SerializeField] private LeveledGaugeUI muscleGauge;
    [SerializeField] private LeveledGaugeUI intelligenceGauge;

    private void OnEnable()
    {
        statsController.OnHealthChanged      += healthGauge.SetValue;
        statsController.OnWaterChanged       += waterGauge.SetValue;
        statsController.OnHungerChanged      += hungerGauge.SetValue;
        statsController.OnSleepEnergyChanged += sleepGauge.SetValue;
        statsController.OnMuscleChanged      += muscleGauge.SetLevel;
        statsController.OnIntelligenceChanged += intelligenceGauge.SetLevel;
    }

    private void OnDisable()
    {
        statsController.OnHealthChanged      -= healthGauge.SetValue;
        statsController.OnWaterChanged       -= waterGauge.SetValue;
        statsController.OnHungerChanged      -= hungerGauge.SetValue;
        statsController.OnSleepEnergyChanged -= sleepGauge.SetValue;
        statsController.OnMuscleChanged      -= muscleGauge.SetLevel;
        statsController.OnIntelligenceChanged -= intelligenceGauge.SetLevel;
    }
}
