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

    [Header("Uyku / Enerji — TASLAK, sonra ayarlanacak")]
    public float maxSleepEnergy       = 100f;
    public float sleepEnergyDecayRate = 0.4f; // uyanık kaldıkça saniyede azalır (uyku/enerji verici öğeyle doldurulur)

    [Header("Zeka (Kitap Okuma) — TASLAK")]
    public int   maxIntelligenceLevel = 5;
    public float bookReadDuration     = 2f;      // kitap okuma süresi (yemek yeme süresiyle eşlenecek)
    public float intelligencePerBook  = 1f / 3f; // her kitap, mevcut seviyenin bu kadarını doldurur

    [Header("Kas (Fiziksel Aktivite) — TASLAK")]
    public int   maxMuscleLevel                    = 5;
    public float muscleGainPerSprintSecond         = 0.03f; // koşarken saniyede kas ilerlemesi
    public float muscleGainPerHit                  = 0.05f; // düşmana vurunca kas ilerlemesi (ileride dövüş sistemi bağlar)
    public float staminaCostReductionPerMuscleLevel = 0.1f; // her kas seviyesinde stamina maliyeti bu kadar azalır
    public float damageBonusPerMuscleLevel          = 0.15f; // her kas seviyesinde vuruş hasarı bu kadar artar (ileride dövüş sistemi kullanır)
}
