using UnityEngine;
using UnityEngine.UI;

// Seviyeli göstergeler için (Zeka/Kas). Fill = mevcut seviye içindeki ilerleme.
// Maksimum seviyeye ulaşınca renk değişir (Zeka için altın).
public class LeveledGaugeUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color maxLevelColor = Color.white;
    [SerializeField] private int maxLevel = 5;

    public void SetLevel(int level, float progress)
    {
        fillImage.fillAmount = Mathf.Clamp01(progress);
        fillImage.color = level >= maxLevel ? maxLevelColor : normalColor;
    }
}
