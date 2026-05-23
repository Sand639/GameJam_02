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
    [Header("パラメータ")]
    [SerializeField]
    float _gageAddAmount = 0.35f;
    float _successRate = 0.6f;
   //終了猶予時間
   float _finishDelay = 0.5f;

    [Header("サウンド設定")]
    public AudioSource audioSource; // 音を再生するスピーカー役
    public AudioClip successSound;  // 成功時の音データ
    public AudioClip failureSound;  // 失敗時の音データ

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

        bool isLeftPressed = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
        bool isRightPressed = Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);

        // --- 左側（A または 左矢印）が押された時 ---
        // 交互打ちの判定のために、左側が押された記録として代表で KeyCode.A を使う
        if (isLeftPressed && lastPressedKey != KeyCode.A)
        {
            currentScore++;
            lastPressedKey = KeyCode.A;
            if (keyA_Image != null) keyA_Image.sprite = spriteA_Down;
            if (keyD_Image != null) keyD_Image.sprite = spriteD_Up;
            UpdateUI();
        }
        // --- 右側（D または 右矢印）が押された時 ---
        // 交互打ちの判定のために、右側が押された記録として代表で KeyCode.D を使う
        else if (isRightPressed && lastPressedKey != KeyCode.D)
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

        float recoveryPercentage = Mathf.Clamp01((float)currentScore / maxScore * _gageAddAmount) * 100;

        Debug.Log($"ミニゲーム終了！ 連打回数: {currentScore} 回。 燃料を {recoveryPercentage}% 回復！");

        if(currentScore > maxScore * _successRate)
        {
            // 成功音をならす
            if (audioSource != null && successSound != null)
            {
                // PlayOneShotを使うと、音が重なっても途切れずに再生されます
                audioSource.PlayOneShot(successSound);
            }
        }
        else
        {
            // 失敗音をならす
            if (audioSource != null && failureSound != null)
            {
                audioSource.PlayOneShot(failureSound);
            }
        }

        // すぐに閉じず、終了コルーチンを呼ぶ
        StartCoroutine(FinishRoutine(recoveryPercentage));

    }

    private void UpdateUI()
    {
        if (gaugeSlider != null)
        {
            gaugeSlider.value = (float)currentScore / maxScore;
        }
    }

    private IEnumerator FinishRoutine(float recoveryPercentage)
    {
        // プレイヤーが連打をやめるための猶予時間 ＋ 結果を見せる時間
        yield return new WaitForSecondsRealtime(_finishDelay);

        // 溜まっていたキーボード等の入力を完全にクリア！
        Input.ResetInputAxes();

        // 入力をクリアしてから、UIを消して時間を動かす
        if (miniGamePanel != null) miniGamePanel.SetActive(false);
        Time.timeScale = 1f;

        // 完全に終わってから燃料を追加
        Player.RequestAddEnergy(recoveryPercentage);
    }

}