using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Sağlık, açlık ve su değerlerini yönetir; değişince event fırlatır
public class PlayerStatsController : MonoBehaviour
{
    [SerializeField] private PlayerStatsSO stats;

    // UI ve diğer sistemler bu event'leri dinler (0-1 arası normalize değer gönderir)
    public event Action<float> OnHealthChanged;
    public event Action<float> OnHungerChanged;
    public event Action<float> OnWaterChanged;
    public event Action<float> OnStaminaChanged;
    public event Action<float> OnSleepEnergyChanged;
    public event Action<int, float> OnIntelligenceChanged; // seviye, mevcut seviye içindeki ilerleme (0-1)
    public event Action<int, float> OnMuscleChanged;       // seviye, mevcut seviye içindeki ilerleme (0-1)
    public event Action OnPlayerDied;

    public float CurrentHealth      { get; private set; }
    public float CurrentHunger      { get; private set; }
    public float CurrentWater       { get; private set; }
    public float CurrentStamina     { get; private set; }
    public float CurrentSleepEnergy { get; private set; }
    public int   CurrentIntelligenceLevel { get; private set; }
    public int   CurrentMuscleLevel       { get; private set; }

    // Su durumuna göre PlayerController hız çarpanı alır
    public float SpeedMultiplier => CurrentWater > 0f ? 1f : stats.lowWaterSpeedMultiplier;

    // Kilitliyken (Shift'i eşiğin altındayken bıraktıysa) koşamaz; eşiğe çıkınca açılır
    public bool CanSprint => !isStaminaLocked && CurrentStamina > 0f;

    // Kas seviyesi arttıkça koşma stamina maliyeti düşer, vuruş hasarı artar (dövüş sistemi ileride kullanır)
    public float MuscleStaminaMultiplier => 1f - (CurrentMuscleLevel * stats.staminaCostReductionPerMuscleLevel);
    public float MuscleDamageMultiplier  => 1f + (CurrentMuscleLevel * stats.damageBonusPerMuscleLevel);

    private bool isDead;
    private bool isStaminaLocked;
    private bool wasSprintKeyHeldLastFrame;
    private float lastSprintTime = -999f;
    private float intelligenceProgress; // 0-1, mevcut zeka seviyesi içindeki ilerleme
    private float muscleProgress;       // 0-1, mevcut kas seviyesi içindeki ilerleme

    private void Start()
    {
        CurrentHealth      = stats.maxHealth;
        CurrentHunger      = stats.maxHunger;
        CurrentWater       = stats.maxWater;
        CurrentStamina     = stats.maxStamina;
        CurrentSleepEnergy = stats.maxSleepEnergy;

        // Başlangıç UI güncellemesi
        OnHealthChanged?.Invoke(CurrentHealth / stats.maxHealth);
        OnHungerChanged?.Invoke(CurrentHunger / stats.maxHunger);
        OnWaterChanged?.Invoke(CurrentWater / stats.maxWater);
        OnStaminaChanged?.Invoke(CurrentStamina / stats.maxStamina);
        OnSleepEnergyChanged?.Invoke(CurrentSleepEnergy / stats.maxSleepEnergy);
        OnIntelligenceChanged?.Invoke(CurrentIntelligenceLevel, intelligenceProgress);
        OnMuscleChanged?.Invoke(CurrentMuscleLevel, muscleProgress);
    }

    private void Update()
    {
        if (isDead) return;
        DecayStats();
    }

    // Her frame'de açlık/su/stamina günceller; koşma stamina tarafından sınırlanır
    private void DecayStats()
    {
        ChangeHunger(-stats.hungerDecayRate * Time.deltaTime);

        bool wantsSprint = Keyboard.current.leftShiftKey.isPressed;
        float staminaFraction = CurrentStamina / stats.maxStamina;

        // Eşiğe ulaşıldıysa kilidi kaldır
        if (isStaminaLocked && staminaFraction >= stats.staminaSprintThreshold)
            isStaminaLocked = false;

        // Stamina tamamen bittiyse VEYA eşiğin altındayken Shift bırakıldıysa kilitle
        // (basılı tutulduğu sürece 0'a kadar düşebilir, kesilmez)
        bool releasedBelowThreshold = wasSprintKeyHeldLastFrame && !wantsSprint && staminaFraction < stats.staminaSprintThreshold;
        if (CurrentStamina <= 0f || releasedBelowThreshold)
            isStaminaLocked = true;

        wasSprintKeyHeldLastFrame = wantsSprint;

        bool isSprinting = wantsSprint && CanSprint;

        if (isSprinting)
        {
            lastSprintTime = Time.time;
            // Kas seviyesi arttıkça koşma stamina'ya daha az mal olur
            ChangeStamina(-stats.staminaDrainRate * MuscleStaminaMultiplier * Time.deltaTime);
            GainMuscle(stats.muscleGainPerSprintSecond * Time.deltaTime);
        }
        else if (Time.time - lastSprintTime >= stats.staminaRegenDelay)
        {
            // Koşma bırakılalı yeterince zaman geçtiyse dolmaya başla (aniden dolmasın)
            ChangeStamina(stats.staminaRegenRate * Time.deltaTime);
        }

        float waterDrain = stats.waterDecayRate + (isSprinting ? stats.sprintWaterDrainRate : 0f);
        ChangeWater(-waterDrain * Time.deltaTime);

        // Uyanık kaldıkça uyku/enerji azalır (uyuyunca veya enerji verici öğeyle doldurulur)
        AddSleepEnergy(-stats.sleepEnergyDecayRate * Time.deltaTime);

        if (CurrentHunger <= 0f)
            ChangeHealth(-stats.healthDecayWhenStarving * Time.deltaTime);
    }

