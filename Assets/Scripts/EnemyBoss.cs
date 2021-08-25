using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : MonoBehaviour
{
    [SerializeField]
    private float _bossMoveSpeed = .5f;
    private float _fireRate = 3.0f;
    private float _canFire = -1f;
    [SerializeField]
    private int _bossMaxHealth = 10;
    [SerializeField]
    private int _bossCurrentHealth;
    [SerializeField]
    private GameObject _bossLaserPrefab;
    private bool _enemyIsAlive = true;
    private AudioSource _audioSource;
    private Animator _animator;
    [SerializeField]
    private AudioClip _bossAudio;
    
    public UIManager _uiManager;
    private Player _player;
    private SpawnManager _spawnManager;

    // Start is called before the first frame update
    void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if(_uiManager == null)
        {
            Debug.LogError("UIMananger on Boss is Null");
        }

        _bossCurrentHealth = _bossMaxHealth;
        _uiManager.SetBossMaxHealth(_bossMaxHealth);
        
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();

        if(_animator == null)
        {
            Debug.LogError("Animator on Boss is Null");
        }

        _player = GameObject.Find("Player").GetComponent<Player>();
        if(_player == null)
        {
            Debug.LogError("Player on Boss is Null");
        }

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if(_spawnManager == null)
        {
            Debug.LogError("SpawnManager on Boss is Null");
        }

        StartCoroutine(BossEntrance(new Vector3(0, 3f, 0)));
    }

    // Update is called once per frame
    void Update()
    {
        BossShoot();
    }
    private IEnumerator BossEntrance(Vector3 dir)
    {
        while (transform.position != dir)
        {
            transform.position = Vector3.MoveTowards(transform.position, dir, _bossMoveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void BossShoot()
    {
        if(Time.time > _canFire)
        {
            if (_enemyIsAlive == true)
            {
                _fireRate = Random.Range(1f, 2.5f);
                _canFire = Time.time + _fireRate;
                GameObject bossLaser = Instantiate(_bossLaserPrefab, transform.position + 
                    new Vector3(0, -2.5f, 0), Quaternion.identity);
                AudioSource.PlayClipAtPoint(_bossAudio, new Vector3(0, 0, -10), 1.0f);
                Laser[] lasers = bossLaser.GetComponentsInChildren<Laser>();

                for (int i = 0; i < lasers.Length; i++)
                {
                    lasers[i].AssignEnemyLaser();
                }
            }
            else
            {
                _enemyIsAlive = false;
            }
            
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Laser")
        {
            Destroy(other.gameObject);
            if(_player != null)
            {
                _player.AddScore(10);
            }
            
            _bossCurrentHealth--;
            _uiManager.SetBossHealth(_bossCurrentHealth);
            if (_bossCurrentHealth == 0)
            {
                if (_player != null)
                {
                    _player.AddScore(1000);
                }
                _animator.SetTrigger("OnEnemyDeath");
                _audioSource.Play();
                _spawnManager.EnemyKilled();
                _enemyIsAlive = false;
                Destroy(gameObject,2.8f);
                _uiManager._bossHealthSlider.gameObject.SetActive(false);
            }
        }
        if(other.tag == "Missile")
        {
            Destroy(other.gameObject);
            if(_player != null)
            {
                _player.AddScore(10);
            }
            
            _bossCurrentHealth --;
            _uiManager.SetBossHealth(_bossCurrentHealth);
            if(_bossCurrentHealth == 0)
            {
                if (_player != null)
                {
                    _player.AddScore(1000);
                }
                _animator.SetTrigger("OnEnemyDeath");
                _audioSource.Play();
                _spawnManager.EnemyKilled();
                _enemyIsAlive = false;
                Destroy(gameObject,2.8f);
                _uiManager._bossHealthSlider.gameObject.SetActive(false);
                
            }
        }
    }

}
