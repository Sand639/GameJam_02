using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Player : MonoBehaviour
{
    static float s_energy = 100;
    static float s_energyMax = 100;

    static bool s_isAddRequested = false;
    static float s_addEnergy = 0;

    [SerializeField] float _energy = s_energy;
    [SerializeField] float _energyMax = s_energyMax;
    [SerializeField] float _energyCost = 0.03f;

    [SerializeField] GameObject _playerModel = null;
    [SerializeField] float _playerAnimIdleShakePower = 1;
    [SerializeField] float _playerAnimIdleShakeSpeed = 1;
    [SerializeField] float _playerAnimShakePower = 1;
    [SerializeField] float _playerAnimShakeSpeed = 1;

    [SerializeField] float _laneMiddlePlayerPosX = 0;
    [SerializeField] float _laneLeftPlayerPosX = -1.0f;
    [SerializeField] float _laneRightPlayerPosX = 1.0f;

    bool _isDead = false;

    int _lanePos = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDead) return;

        if (_playerModel)
        {
            float energyRate = _energy / _energyMax;

            float baseShake = Mathf.Sin(Time.time * _playerAnimIdleShakeSpeed) * _playerAnimIdleShakePower;

            float randomIntensity = Easing.InQuad(1f - energyRate);
            Vector3 randomShake = Random.insideUnitSphere * _playerAnimShakePower * randomIntensity;

            _playerModel.transform.localPosition = randomShake + new Vector3(0f, baseShake, 0f);
            //_playerModel.transform.localRotation =
            //    Quaternion.Euler(
            //        baseShake,
            //        0f,
            //        baseShake * 0.5f
            //    );
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(_lanePos < 1) _lanePos++;
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if(_lanePos > -1) _lanePos--;
        }

        {
            float targetX = _lanePos switch
            {
                -1 => _laneLeftPlayerPosX,
                0 => _laneMiddlePlayerPosX,
                1 => _laneRightPlayerPosX,
                _ => _laneMiddlePlayerPosX
            };
            Vector3 pos = transform.position;
            pos.x = Mathf.Lerp(pos.x, targetX, Time.deltaTime * 10f);
            transform.position = pos;
        }

        if (s_isAddRequested)
        {
            AddEnergy(s_addEnergy);
            s_isAddRequested = false;
        }

        _energy -= _energyCost;

        if (_energy <= 0)
        {
            _isDead = true;
            _energy = 0;

            //一応
            Destroy(gameObject);
            Debug.Log("Player is dead.");
        }

        s_energy = _energy;
        s_energyMax = _energyMax;
    }

    public void AddEnergy(float amount)
    {
        _energy += amount;
        if (_energy > _energyMax)
        {
            _energy = _energyMax;
        }
    }

    public static void RequestAddEnergy(float amount)
    {
        s_isAddRequested = true;
        s_addEnergy = amount;
    }

    public static float GetEnergy()
    {
        return s_energy;
    }
}
