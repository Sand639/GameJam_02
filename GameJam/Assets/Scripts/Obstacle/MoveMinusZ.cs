using UnityEngine;

public class MoveMinusZ : MonoBehaviour
{
    [Header("移動させる対象（未指定なら自分自身）")]
    [Tooltip("動かしたいオブジェクトをドラッグ＆ドロップします。空欄の場合はこのスクリプトがついているオブジェクトが動きます。")]
    public Transform target;

    [Header("移動スピード")]
    public float speed = 5f; // 1秒間に進む距離

    void Start()
    {
        // 対象が指定されていない場合は、自分自身（スクリプトが付いているオブジェクト）をセットする
        if (target == null)
        {
            target = this.transform;
        }
    }

    void Update()
    {
        // 現在の位置を取得
        Vector3 pos = target.position;

        // Z座標をマイナス方向（-Z）へ移動させる
        pos.z -= speed * Time.deltaTime;

        // 変更した位置をオブジェクトに反映
        target.position = pos;
    }
}