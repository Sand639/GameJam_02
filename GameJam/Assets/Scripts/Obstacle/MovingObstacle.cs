using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    [Header("移動範囲と速度")]
    public float minX = -5f; // 左の限界座標
    public float maxX = 5f;  // 右の限界座標
    public float speed = 3f; // 移動スピード (Aの早さ)

    [Header("ブロックのサイズ設定")]
    [Tooltip("チェックを入れると、指定したmin/maxの枠からブロックがはみ出さないように自動計算します")]
    public bool considerBlockSize = true;

    // 実際に移動できる限界の座標（計算後）
    private float actualMinX;
    private float actualMaxX;

    // 移動方向 (1 = 右, -1 = 左)
    private int direction = 1;

    void Start()
    {
        // 初期値として設定した値を代入
        actualMinX = minX;
        actualMaxX = maxX;

        // ブロックのサイズを加味する処理
        if (considerBlockSize)
        {
            // オブジェクトに付いているCollider(当たり判定)を取得
            Collider col = GetComponent<Collider>();

            if (col != null)
            {
                // bounds.extents.x は「オブジェクトの中心から端までの距離（横幅の半分）」
                float halfWidth = col.bounds.extents.x;

                // 限界座標を内側に狭めることで、ブロックがはみ出さないようにする
                actualMinX = minX + halfWidth;
                actualMaxX = maxX - halfWidth;
            }
            else
            {
                Debug.LogWarning("Colliderが見つからないため、サイズの自動計算ができません。");
            }
        }
    }

    void Update()
    {
        // 現在の位置を取得
        Vector3 pos = transform.position;

        // X座標を移動させる (速さ × 時間経過 × 方向)
        pos.x += speed * Time.deltaTime * direction;

        // 右の限界に達したら左へ折り返す
        if (pos.x >= actualMaxX)
        {
            pos.x = actualMaxX;
            direction = -1;
        }
        // 左の限界に達したら右へ折り返す
        else if (pos.x <= actualMinX)
        {
            pos.x = actualMinX;
            direction = 1;
        }

        // 変更した位置をオブジェクトに反映
        transform.position = pos;
    }
}