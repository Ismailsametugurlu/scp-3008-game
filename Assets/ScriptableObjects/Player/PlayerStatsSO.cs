using UnityEngine;

// Oyuncu stat değerlerini Inspector'dan ayarlanabilir hale getirir
[CreateAssetMenu(fileName = "PlayerStats", menuName = "SCP3008/Player Stats")]
public class PlayerStatsSO : ScriptableObject
{
    [Header("Maksimum Değerler")]
    public float maxHealth  = 100f;
    public float maxHunger  = 100f;
    public float maxEnergy  = 100f;

    [Header("Azalma Hızları (saniyede)")]
    public float hungerDecayRate = 1f;   // normal hızda açlık
    public float energyDecayRate = 0.5f; // enerji biraz daha yavaş azalır

    [Header("Açlıktan Can Kaybı")]
    public float healthDecayWhenStarving = 2f; // hunger 0 olunca saniyede bu kadar can gider

    [Header("Düşük Enerji Hız Çarpanı")]
    [Range(0.1f, 1f)]
    public float lowEnergySpeedMultiplier = 0.5f; // enerji bitince hareket bu kadar yavaşlar
}
