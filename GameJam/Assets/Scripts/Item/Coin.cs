using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("このコインを取った時のスコア")]
    public int scoreValue = 10;

    [SerializeField] private GameObject _rotateObj;
    [SerializeField] private float _rotateSpeed;
    float _curRotateSpeed = 0.0f;

    private void Update()
    {
        _curRotateSpeed += _rotateSpeed * Time.deltaTime;
        if (_rotateObj != null)
        {
            _rotateObj.transform.localEulerAngles = new Vector3(0.0f, _curRotateSpeed, 0.0f);
        }
    }

    // 何かがこのオブジェクトの当たり判定に入った瞬間に呼ばれる関数
    private void OnTriggerEnter(Collider other)
    {
        // ぶつかってきた相手が「Player」タグを持っているか確認
        if (other.CompareTag("Player"))
        {
            // スコアを加算する関数を呼ぶ（中身はスコア担当者が実装）
            AddScore(scoreValue);

            // コイン自身を画面から削除する
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 【スコア担当者・プレイヤー担当者へ】
    /// ここに実際のスコア加算処理を追記してください。
    /// 例: ScoreManager.Instance.Add(amount); など
    /// </summary>
    private void AddScore(int amount)
    {
        // 開発中の確認用ログ（実装後に消してOKです）
        Debug.Log($"コイン取得！スコアを {amount} 加算する処理をここに書く");
        ScoreManager.instance.AddScore(10);
    }
}