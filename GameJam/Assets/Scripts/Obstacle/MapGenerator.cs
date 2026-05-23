using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("マップ（障害物）が手前に迫ってくる速度")]
    public float scrollSpeed = 10f;

    [Header("プレイヤーの後ろの削除基準座標（Z軸）")]
    public float deleteZ = -15f;

    [Header("マップパーツのプレハブリスト")]
    public List<GameObject> mapPrefabs;

    [Header("画面内に常に維持するマップの枚数")]
    public int initialSpawnCount = 5;

    [Header("--- イベントボックスの設定 ---")]
    [Tooltip("プレハブの中にあるイベントボックスの『オブジェクト名』を正確に入力してください")]
    public string eventBoxName = "EventBox";

    [Tooltip("何回目でイベントボックスを出現させるか（例：3なら3回に1回出現）")]
    public int showEventBoxCount = 3;

    // 生成済みのマップを管理するリスト
    private List<GameObject> activeMaps = new List<GameObject>();

    // 現在までにイベントボックス付きマップが何回生成されたかのカウンター
    private int currentEventBoxCount = 0;

    void Start()
    {
        // 最初に画面内に配置するマップを生成
        for (int i = 0; i < initialSpawnCount; i++)
        {
            SpawnMap(Random.Range(0, mapPrefabs.Count));
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
                    // 新しいマップを一番奥に追加
                    SpawnMap(Random.Range(0, mapPrefabs.Count));
                }
            }
        }
    }

    /// <summary>
    /// リストの最後尾のマップにピッタリくっつけてマップを生成する
    /// </summary>
    void SpawnMap(int prefabIndex)
    {
        GameObject prefab = mapPrefabs[prefabIndex];

        BoxCollider boxCol = prefab.GetComponent<BoxCollider>();
        if (boxCol == null)
        {
            Debug.LogError($"マッププレハブ '{prefab.name}' にBoxColliderが見つかりません。");
            return;
        }

        // 新しく生成するマップの「入り口」のローカル座標を計算
        float scaleZ = prefab.transform.localScale.z;
        float localGridStartZ = (boxCol.center.z - (boxCol.size.z / 2f)) * scaleZ;

        // 【超重要変更点】目標とするZ座標（1つ前のマップの出口）を計算
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
}