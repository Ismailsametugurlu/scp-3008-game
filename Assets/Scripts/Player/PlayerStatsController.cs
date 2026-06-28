using System;
using UnityEngine;

// Sağlık, açlık ve enerji değerlerini yönetir; değişince event fırlatır
public class PlayerStatsController : MonoBehaviour
{
    [SerializeField] private PlayerStatsSO stats;

    // UI ve diğer sistemler bu event'leri dinler (0-1 arası normalize değer gönderir)
    public event Action<float> OnHealthChanged;
    public event Action<float> OnHungerChanged;
    public event Action<float> OnEnergyChanged;
    public event Action OnPlayerDied;

    public float CurrentHealth  { get; private set; }
    public float CurrentHunger  { get; private set; }
    public float CurrentEnergy  { get; private set; }

    // Enerji durumuna göre PlayerController hız çarpanı alır
    public float SpeedMultiplier => CurrentEnergy > 0f ? 1f : stats.lowEnergySpeedMultiplier;

    private bool isDead;

    private void Start()
    {
        CurrentHealth = stats.maxHealth;
        CurrentHunger = stats.maxHunger;
        CurrentEnergy = stats.maxEnergy;

        // Başlangıç UI güncellemesi
        OnHealthChanged?.Invoke(CurrentHealth / stats.maxHealth);
        OnHungerChanged?.Invoke(CurrentHunger / stats.maxHunger);
        OnEnergyChanged?.Invoke(CurrentEnergy / stats.maxEnergy);
    }

    private void Update()
    {
        if (isDead) return;
        DecayStats();
    }

    // Her frame'de açlık ve enerjiyi düşür; açlık sıfırsa can al
    private void DecayStats()
    {
        ChangeHunger(-stats.hungerDecayRate * Time.deltaTime);
        ChangeEnergy(-stats.energyDecayRate * Time.deltaTime);

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

    public void ChangeEnergy(float amount)
    {
        CurrentEnergy = Mathf.Clamp(CurrentEnergy + amount, 0f, stats.maxEnergy);
        OnEnergyChanged?.Invoke(CurrentEnergy / stats.maxEnergy);
    }

    // Yemek yenince çağrılır (ilerleyen oturumda yemek sistemi bağlar)
    public void Eat(float hungerAmount, float healthAmount = 0f)
    {
        ChangeHunger(hungerAmount);
        if (healthAmount > 0f) ChangeHealth(healthAmount);
    }
}
