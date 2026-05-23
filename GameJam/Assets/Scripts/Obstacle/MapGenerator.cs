using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("プレイヤーのTransform")]
    public Transform playerTransform;

    [Header("マップパーツのプレハブリスト")]
    public List<GameObject> mapPrefabs;

    [Header("最初に生成しておく枚数")]
    public int initialSpawnCount = 5;

    [Header("プレイヤーの何メートル先まで先読みして生成するか")]
    public float forwardSpawnDistance = 50f;

    // 次にマップを配置すべきZ座標
    private float nextSpawnZ = 0f;

    // 生成済みのマップを管理するリスト（古いものを消す用）
    private List<GameObject> activeMaps = new List<GameObject>();

    void Start()
    {
        if (playerTransform == null)
        {
            // インスペクターで未設定の場合、Playerタグから自動取得を試みる
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }

        // 最初にいくつかマップを生成しておく
        for (int i = 0; i < initialSpawnCount; i++)
        {
            // 最初の一枚目は固定にするか、ランダムにするか選べます
            SpawnMap(Random.Range(0, mapPrefabs.Count));
        }
    }

    void Update()
    {
        if (playerTransform == null || mapPrefabs.Count == 0) return;

        // プレイヤーの現在位置から「先読み距離」の間にマップが足りなくなったら自動生成
        if (playerTransform.position.z + forwardSpawnDistance > nextSpawnZ)
        {
            SpawnMap(Random.Range(0, mapPrefabs.Count));

            // 【オマケ機能】プレイヤーの後ろに回りすぎた古いマップを削除して軽くする
            if (activeMaps.Count > initialSpawnCount + 2)
            {
                RemoveOldestMap();
            }
        }
    }

    /// <summary>
    /// 指定されたインデックスのマッププレハブをサイズ考慮して生成する
    /// </summary>
    void SpawnMap(int prefabIndex)
    {
        GameObject prefab = mapPrefabs[prefabIndex];

        // 【変更点】Colliderではなく「BoxCollider」として取得する
        BoxCollider boxCol = prefab.GetComponent<BoxCollider>();
        if (boxCol == null)
        {
            Debug.LogError($"マッププレハブ '{prefab.name}' にBoxColliderが見つかりません。");
            return;
        }

        // 【変更点】boundsではなく、BoxColliderのSizeの値を直接読み取る
        // ※プレハブ自体のScaleが変更されていても対応できるように掛け算します
        float mapLength = boxCol.size.z * prefab.transform.localScale.z;

        // 次の配置位置（nextSpawnZ）に「マップの長さの半分」を足した場所が中心座標
        float spawnCenterZ = nextSpawnZ + (mapLength / 2f);

        // 生成位置を決定
        Vector3 spawnPosition = new Vector3(0f, 0f, spawnCenterZ);

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