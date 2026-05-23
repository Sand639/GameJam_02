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

    // 次にマップを配置すべきワールドZ座標
    private float nextSpawnZ = 0f;

    // 生成済みのマップを管理するリスト
    private List<GameObject> activeMaps = new List<GameObject>();

    void Start()
    {
        // 最初に指定枚数のマップを前方にズラッと並べる
        for (int i = 0; i < initialSpawnCount; i++)
        {
            SpawnMap(Random.Range(0, mapPrefabs.Count));
        }
    }

    void Update()
    {
        if (mapPrefabs.Count == 0) return;

        // 1. リストにあるすべてのマップを手前（-Z方向）に移動させる
        float moveDistance = scrollSpeed * Time.deltaTime;
        foreach (GameObject map in activeMaps)
        {
            if (map != null)
            {
                map.transform.position += new Vector3(0f, 0f, -moveDistance);
            }
        }

        // 2. マップが手前に動いた分、次に奥に生成する位置（nextSpawnZ）も手前に引く
        nextSpawnZ -= moveDistance;

        // 3. 一番手前（古い）のマップが、削除基準を完全に通り過ぎたかチェック
        if (activeMaps.Count > 0)
        {
            GameObject oldestMap = activeMaps[0];
            BoxCollider boxCol = oldestMap.GetComponent<BoxCollider>();

            if (boxCol != null)
            {
                float scaleZ = oldestMap.transform.localScale.z;
                // マップの「奥の端（出口）」のローカル座標を計算
                float localEndZ = (boxCol.center.z + (boxCol.size.z / 2f)) * scaleZ;
                // ワールド座標での「奥の端」を計算
                float worldEndZ = oldestMap.transform.position.z + localEndZ;

                // マップの出口が削除位置（プレイヤーの後ろ）より後ろに行き過ぎたら
                if (worldEndZ < deleteZ)
                {
                    // 古いマップを削除
                    RemoveOldestMap();

                    // 【ご要望のポイント】削除したのと同時に、一番奥に新しいマップを1枚生成
                    SpawnMap(Random.Range(0, mapPrefabs.Count));
                }
            }
        }
    }

    /// <summary>
    /// 指定されたインデックスのマッププレハブをサイズ考慮して生成する
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

        float scaleZ = prefab.transform.localScale.z;
        float mapLength = boxCol.size.z * scaleZ;

        // プレハブの基準点から見て「マップの一番手前の端（入り口）」のローカル座標
        float localGridStartZ = (boxCol.center.z - (boxCol.size.z / 2f)) * scaleZ;

        // 前のマップの終わりに、次のマップの入り口をピッタリ合わせるワールド座標
        float spawnWorldZ = nextSpawnZ - localGridStartZ;

        // 生成位置を決定（XとYはプレハブの設定を維持）
        Vector3 spawnPosition = new Vector3(prefab.transform.position.x, prefab.transform.position.y, spawnWorldZ);

        // マップを生成
        GameObject spawnedMap = Instantiate(prefab, spawnPosition, Quaternion.identity);
        activeMaps.Add(spawnedMap);

        // 次のマップのために、配置したマップの長さ分だけZ座標を進める
        nextSpawnZ += mapLength;
    }

    /// <summary>
    /// 一番古い（後ろにある）マップを削除する
    /// </summary>
    void RemoveOldestMap()
    {
        GameObject oldestMap = activeMaps[0];
        activeMaps.RemoveAt(0);
        Destroy(oldestMap);
    }
}