    public void ChangeHealth(float amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0f, stats.maxHealth);
        OnHealthChanged?.Invoke(CurrentHealth / stats.maxHealth);

        if (CurrentHealth <= 0f && !isDead)
        {
            isDead = true;
            OnPlayerDied?.Invoke();
            Debug.Log("[PlayerStats] Oyuncu öldü!");
        }
    }

    public void ChangeHunger(float amount)
    {
        CurrentHunger = Mathf.Clamp(CurrentHunger + amount, 0f, stats.maxHunger);
        OnHungerChanged?.Invoke(CurrentHunger / stats.maxHunger);
    }

    public void ChangeWater(float amount)
    {
        CurrentWater = Mathf.Clamp(CurrentWater + amount, 0f, stats.maxWater);
        OnWaterChanged?.Invoke(CurrentWater / stats.maxWater);
    }

    public void ChangeStamina(float amount)
    {
        CurrentStamina = Mathf.Clamp(CurrentStamina + amount, 0f, stats.maxStamina);
        OnStaminaChanged?.Invoke(CurrentStamina / stats.maxStamina);
    }

    // Yemek yenince çağrılır (ilerleyen oturumda yemek sistemi bağlar)
    public void Eat(float hungerAmount, float healthAmount = 0f)
    {
        ChangeHunger(hungerAmount);
        if (healthAmount > 0f) ChangeHealth(healthAmount);
    }

    // Su içilince çağrılır (ilerleyen oturumda su/içecek sistemi bağlar)
    public void Drink(float waterAmount)
    {
        ChangeWater(waterAmount);
    }

    // Uyuyunca veya enerji verici bir şey yiyip/içince çağrılır (ilerleyen oturumda bağlanacak)
    public void AddSleepEnergy(float amount)
    {
        CurrentSleepEnergy = Mathf.Clamp(CurrentSleepEnergy + amount, 0f, stats.maxSleepEnergy);
        OnSleepEnergyChanged?.Invoke(CurrentSleepEnergy / stats.maxSleepEnergy);
    }

    // Kitap okununca çağrılır (ilerleyen oturumda sağ-tık-basılı-tut etkileşimi bağlanacak)
    public void ReadBook()
    {
        if (CurrentIntelligenceLevel >= stats.maxIntelligenceLevel) return;

        intelligenceProgress += stats.intelligencePerBook;
        if (intelligenceProgress >= 1f)
        {
            intelligenceProgress = 0f;
            CurrentIntelligenceLevel = Mathf.Min(CurrentIntelligenceLevel + 1, stats.maxIntelligenceLevel);
        }

        bool isMax = CurrentIntelligenceLevel >= stats.maxIntelligenceLevel;
        OnIntelligenceChanged?.Invoke(CurrentIntelligenceLevel, isMax ? 1f : intelligenceProgress);
    }

    // Koşarken otomatik çağrılır; düşmana vurunca da dövüş sistemi ileride bunu çağıracak
    public void GainMuscle(float amount)
    {
        if (CurrentMuscleLevel >= stats.maxMuscleLevel) return;

        muscleProgress += amount;
        if (muscleProgress >= 1f)
        {
            muscleProgress = 0f;
            CurrentMuscleLevel = Mathf.Min(CurrentMuscleLevel + 1, stats.maxMuscleLevel);
        }

        bool isMax = CurrentMuscleLevel >= stats.maxMuscleLevel;
        OnMuscleChanged?.Invoke(CurrentMuscleLevel, isMax ? 1f : muscleProgress);
    }
}
