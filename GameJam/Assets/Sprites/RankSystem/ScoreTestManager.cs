using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreTestManager : MonoBehaviour
{
	[Header("UI References")]
	[SerializeField] private TextMeshProUGUI currentScoreText;
	[SerializeField] private Button addScoreButton;
	[SerializeField] private Button finishButton;


	private int currentScore = 0;

	private void Awake()
	{
		if (addScoreButton != null) addScoreButton.onClick.AddListener(AddScore);
		if (finishButton != null) finishButton.onClick.AddListener(FinishGame);
	}

	private void Start()
	{
		UpdateScoreUI();
	}

	private void AddScore()
	{
		currentScore += 10;
		UpdateScoreUI();
	}

	private void UpdateScoreUI()
	{
		if (currentScoreText != null)
		{
			currentScoreText.text = $"Score: {currentScore}";
		}
	}

	private void FinishGame()
	{
		if (addScoreButton != null) addScoreButton.interactable = false;
		if (finishButton != null) finishButton.interactable = false;

		if (GameManager.Instance != null)
		{
			GameManager.Instance.SaveAndGoToResult(currentScore);
		}
		else
		{
			Debug.LogError("unfind GameManager ");
		}
	}
}