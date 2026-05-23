using System.Collections.Generic;
using UnityEngine;

public class RankSystem : MonoBehaviour
{
	[Header("System References")]
	[SerializeField] private HighscoreHandler highscoreHandler;

	[Header("UI Panel Reference")]
	[SerializeField] private ScorePanelScript scorePanel;

	[Header("Settings")]
	[SerializeField] private string jsonFilename = "HighScore.json";
	[SerializeField] private int maxCount = 7;

	private List<HighscoreElement> currentListData = new List<HighscoreElement>();

	private void OnEnable()
	{
		HighscoreHandler.onHighscoreListChanged += OnHighscoreDataChanged;
	}

	private void OnDisable()
	{
		HighscoreHandler.onHighscoreListChanged -= OnHighscoreDataChanged;
	}

	private void Start()
	{
		currentListData = FileHandler.ReadListFromJSON<HighscoreElement>(jsonFilename);

		if (GameManager.Instance != null)
		{
			int finalScore = GameManager.Instance.CurrentGameScore;

			if (finalScore > 0 && IsNewHighscore(finalScore))
			{

				if (scorePanel != null)
				{
					scorePanel.gameObject.SetActive(true);

					scorePanel.ShowNameInput((inputtedName) =>
					{
						if (highscoreHandler != null)
						{
							HighscoreElement element = new HighscoreElement(inputtedName, finalScore);
							highscoreHandler.AddHighscoreIfPossible(element);
						}
					});

					scorePanel.RefreshDisplay(currentListData);
				}
				return;
			}
		}

		TriggerDefaultDisplay();
	}

	private void TriggerDefaultDisplay()
	{
		if (scorePanel != null)
		{
			scorePanel.gameObject.SetActive(true);
			scorePanel.RefreshDisplay(currentListData);
		}
	}


	private bool IsNewHighscore(int score)
	{
		if (currentListData == null || currentListData.Count < maxCount)
		{
			return true;
		}

		return score > currentListData[currentListData.Count - 1].points;
	}

	private void OnHighscoreDataChanged(List<HighscoreElement> newList)
	{
		currentListData = newList;

		if (scorePanel != null && scorePanel.gameObject.activeSelf)
		{
			scorePanel.RefreshDisplay(currentListData);
		}
	}
}