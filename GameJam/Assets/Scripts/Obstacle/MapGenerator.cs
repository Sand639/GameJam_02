using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("マップ（障害物）が手前に迫ってくる速度")]
    public float scrollSpeed = 10f;

    [Header("プレイヤーの後ろの削除基準座標（Z軸）")]
    public float deleteZ = -15f;

    [Header("マップパーツのプレハブリスト（ランダム生成用）")]
    public List<GameObject> mapPrefabs;

    [Header("画面内に常に維持するマップの枚数")]
    public int initialSpawnCount = 5;

    [Header("【追加】最初に順番に配置したいプレハブ")]
    [Tooltip("ここに登録したプレハブが最初から順番に生成されます。initialSpawnCountより少ない場合、残りはランダム生成になります。")]
    public List<GameObject> startingPrefabs = new List<GameObject>();

    [Header("--- イベントボックスの設定 ---")]
    [Tooltip("プレハブの中にあるイベントボックスの『オブジェクト名』を正確に入力してください")]
    public string eventBoxName = "EventBox";

    [Tooltip("何回目でイベントボックスを出現させるか（例：3なら3回に1回出現）")]
    public int showEventBoxCount = 3;

    // 生成済みのマップを管理するリスト
    private List<GameObject> activeMaps = new List<GameObject>();

    // 現在までにイベントボックス付きマップが何回生成されたかのカウンター
    private int currentEventBoxCount = 0;
    // 「1巡」の中身を管理するバッグ（袋）
    private List<int> randomBag = new List<int>();

    void Start()
    {
        // 最初に画面内に配置するマップを生成
        for (int i = 0; i < initialSpawnCount; i++)
        {
            // startingPrefabsに指定があれば、それを順番に生成する
            if (i < startingPrefabs.Count && startingPrefabs[i] != null)
            {
                SpawnMap(startingPrefabs[i]);
            }
            else
            {
                // 指定枠がない、または使い切った場合は通常通りランダム生成
                SpawnMap(mapPrefabs[GetNextBagIndex()]);
            }
        }
    }

    void Update()
    {
        if (mapPrefabs.Count == 0) return;

        // マップ全体を手前に移動させる
        float moveDistance = scrollSpeed * Time.deltaTime;
        foreach (GameObject map in activeMaps)
        {
            if (map != null)
            {
                map.transform.position += new Vector3(0f, 0f, -moveDistance);
            }
        }

        // 一番手前のマップが削除ラインを超えたかチェック
        if (activeMaps.Count > 0)
        {
            GameObject oldestMap = activeMaps[0];
            BoxCollider boxCol = oldestMap.GetComponent<BoxCollider>();

            if (boxCol != null)
            {
                float scaleZ = oldestMap.transform.localScale.z;
                float localEndZ = (boxCol.center.z + (boxCol.size.z / 2f)) * scaleZ;
                float worldEndZ = oldestMap.transform.position.z + localEndZ;

                // 削除ラインを越えたら
                if (worldEndZ < deleteZ)
                {
                    RemoveOldestMap();

                    // 【変更】追加される新しいマップは常にランダム
                    SpawnMap(mapPrefabs[GetNextBagIndex()]);
                }
            }
        }
    }

    /// <summary>
    /// リストの最後尾のマップにピッタリくっつけてマップを生成する
    /// 【変更】引数をインデックスからGameObject(プレハブ本体)にしました
    /// </summary>
    void SpawnMap(GameObject prefab)
    {
        BoxCollider boxCol = prefab.GetComponent<BoxCollider>();
        if (boxCol == null)
        {
            Debug.LogError($"マッププレハブ '{prefab.name}' にBoxColliderが見つかりません。");
            return;
        }

        // 新しく生成するマップの「入り口」のローカル座標を計算
        float scaleZ = prefab.transform.localScale.z;
        float localGridStartZ = (boxCol.center.z - (boxCol.size.z / 2f)) * scaleZ;

        // 目標とするZ座標（1つ前のマップの出口）を計算
        float targetZ = 0f;
        if (activeMaps.Count > 0)
        {
            // すでに生成されているマップがあれば、その「一番最後」のマップを取得
            GameObject lastMap = activeMaps[activeMaps.Count - 1];
            BoxCollider lastBox = lastMap.GetComponent<BoxCollider>();

            // 一番最後のマップの「出口（お尻）」のワールド座標を計算
            float lastScaleZ = lastMap.transform.localScale.z;
            float lastLocalEndZ = (lastBox.center.z + (lastBox.size.z / 2f)) * lastScaleZ;
            targetZ = lastMap.transform.position.z + lastLocalEndZ;
        }

        // 1つ前のマップのお尻（targetZ）に、新しいマップの入り口（localGridStartZ）を合わせる
        float spawnWorldZ = targetZ - localGridStartZ;

        Vector3 spawnPosition = new Vector3(prefab.transform.position.x, prefab.transform.position.y, spawnWorldZ);
        GameObject spawnedMap = Instantiate(prefab, spawnPosition, Quaternion.identity);
        activeMaps.Add(spawnedMap);

        // イベントボックスの出現制御
        foreach (Transform child in spawnedMap.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == eventBoxName)
            {
                currentEventBoxCount++;

                if (currentEventBoxCount >= showEventBoxCount)
                {
                    child.gameObject.SetActive(true);
                    currentEventBoxCount = 0;
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
                break;
            }
        }
    }

    /// <summary>
    /// 最も古いマップを削除する
    /// </summary>
    void RemoveOldestMap()
    {
        GameObject oldestMap = activeMaps[0];
        activeMaps.RemoveAt(0);
        Destroy(oldestMap);
    }

    /// <summary>
    /// テトリスの「バッグシステム（N種1巡）」方式で次のインデックスを取得する
    /// </summary>
    private int GetNextBagIndex()
    {
        // バッグが空になったら、全種類のインデックスを入れ直してシャッフルする（新しい袋を作る）
        if (randomBag.Count == 0)
        {
            RefillAndShuffleBag();
        }

        // バッグの先頭から1つ取り出す
        int nextIndex = randomBag[0];

        // 取り出したものは袋から消す
        randomBag.RemoveAt(0);

        return nextIndex;
    }

    /// <summary>
    /// バッグにプレハブの種類を全部入れてシャッフルする
    /// </summary>
    private void RefillAndShuffleBag()
    {
        // 0 から (プレハブの種類数 - 1) までの数字を順番にバッグに入れる
        for (int i = 0; i < mapPrefabs.Count; i++)
        {
            randomBag.Add(i);
        }

        // 入れた数字をシャッフルする（Fisher-Yatesアルゴリズム）
        for (int i = randomBag.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = randomBag[i];
            randomBag[i] = randomBag[j];
            randomBag[j] = temp;
        }
    }

}