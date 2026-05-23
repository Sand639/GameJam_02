using UnityEngine;

public class EventBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // ぶつかってきたのがPlayerだったら
        if (other.CompareTag("Player"))
        {
            // シーン内にある FuelMiniGame スクリプトを探して起動する
            PressMiniGame miniGame = FindFirstObjectByType<PressMiniGame>();

            if (miniGame != null)
            {
                miniGame.StartMiniGame();

                // イベントボックス自身は役割を終えたので消す
                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("FuelMiniGame スクリプトがシーンに見つかりません！");
            }
        }
    }
}