using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Types")]
    [SerializeField]
    private bool _isNormal = false;
    [SerializeField]
    private bool _isZigZag = false;
    [SerializeField]
    private bool _isAggressive = false;
    [SerializeField]
    private bool _isDodge = false;
    [SerializeField]
    private bool _isEnemyShootBackwards = false;

    [SerializeField]
    private float _enemyMovementSpeed = 4f;
    private Player _player;
    private Animator _animator;
    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _laserAudio;
    private SpawnManager _spawnManager;
    private bool enemyIsAlive = true;
    [SerializeField]
    private GameObject _laser_Prefab;
       
    private float _fireRate = 3.0f;
    private float _fireRateAtPowerup = 5.0f;
 
    private float _canFire = -1f;
    private float _canFireAtPowerup = -1f;
    //ZigZag
    private float _frequency = 3f;
    private float _magnitude = 2f;
    //Shields
    [SerializeField]
    private GameObject _shields;
    private bool _isShieldsActive = false;
    private int _shieldChance;
    private int _shieldPower;

    //Dodge
    private bool _laserDetected = false;
    private int _randomNumber;

    private Vector3 _position;
    private Vector3 _axis;

    private SpriteRenderer _spriteRenderer;
    

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent <Player>();
        _audioSource = GetComponent<AudioSource>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_player == null)
        {
            Debug.LogError("Player is Null");            
        }

        _animator = GetComponent<Animator>();

        if (_animator == null)
        {
            Debug.LogError("Animator is Null");
        }

        if(_spawnManager == null)
        {
            Debug.LogError("SpawnManager is Null");
        }
        _position = transform.position;
        _axis = transform.right;

        _isShieldsActive = false;
        _shields.SetActive(false);
        ShieldCheck();
        _randomNumber = Random.Range(0, 2) * 2 - 1;
        
    }

    // Update is called once per frame
    void Update()
    {
        DiagonalMovement();
        CalculateMovement();
        AggressiveEnemy();
        DodgeMovement();
        ShootPowerups();
        ShootBackwardsEnemy();

        if(Time.time > _canFire)
        {
            if(enemyIsAlive == true)
            {
                _fireRate = Random.Range(3f, 5f);
                _canFire = Time.time + _fireRate;
                GameObject enemyLaser = Instantiate(_laser_Prefab, transform.position, Quaternion.identity);
                AudioSource.PlayClipAtPoint(_laserAudio, new Vector3(0, 0, -10), 1.0f);
                Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            
                for (int i = 0; i < lasers.Length; i++)
                    {
                    lasers[i].AssignEnemyLaser();
                    }
            }
            else
            {
                enemyIsAlive = false;
            }
        }
    }

    void CalculateMovement()
    {
        if(_isNormal == true)
        {
            transform.Translate(Vector3.down * _enemyMovementSpeed * Time.deltaTime);
            if (transform.position.y < -6f)
            {
                transform.position = new Vector3(Random.Range(-9f, 9f), 8f, 0);
            }
        }        
    }

    private void DodgeMovement()
    {
        if (_isDodge == true)
        {
            transform.Translate(Vector3.down * _enemyMovementSpeed * Time.deltaTime);
            if (transform.position.y < -6f)
            {
                transform.position = new Vector3(Random.Range(-9f, 9f), 8f, 0);
            }
            if(_laserDetected)
            {
                transform.Translate(new Vector3(_randomNumber * 5, -1, 0) * _enemyMovementSpeed * Time.deltaTime);
            }
        }
    }

  public void LaserFound(bool status)
    {
        _laserDetected = status;
    }

    private void DiagonalMovement()
    {
        if(_isZigZag == true)
        {
            _position += Vector3.down * Time.deltaTime * _enemyMovementSpeed;
            transform.position = _position + _axis * Mathf.Sin(Time.time * _frequency) * _magnitude;
            if (transform.position.y < -6f)
            {
                transform.position = new Vector3(Random.Range(-9f, 9f), 8f, 0);
                _position = transform.position;
            }
        }
    }

    private void AggressiveEnemy()
    {
        if(_isAggressive == true)
        {
            transform.Translate(Vector3.down * _enemyMovementSpeed * Time.deltaTime);
            if (transform.position.y < -6f)
            {
                transform.position = new Vector3(Random.Range(-9f, 9f), 8f, 0);
            }
            if ((transform.position - _player.transform.position).magnitude < 5)
            {
                Vector3 distance = _player.transform.position - transform.position;
                transform.Translate(distance * 1.5f * Time.deltaTime);
                _spriteRenderer.color = Color.blue;
            }
        }
    }

    private void ShootPowerups()
    {
        RaycastHit2D hitRay = Physics2D.Raycast(transform.position,
            transform.TransformDirection(Vector2.down), 10.0f, 1 << LayerMask.NameToLayer("Powerup"));
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.down) * 10.0f, Color.red);

        if (hitRay.collider != null)
        {
            if (hitRay.collider.gameObject.CompareTag("Powerup_Item"))
            {
                if (Time.time > _canFireAtPowerup)
                {
                    if (enemyIsAlive == true)
                    {
                        _canFireAtPowerup = Time.time + _fireRateAtPowerup;
                        GameObject enemyLaser = Instantiate(_laser_Prefab, transform.position, Quaternion.identity);
                        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

                        for (int i = 0; i < lasers.Length; i++)
                        {
                            lasers[i].AssignEnemyLaser();
                        }
                        Debug.Log("Shooting Powerup");
                    }
                    else
                    {
                        enemyIsAlive = false;
                    }
                }
            }
        }
    }

    private void ShootBackwardsEnemy()
    {
        if (_isEnemyShootBackwards == true)
        {
            if (Time.time > _canFire)
            {
                if (enemyIsAlive == true)
                {
                    _fireRate = Random.Range(1f, 3f);
                    _canFire = Time.time + _fireRate;
                    if (_player != null)
                    {
                        if (transform.position.y > _player.transform.position.y)
                        {
                            Vector3 laserOffset = new Vector3(0, 0.45f, 0);
                            GameObject enemyLaser = Instantiate(_laser_Prefab, transform.position, Quaternion.identity);
                            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

                            for (int i = 0; i < lasers.Length; i++)
                            {
                                lasers[i].AssignEnemyLaser();
                            }
                        }
                        else if (transform.position.y < _player.transform.position.y)
                        {
                            Vector3 laserOffset = new Vector3(0, 5f, 0);
                            GameObject enemyLaser = Instantiate(_laser_Prefab, transform.position + laserOffset, Quaternion.identity);
                            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

                            for (int i = 0; i < lasers.Length; i++)
                            {
                                lasers[i].AssignEnemyLaser();
                                lasers[i].AssignDoubleSidedLaser();
                            }
                        }
                    }
                }
                else
                {
                    enemyIsAlive = false;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.transform.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            if(player != null)
            {
                player.Damage();
            }
            _animator.SetTrigger("OnEnemyDeath");
            _enemyMovementSpeed = 0;
            _audioSource.Play();
            Destroy(GetComponent<Collider2D>());
            Destroy(gameObject,2.8f);
            enemyIsAlive = false;
            _spawnManager.EnemyKilled();
        }

        
        if (other.transform.tag == "Laser" && _isShieldsActive == true)
        {
            Destroy(other.gameObject);
            _shieldPower--;
            if(_shieldPower == 0)
            {
                _shields.SetActive(false);
                StartCoroutine(ShieldChangeDelay());
            }
            return;
        }

        if (other.transform.tag == "Laser" && _isShieldsActive == false)
        { 
            Destroy(other.gameObject);
            if (_player != null)
            {
                _player.AddScore(10);
            }
            _animator.SetTrigger("OnEnemyDeath");
            _enemyMovementSpeed = 0;
            _audioSource.Play();
            Destroy(GetComponent<Collider2D>());
            Destroy(gameObject,2.8f);
            enemyIsAlive = false;
            _spawnManager.EnemyKilled();
        }
        else if(other.tag == "Missile")
        {
            Destroy(other.gameObject);
            if(_player != null)
            {
                _player.AddScore(10);
            }
            _animator.SetTrigger("OnEnemyDeath");
            _enemyMovementSpeed = 0;
            _audioSource.Play();
            Destroy(gameObject, 2.8f);
            enemyIsAlive = false;
            _spawnManager.EnemyKilled();
        }
    }

    private void ShieldIsActive()
    {
        _isShieldsActive = true;
        _shields.SetActive(true);    
        _shieldPower = 1;
        Debug.Log("Shields Active");
    }

    private void ShieldCheck()
    {
        _shieldChance = Random.Range(0, 4);
        if(_shieldChance == 0)
        {
            ShieldIsActive();
        }
    }

    IEnumerator ShieldChangeDelay()
    {
        yield return new WaitForSeconds(1f);
        _isShieldsActive = false;
    }
}
