using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ScorePanelScript : MonoBehaviour
{
	[Header("Buttons")]
	[SerializeField] private Button BackTitleButton;
	[SerializeField] private Button endGameButton;

	[Header("Highscore Display Layout")]
	[SerializeField] private GameObject rankItemPrefab;
	[SerializeField] Transform itemContainer;

	[Header("Retro Name Input UI (New)")]
	[SerializeField] private GameObject nameInputArea;
	[SerializeField] private TMP_InputField nameInputField;
	[SerializeField] private Button confirmNameButton;

	private System.Collections.Generic.List<GameObject> spawnedItems = new System.Collections.Generic.List<GameObject>();

	private System.Action<string> onNameConfirmedCallback;

	private void Awake()
	{
		if (BackTitleButton != null) BackTitleButton.onClick.AddListener(RetryGame);
		if (endGameButton != null) endGameButton.onClick.AddListener(EndGameFunc);
		if (confirmNameButton != null) confirmNameButton.onClick.AddListener(OnConfirmNameClicked);
		if (nameInputArea != null) nameInputArea.SetActive(false);

		if (nameInputField != null)
		{
			nameInputField.characterLimit = 10;
			nameInputField.onValueChanged.AddListener((text) => {
				nameInputField.text = text.ToUpper();
			});
		}
	}

	public void ShowNameInput(System.Action<string> onConfirmed)
	{
		onNameConfirmedCallback = onConfirmed;

		if (nameInputArea != null) nameInputArea.SetActive(true);

		if (nameInputField != null)
		{
			nameInputField.text = "PLAYER";
			nameInputField.ActivateInputField();
		}

		if (BackTitleButton != null) BackTitleButton.gameObject.SetActive(false);
		if (endGameButton != null) endGameButton.gameObject.SetActive(false);
	}

	private void OnConfirmNameClicked()
	{
		string finalName = "PLAYER";
		if (nameInputField != null && !string.IsNullOrEmpty(nameInputField.text))
		{
			finalName = nameInputField.text.ToUpper();
		}

		if (nameInputArea != null) nameInputArea.SetActive(false);

		if (BackTitleButton != null) BackTitleButton.gameObject.SetActive(true);
		if (endGameButton != null) endGameButton.gameObject.SetActive(true);

		onNameConfirmedCallback?.Invoke(finalName);
	}

	public void RetryGame()
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.BackToTitle();
		}
		else
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene(0);
		}
	}

	public void EndGameFunc()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); 
#endif
	}

	public void RefreshDisplay(System.Collections.Generic.List<HighscoreElement> highscoreList)
	{
		foreach (var item in spawnedItems)
		{
			if (item != null) Destroy(item);
		}
		spawnedItems.Clear();

		for (int i = 0; i < highscoreList.Count; i++)
		{
			HighscoreElement data = highscoreList[i];
			if (data == null || data.points <= 0) continue;

			GameObject inst = Instantiate(rankItemPrefab, itemContainer);
			spawnedItems.Add(inst);

			RankItem rankItem = inst.GetComponent<RankItem>();
			if (rankItem != null)
			{
				if (rankItem.rankText != null) rankItem.rankText.text = (i + 1).ToString();
				if (rankItem.playerNameText != null) rankItem.playerNameText.text = data.playerName;
				if (rankItem.scoreText != null) rankItem.scoreText.text = data.points.ToString();
			}
			else
			{
				var texts = inst.GetComponentsInChildren<Text>();
				if (texts.Length >= 2)
				{
					texts[0].text = data.playerName;
					texts[1].text = data.points.ToString();
				}
			}
		}
	}
}