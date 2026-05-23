using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Player : MonoBehaviour
{
    static float s_energy = 100;
    static float s_energyMax = 100;
    static int s_coin = 0;

    static bool s_isAddRequested = false;
    static float s_addEnergy = 0;

    [SerializeField] float _energy = s_energy;
    [SerializeField] float _energyMax = s_energyMax;
    [SerializeField] float _energyCost = 0.03f;
    [SerializeField] float _energyHitDamage = 5.0f;
    [SerializeField] int _coinValue = 0;

    [SerializeField] GameObject _playerModel = null;
    [SerializeField] float _playerAnimIdleShakePower = 1;
    [SerializeField] float _playerAnimIdleShakeSpeed = 1;
    [SerializeField] float _playerAnimShakePower = 1;
    [SerializeField] float _playerAnimShakeSpeed = 1;

    [SerializeField] float _laneMiddlePlayerPosX = 0;
    [SerializeField] float _laneLeftPlayerPosX = -1.0f;
    [SerializeField] float _laneRightPlayerPosX = 1.0f;
    [SerializeField] LayerMask _targetLayer;
    [SerializeField] float _targetradius = 5f;
    [SerializeField] LayerMask _coinLayer;
    [SerializeField] float _coinRadius = 1f;
    [SerializeField] LayerMask _eventBoxLayer;
    [SerializeField] float _eventBoxRadius = 1f;

    bool _isDead = false;
    bool _isTargetHit = false;

    int _lanePos = 0;

    bool IsTargetInRange()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _targetradius, _targetLayer);
        return hits.Length > 0;
    }
    Collider[] GetCoinInRange()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _coinRadius, _coinLayer);
        return hits;
    }
    Collider[] GetEventBoxInRange()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _eventBoxRadius, _eventBoxLayer);
        return hits;
    }

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
            if (_lanePos < 1) _lanePos++;
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (_lanePos > -1) _lanePos--;
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
            s_addEnergy = 0;
            s_isAddRequested = false;
        }

        if (IsTargetInRange())
        {
            if (!_isTargetHit)
            {
                Camera.RequestShake(1.0f, 0.5f);
                _energy -= _energyHitDamage;

                //Debug.Log("Target Hit!");
            }
            _isTargetHit = true;
        }
        else
        {
            _isTargetHit = false;
        }

        {
            var coins = GetCoinInRange();
            if (coins.Length > 0)
            {
                foreach (var coin in coins)
                {
                    _coinValue++;
                    Destroy(coin.gameObject);
                }
            }
        }
        {
            var eventBoxes = GetEventBoxInRange();
            if (eventBoxes.Length > 0)
            {
                foreach (var eventBox in eventBoxes)
                {
                    var box = eventBox.GetComponent<EventBox>();
                    if (box != null)
                    {
                        box.MiniGameStart();
                    }
                    Destroy(eventBox.gameObject);
                }
            }
        }

        _energy -= _energyCost * Time.deltaTime;

        if (_energy <= 0)
        {
            _isDead = true;
            _energy = 0;

            if(GameManager.Instance != null)
            {
				GameManager.Instance.SaveAndGoToResult(ScoreManager.instance.GetScore());
			}
			//一応
			Destroy(gameObject);
            Debug.Log("Player is dead.");

        }

        s_energy = _energy;
        s_energyMax = _energyMax;
        s_coin = _coinValue;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (LayerMask.LayerToName(other.gameObject.layer) == "Enemy")
    //    {
    //        Debug.Log("Hit Enemy");
    //    }
    //}
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (LayerMask.LayerToName(collision.gameObject.layer) == "Enemy")
    //    {
    //        Debug.Log("Collided with Enemy");
    //    }
    //}

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
        s_addEnergy += amount;
    }

    public static float GetEnergy()
    {
        return s_energy;
    }

    public static int GetCoin()
    {
        return s_coin;
    }
}