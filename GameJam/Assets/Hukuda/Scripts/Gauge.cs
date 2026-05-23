using UnityEngine;
using UnityEngine.UI;

public class Gauge : MonoBehaviour
{
    public Image fillImage;

    private float maxValue = 100f;
    private float currentValue = 100f;

    void Update()
    {
        UpdateGauge();
    }

    public void FuelDamege(float amount)
    {
        currentValue -= amount;
        currentValue = Mathf.Clamp(currentValue, 0, maxValue);
    }

    void UpdateGauge()
    {
        float target = currentValue / maxValue;

        fillImage.fillAmount = Mathf.Lerp(
            fillImage.fillAmount,
            target,
            Time.deltaTime * 5f
        );
    }
}