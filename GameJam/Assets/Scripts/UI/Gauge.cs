using UnityEngine;
using UnityEngine.UI;

public class Gauge : MonoBehaviour
{
    [Header("ゲージ画像")]
    public Image fillImage;

    private float maxValue = 100f;
 

    void Update()
    {
        UpdateGauge();
    }

    //public void FuelDamege(float amount)
    //{
    //    currentValue -= amount;
    //    currentValue = Mathf.Clamp(currentValue, 0, maxValue);
    //}
    //ゲージの更新
    void UpdateGauge()
    {
        float current = Player.GetEnergy();
        float target = current / maxValue;

        fillImage.fillAmount = Mathf.Lerp(
           fillImage.fillAmount,
           target,
           Time.deltaTime * 5f
       );
    }
}