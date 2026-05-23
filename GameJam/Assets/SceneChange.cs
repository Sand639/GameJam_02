using UnityEngine;
using UnityEngine.SceneManagement; // これが必須

public class SceneChanger : MonoBehaviour
{
    //次のシーンを呼び出す
    public void LoadNextScene()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        int maxIndex = SceneManager.sceneCountInBuildSettings;

        int nextIndex = (index + 1) % maxIndex;

        SceneManager.LoadScene(nextIndex);
    }
}