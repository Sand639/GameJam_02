using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    //次のシーンを呼び出す(シーンリストに登録した順番)
    public void LoadNextScene()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        int maxIndex = SceneManager.sceneCountInBuildSettings;

        int nextIndex = (index + 1) % maxIndex;

        SceneManager.LoadScene(nextIndex);
    }
}