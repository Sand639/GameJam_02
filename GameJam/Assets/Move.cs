using UnityEngine;
using UnityEngine.SceneManagement;

public class Move : MonoBehaviour
{
    public float speed = 5f;   // 移動速度
    public float jumpForce = 5f; // ジャンプ力
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Moving();
        CheckFall();
    }

    // 移動処理（WASD）
    void Moving()
    {
        float x = Input.GetAxis("Horizontal"); // A,D
        float z = Input.GetAxis("Vertical");   // W,S

        Vector3 move = new Vector3(x, 0, z) * speed;

        // 位置移動
        transform.Translate(move * Time.deltaTime);

        // ジャンプ（スペースキー）
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    // 落下チェック
    void CheckFall()
    {
        if (transform.position.y < -1f)
        {
            LoadNextScene();
        }
    }

    // シーン遷移（ループ対応）
    void LoadNextScene()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        int maxIndex = SceneManager.sceneCountInBuildSettings;

        int nextIndex = (index + 1) % maxIndex;

        SceneManager.LoadScene(nextIndex);
    }
}
