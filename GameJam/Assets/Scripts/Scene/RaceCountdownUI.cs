using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class RaceCountdownUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text countdownText;   // 中央のTMP
    [SerializeField] private Image maskImage;          // 画面マスク(任意)
    [SerializeField] private Button startButton;       // 開始ボタン(任意)

    [Header("Countdown")]
    [SerializeField] private int startNumber = 5;      // 3にするとマリカ寄り
    [SerializeField] private float stepSeconds = 1.0f; // 数字切替間隔(通常1秒)

    [Header("Animation")]
    [SerializeField] private float popDuration = 0.25f; // “ドン”の時間
    [SerializeField] private float startScale = 0.2f;
    [SerializeField] private float peakScale = 1.25f;
    [SerializeField] private float endScale = 1.0f;

    [Header("Colors")]
    [SerializeField] private Color numberColor = Color.white;
    [SerializeField] private Color goColor = new Color(0.2f, 0.0f, 0.2f);

    private bool running;

    // ボタンのOnClickから呼ぶ
    public void StartCountdown()
    {
        if (running) return;
        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        running = true;

        if (startButton != null) startButton.interactable = false;

        if (maskImage != null) maskImage.gameObject.SetActive(true);
        countdownText.gameObject.SetActive(true);

        // 例: 5,4,3,2,1
        for (int n = startNumber; n >= 1; n--)
        {
            countdownText.color = numberColor;
            countdownText.text = n.ToString();

            yield return PopAnimation();
            yield return new WaitForSeconds(Mathf.Max(0f, stepSeconds - popDuration));
        }

        // GO!
        countdownText.color = goColor;
        countdownText.text = "GO!";
        yield return PopAnimation();
        yield return new WaitForSeconds(0.3f);

        // 表示を消す
        countdownText.gameObject.SetActive(false);
        if (maskImage != null) maskImage.gameObject.SetActive(false);

        // シーン遷移（次へ、最後なら最初へ）
        LoadNextSceneLoop();

        running = false;
    }

    private IEnumerator PopAnimation()
    {
        // “ドン！”：小→大→等倍
        RectTransform rt = countdownText.rectTransform;
        rt.localScale = Vector3.one * startScale;

        float t = 0f;
        // 小→大
        while (t < popDuration * 0.6f)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / (popDuration * 0.6f));
            float s = Mathf.Lerp(startScale, peakScale, EaseOutBack(p));
            rt.localScale = Vector3.one * s;
            yield return null;
        }

        t = 0f;
        // 大→等倍
        while (t < popDuration * 0.4f)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / (popDuration * 0.4f));
            float s = Mathf.Lerp(peakScale, endScale, p);
            rt.localScale = Vector3.one * s;
            yield return null;
        }

        rt.localScale = Vector3.one * endScale;
    }

    // それっぽい“跳ね”イージング（プラグイン不要）
    private float EaseOutBack(float x)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
    }

    private void LoadNextSceneLoop()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        int maxIndex = SceneManager.sceneCountInBuildSettings;
        int nextIndex = (index + 1) % maxIndex;
        SceneManager.LoadScene(nextIndex);
    }
}