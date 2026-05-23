using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


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

    private int previousCoin = 0;
    public static ScoreManager instance;

    // シングルトンの初期化
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        UpdateScore();

        previousCoin = Player.GetCoin(); // 初期値
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
        CheckCoinIncrease(); // ←これ追加
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

    public int GetScore()
    {
        return score;
	}
    void CheckCoinIncrease()
    {
        int currentCoin = Player.GetCoin();

        int diff = currentCoin - previousCoin;

        if (diff > 0)
        {
            AddScore(diff * 10, true); // 1枚10点
        }

        previousCoin = currentCoin;
    }
    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        scoreText = FindObjectOfType<TextMeshProUGUI>();
    }
}