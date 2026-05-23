using TMPro;
using UnityEngine;



public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    public GameObject scorePopupPrefab;
    public Transform canvas;
    [Header("スコアポップアップの位置調整((スコアテキスト基準)")]
    public float popupX = 150f;
    public float popupY = 0f;
    private int score = 0;
    [Header("毎秒スコア")]
    public int scorePerSecond = 10;
    [Header("スコアが加算される時間")]
    public float scoreAddSecond = 1.0f;
    // 毎秒スコア加算のタイマー
    private float timer = 0f;

    public static ScoreManager instance;

    // シングルトンの初期化
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        UpdateScore();

       
    }

    void Update()
    {
        // タイマーを更新
        timer += Time.deltaTime;
        // 一定時間ごとにスコアを加算
        if (timer >= scoreAddSecond && Player.GetEnergy() > 0)
        {
            AddScore(scorePerSecond, false); // ポップアップなし
            timer = 0f;
        }
    }
    //スコアが追加される関数showPopupはスコアポップアップを表示するかどうかのフラグ
    public void AddScore(int amount, bool showPopup = true)
    {
        score += amount;
        UpdateScore();

        if (!showPopup) return;

        GameObject obj = Instantiate(scorePopupPrefab, canvas);

        obj.transform.position = scoreText.transform.position;
        obj.transform.position += new Vector3(popupX, popupY, 0f);

        TextMeshProUGUI text = obj.GetComponentInChildren<TextMeshProUGUI>();
        text.text = "+" + amount;

        Destroy(obj, 1f);
    }

    // スコアテキストを更新する関数
    void UpdateScore()
    {
        scoreText.text = "Score : " + score.ToString("0000");
    }
}