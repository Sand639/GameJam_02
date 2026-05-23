using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField]
    float _shakeBasePower = 1f;

    [SerializeField]
    bool _debugShakeFlag = false;
    [SerializeField]
    float _debugShakePower = 1f;
    [SerializeField]
    float _debugShakeTime = 0;

    float _shakePower = 1f;
    float _shakeTime = 0;
    float _shakeTimeMax = 1.0f;

    static bool s_isRequested = false;
    static float s_shakePower = 1f;
    static float s_shakeTime = 0;
    static float s_shakeTimeMax = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (_debugShakeFlag)
        {
            RequestShake(_debugShakePower, _debugShakeTime);
    
            _debugShakeFlag = false;
        }

        if (s_isRequested)
        {
            _shakePower = s_shakePower;
            _shakeTimeMax = s_shakeTimeMax;
            _shakeTime = s_shakeTime;
    
            s_isRequested = false;
        }

        if (_shakeTime > 0.0f)
        {
            float shakeAmount = _shakeBasePower * (_shakeTime / _shakeTimeMax);
            Vector3 shakeOffset = Random.insideUnitSphere * shakeAmount;
            transform.localPosition = shakeOffset;

            _shakeTime -= Time.deltaTime;
        }
        else
        {
            transform.localPosition = Vector3.zero;
        }
    }

    public static void RequestShake(float power, float time)
    {
        s_isRequested = true;
        s_shakePower = power;
        s_shakeTimeMax = time;
        s_shakeTime = s_shakeTimeMax;
    }
}
