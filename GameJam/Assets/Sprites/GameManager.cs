using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	[Header("Game Data Transfer")]
	public int CurrentGameScore { get; private set; } = 0;

	[Header("Scene Names")]
	[SerializeField] private string titleSceneName = "TitleScene";
	[SerializeField] private string gameSceneName = "GameScene";
	[SerializeField] private string resultSceneName = "ResultScene";

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
			return;
		}
	}


	public void SaveAndGoToResult(int score)
	{
		CurrentGameScore = score;

		Debug.Log($"[GameManager] get {CurrentGameScore} score。switch to Result Scene。");
		SceneManager.LoadScene(resultSceneName);
	}

	public void RestartGame()
	{
		CurrentGameScore = 0;
		Time.timeScale = 1f;
		Debug.Log("[GameManager] score reset，restartgame。");
		SceneManager.LoadScene(gameSceneName);
	}

	public void BackToTitle()
	{
		CurrentGameScore = 0;
		Time.timeScale = 1f;
		Debug.Log("[GameManager] score reset，back to title。");
		SceneManager.LoadScene(titleSceneName);
	}
}