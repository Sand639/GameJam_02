using System.Collections; // コルーチンを使うために追加
using UnityEngine;
using UnityEngine.UI;

public class PressMiniGame : MonoBehaviour
{
    [Header("ミニゲームの設定")]
    public float readyTime = 2.0f; // 【追加】開始までの待機時間（X秒）
    public float timeLimit = 5.0f;
    public int maxScore = 30;

    [Header("UIの参照")]
    public GameObject miniGamePanel;
    public Text countdownText; // 【追加】「READY... GO!」を表示するテキスト

    [Header("キー画像の参照")]
    public Image keyA_Image;
    public Image keyD_Image;
    public Sprite spriteA_Up;
    public Sprite spriteA_Down;
    public Sprite spriteD_Up;
    public Sprite spriteD_Down;

    [Header("ゲージの参照")]
    public Slider gaugeSlider;

    private bool isPlaying = false;
    private float currentTimer;
    private int currentScore;
    private KeyCode lastPressedKey = KeyCode.None;

    void Start()
    {
        if (miniGamePanel != null) miniGamePanel.SetActive(false);
    }

    void Update()
    {
        // 待機中（isPlayingがfalse）は入力を一切受け付けない
        if (!isPlaying) return;

        currentTimer -= Time.unscaledDeltaTime;

        if (currentTimer <= 0f)
        {
            EndMiniGame();
            return;
        }

        // --- Aが押された時 ---
        if (Input.GetKeyDown(KeyCode.A) && lastPressedKey != KeyCode.A)
        {
            currentScore++;
            lastPressedKey = KeyCode.A;
            if (keyA_Image != null) keyA_Image.sprite = spriteA_Down;
            if (keyD_Image != null) keyD_Image.sprite = spriteD_Up;
            UpdateUI();
        }
        // --- Dが押された時 ---
        else if (Input.GetKeyDown(KeyCode.D) && lastPressedKey != KeyCode.D)
        {
            currentScore++;
            lastPressedKey = KeyCode.D;
            if (keyA_Image != null) keyA_Image.sprite = spriteA_Up;
            if (keyD_Image != null) keyD_Image.sprite = spriteD_Down;
            UpdateUI();
        }
    }

    public void StartMiniGame()
    {
        Time.timeScale = 0f;
        if (miniGamePanel != null) miniGamePanel.SetActive(true);

        // UIの初期化
        if (keyA_Image != null) keyA_Image.sprite = spriteA_Up;
        if (keyD_Image != null) keyD_Image.sprite = spriteD_Up;
        if (gaugeSlider != null) gaugeSlider.value = 0f;

        // 【追加】すぐには始めず、準備カウントダウンのコルーチンを呼び出す
        StartCoroutine(ReadyRoutine());
    }

    // 【追加】ゲーム開始前の演出と待機を行う処理
    private IEnumerator ReadyRoutine()
    {
        isPlaying = false; // まだ入力は受け付けない

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
            countdownText.text = "READY...";
        }

        // ここで readyTime（例：2秒）だけ待つ（TimeScale=0の影響を受けないメソッド）
        yield return new WaitForSecondsRealtime(readyTime);

        // 待機が終わったらGO！を表示してゲーム開始
        if (countdownText != null)
        {
            countdownText.text = "GO!!";
        }

        currentTimer = timeLimit;
        currentScore = 0;
        lastPressedKey = KeyCode.None;
        isPlaying = true; // ここからUpdateでの入力受付がスタート！

        // GOの文字を1秒間だけ表示して消す
        yield return new WaitForSecondsRealtime(1.0f);
        if (countdownText != null) countdownText.gameObject.SetActive(false);
    }

    private void EndMiniGame()
    {
        isPlaying = false;
        if (miniGamePanel != null) miniGamePanel.SetActive(false);
        Time.timeScale = 1f;

        //  /  5は補正
        float recoveryPercentage = Mathf.Clamp01((float)currentScore / maxScore / 5);
        Debug.Log($"ミニゲーム終了！ 連打回数: {currentScore} 回。 燃料を {recoveryPercentage * 100}% 回復！");

        //ここに燃料を追加するコードを追加してください

        // 例: Player.Instance.AddFuel(recoveryPercentage);

    }

    private void UpdateUI()
    {
        if (gaugeSlider != null)
        {
            gaugeSlider.value = (float)currentScore / maxScore;
        }
    }
}