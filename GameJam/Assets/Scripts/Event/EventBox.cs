using UnityEngine;

public class EventBox : MonoBehaviour
{
    public void MiniGameStart()
    {    
        // シーン内にある FuelMiniGame スクリプトを探して起動する
        PressMiniGame miniGame = FindFirstObjectByType<PressMiniGame>();

        miniGame.StartMiniGame();

        // イベントボックス自身は役割を終えたので消す
        Destroy(gameObject);

    }

}