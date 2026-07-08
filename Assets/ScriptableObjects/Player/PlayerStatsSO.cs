using UnityEngine;

// Oyuncu stat değerlerini Inspector'dan ayarlanabilir hale getirir
[CreateAssetMenu(fileName = "PlayerStats", menuName = "SCP3008/Player Stats")]
public class PlayerStatsSO : ScriptableObject
{
    [Header("Maksimum Değerler")]
    public float maxHealth = 100f;
    public float maxHunger = 100f;
    public float maxWater  = 100f;

    [Header("Azalma Hızları (saniyede) — TASLAK, sonra ayarlanacak")]
    public float hungerDecayRate     = 1f;   // normal hızda açlık
    public float waterDecayRate      = 1.3f; // su, açlıktan biraz daha hızlı azalır (pasif)
    public float sprintWaterDrainRate = 2f;  // koşarken suya ek olarak eklenen azalma

    [Header("Stamina (Koşma) — TASLAK, sonra ayarlanacak")]
    public float maxStamina        = 100f;
    public float staminaDrainRate  = 20f; // koşarken saniyede azalma
    public float staminaRegenRate  = 15f; // bekleme bitince saniyede dolma
    public float staminaRegenDelay = 1.5f; // koşma bırakılınca dolmaya başlamadan önceki bekleme (korku gerilimi için)

    [Range(0f, 1f)]
    public float staminaSprintThreshold = 0.35f; // bu oranın altına düşünce koşma kilitlenir, tekrar bu orana çıkana kadar açılmaz

    [Header("Açlıktan Can Kaybı")]
    public float healthDecayWhenStarving = 2f; // hunger 0 olunca saniyede bu kadar can gider

    [Header("Düşük Su Hız Çarpanı")]
    [Range(0.1f, 1f)]
    public float lowWaterSpeedMultiplier = 0.5f; // su bitince hareket bu kadar yavaşlar
}
