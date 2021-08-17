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
    private float _enemyMovementSpeed = 4f;
    private Player _player;
    private Animator _animator;
    private AudioSource _audioSource;
    private SpawnManager _spawnManager;
    private bool enemyIsAlive = true;
    [SerializeField]
    private GameObject _laser_Prefab;
    private float _fireRate = 3.0f;
    private float _canFire = -1f;
    //ZigZag
    private float _frequency = 3f;
    private float _magnitude = 2f;

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
    }

    // Update is called once per frame
    void Update()
    {
        DiagonalMovement();
        CalculateMovement();
        AggressiveEnemy();

        if(Time.time > _canFire && enemyIsAlive == true)
        {
            _fireRate = Random.Range(3f, 5f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laser_Prefab, transform.position, Quaternion.identity);
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

        if (other.transform.tag == "Laser")
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
    }
}
