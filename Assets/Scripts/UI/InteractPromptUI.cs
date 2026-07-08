using UnityEngine;
using UnityEngine.UI;

// Sağ tık basılı tutarken beliren dairesel "ye/iç" ilerleme göstergesi
public class InteractPromptUI : MonoBehaviour
{
    [SerializeField] private PlayerInteractor interactor;
    [SerializeField] private Image fillImage;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Text label;

    private void Update()
    {
        canvasGroup.alpha = interactor.IsEating ? 1f : 0f;
        if (!interactor.IsEating) return;

        fillImage.fillAmount = interactor.EatProgress01;
        label.text = interactor.CurrentEatType == ConsumableType.Water ? "IC" : "YE";
    }
}
