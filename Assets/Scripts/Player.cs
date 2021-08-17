using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Speeds")]
    [SerializeField]
    private float _currentSpeed;
    [SerializeField]
    private float _speed = 5;
    [SerializeField]
    private float _speedMultiplier = 2;
    [SerializeField]
    private float _thrusterSpeed = 8;  
    
    [Header("Powerups")]
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _sprayShotPrefab;
    [SerializeField]
    private GameObject _shieldPrefab;
    [SerializeField]
    private SpriteRenderer _shieldRenderer;
    [SerializeField]
    private int _shieldStrength = 3;

    private bool _isTripleShotActive = false;
    private bool _isSprayShotActive = false;
    private bool _isSpeedBoostActive = false;
    private bool _isShieldsActive = false;
    private bool _canPlayerShoot = true;

    [SerializeField]
    private float _fireRate = 0.5f;
    private float _canFire = -1f;
    [SerializeField]
    private int _ammoCount = 15;
    private int _maximumAmmo;

    [Header("UI Elements")]
    [SerializeField]
    private int _lives = 3;

    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    [SerializeField]
    private int _score;

    [SerializeField]
    private float _fuelBurnRate = 30f;
    [SerializeField]
    private float _fuelRefillRate = 20f;
    [SerializeField]
    private float _thrusterRefillCoolDown = 2f;
    private float _canRefillThrust;

    [Header("Player Visuals")]
    [SerializeField]
    private GameObject _rightEngine;
    [SerializeField]
    private GameObject _leftEngine;
    [SerializeField]
    private GameObject _thruster;
    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _laserAudio;
    private Camera_Shake _shake;



    // Start is called before the first frame update
    void Start()
    {
        _currentSpeed = _speed;
        _maximumAmmo = _ammoCount;
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _audioSource = GetComponent<AudioSource>();
        _shake = GameObject.Find("Main Camera").GetComponent<Camera_Shake>();

        if(_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is Null");
        }

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is Null");
        }

        if (_audioSource == null)
        {
            Debug.LogError("The Audio Source on the player is Null");
        }
        else
        {
            _audioSource.clip = _laserAudio;
        }

        if(_shake == null)
        {
            Debug.LogError("The Camera Shake is null");
        }

        _rightEngine.SetActive(false);
        _leftEngine.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        UpdateEngines();

        if(Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            ShootLaser();
        }
    }
    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        
        transform.Translate(direction * _currentSpeed * Time.deltaTime);
        
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        if (transform.position.x > 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }
        else if (transform.position.x < -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }

        Thrusters();
    }

    private void Thrusters()
    {
        if (Input.GetKey(KeyCode.LeftShift) && _uiManager.currentFuel > 0)
        {
            _thruster.SetActive(true);
            CalculateFuelUse();
            _currentSpeed = _thrusterSpeed;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _thruster.SetActive(false);
            _currentSpeed = _speed;
            _canRefillThrust = Time.time + _thrusterRefillCoolDown;
        }
        else if (_isSpeedBoostActive == false)
        {
            _currentSpeed = _speed;
            RefillFuel();
        }
    }

    private void CalculateFuelUse()
    {
        _uiManager.currentFuel -= _fuelBurnRate * Time.deltaTime;
    }

    public void RefillFuel()
    {
        if(_uiManager.currentFuel < _uiManager.maxFuel && Time.time > _canRefillThrust)
        {
            _uiManager.currentFuel += _fuelRefillRate * Time.deltaTime;
        }
        else if (_uiManager.currentFuel > _uiManager.maxFuel)
        {
            _uiManager.currentFuel = _uiManager.maxFuel;
        }
    }

    void ShootLaser()
    {
        if (_ammoCount > 0)
        {
            _canPlayerShoot = true;
            _canFire = Time.time + _fireRate;

            if (_isTripleShotActive == true)
            {
                _isSprayShotActive = false;
                Instantiate(_tripleShotPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
            }
            else if (_isSprayShotActive == true)
            {
                _isTripleShotActive = false;
                Instantiate(_sprayShotPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
            }
            else
            {
                Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
            }

            _audioSource.Play();
            _ammoCount--;
            _uiManager.UpdateAmmoCount(_ammoCount, _maximumAmmo);
        }
        else
        {
            _canPlayerShoot = false;
        }
        
    }

    public void Damage()
    {
        if(_isShieldsActive == true)
        {
            _shieldStrength -= 1;

            switch (_shieldStrength)
            {
                case 0:
                    _isShieldsActive = false;
                    _shieldPrefab.SetActive(false);
                    _shieldRenderer.color = Color.cyan;
                    break;

                case 1:
                    _shieldRenderer.color = Color.red;
                    break;

                case 2:
                    _shieldRenderer.color = Color.yellow;
                    break;
            }
            return;
        }

        _lives -= 1;
        _shake.CameraShake();
        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(gameObject);
        }
    }
    private void UpdateEngines()
    {
        if (_lives == 3)
        {
            _rightEngine.SetActive(false);
            _leftEngine.SetActive(false);
        }
        else if (_lives == 2)
        {
            _rightEngine.SetActive(true);
            _leftEngine.SetActive(false);
        }

        else if (_lives == 1)
        {
            _rightEngine.SetActive(true);
            _leftEngine.SetActive(true);
        }
    }


    public void ShieldActive()
    {
        _isShieldsActive = true;
        _shieldPrefab.SetActive(true);
        _shieldStrength = 3;
        _shieldRenderer.color = Color.cyan;
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }
    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isTripleShotActive = false;
    }
    public void SprayShotActive()
    {
        _isSprayShotActive = true;
        StartCoroutine(SprayShotPowerDownRoutine());
    }

    IEnumerator SprayShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isSprayShotActive = false;
    }
    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        _currentSpeed *= _speedMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }
    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isSpeedBoostActive = false;
        _currentSpeed /= _speedMultiplier;
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }

    public void AddHealth()
    {
        if(_lives == 3)
            {
                _lives = 3;
            }
        if(_lives <= 2)
        {
            _lives++;
            _uiManager.UpdateLives(_lives);
        }
    }

    public void AddAmmo()
    {
        _ammoCount = 15;
        _uiManager.UpdateAmmoCount(_ammoCount,_maximumAmmo);
    }
    
    public void NoAmmoPowerup()
    {
        _ammoCount = 0;
        _uiManager.UpdateAmmoCount(_ammoCount, _maximumAmmo);
    }
}
