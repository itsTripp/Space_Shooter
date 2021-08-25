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
    
    public UIManager _bossHealthBar;

    // Start is called before the first frame update
    void Start()
    {
        _bossHealthBar = GameObject.Find("Canvas").GetComponent<UIManager>();
        if(_bossHealthBar == null)
        {
            Debug.LogError("UIMananger on Boss is Null");
        }
        _bossCurrentHealth = _bossMaxHealth;
        _bossHealthBar.SetBossMaxHealth(_bossMaxHealth);
        
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();

        if(_animator == null)
        {
            Debug.LogError("Animator on Boss is Null");
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
            _bossCurrentHealth--;
            _bossHealthBar.SetBossHealth(_bossCurrentHealth);
            if (_bossCurrentHealth == 0)
            {
                _animator.SetTrigger("OnEnemyDeath");
                Destroy(gameObject);
                _audioSource.Play();
            }
        }
    }

}
