using UnityEngine;

public class SpaceKeyChanger : MonoBehaviour
{
    public SceneChanger sceneChanger;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            sceneChanger.LoadNextScene();
        }
    }
}