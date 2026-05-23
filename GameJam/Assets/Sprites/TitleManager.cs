using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private Button StartButton;

    private void Awake()
    {
        if (StartButton != null) StartButton.onClick.AddListener(OnStartButtonClicked);
    }
    void Start()
    {

    }

    void Update()
    {

    }

    private void OnStartButtonClicked()
    {
     if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
        else
        {
            Debug.LogError("unfind GameManager ");
		}
	}
}
