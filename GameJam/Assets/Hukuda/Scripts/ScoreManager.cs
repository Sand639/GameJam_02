using TMPro;
using UnityEngine;



public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    public GameObject scorePopupPrefab;
    public Transform canvas;
    public float popupX = 150f;
    public float popupY = 0f;
    private int score = 0;

    public static ScoreManager instance;


    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        UpdateScore();

       
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScore();

        GameObject obj = Instantiate(scorePopupPrefab, canvas);

        // スコアテキストの位置に合わせる
        obj.transform.position = scoreText.transform.position;

        // ずらす
        obj.transform.position += new Vector3(popupX, popupY, 0f);

        TextMeshProUGUI text = obj.GetComponentInChildren<TextMeshProUGUI>();
        text.text = "+" + amount;

        Destroy(obj, 1f);
    }
    void UpdateScore()
    {
        scoreText.text = "Score : " + score.ToString("0000");
    }
}