using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemy;
    [SerializeField]
    private GameObject _enemyContainer;
    private bool _stopSpawning = false;
    public float spawnRate = 5f;
    [SerializeField]
    private GameObject[] _powerups;

    public int[] table =
    {
        40, //Ammo
        20, //TripleShot
        10, //Speed
        10, //Shield
        //10, //PowerDown
        5,  //Health
        5,   //SprayShot
        10   //No Ammo
    };

    private int _totalWeight = 100;
    private int _randomNumber;
    [SerializeField]
    private bool _isGameActive = true;
    [SerializeField]
    private bool _spawnEnemyWave = true;
    [SerializeField]
    private int _currentEnemies = 0;
    [SerializeField]
    private int _enemiesInCurrentWave = 15;
    [SerializeField]
    private int _waveNumber = 1;
    private UIManager _uiManager;

    private void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if (_uiManager == null)
        {
            Debug.LogError("UI Manager is Null");
        }
    }

    private void Update()
    {
        if (_currentEnemies <= 0 && _spawnEnemyWave == false)
        {
           /* if (_waveNumber == 8)
            {
                Vector3 bossSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
                //Instantiate(_bossEnemyPrefab, bossSpawn, Quaternion.identity);
                _currentEnemies++;
                return;
            }*/
            EnableNextWaveSpawning();
            _uiManager.SpawnNextWave();
            StartEnemySpawning();
        }
        if(_isGameActive == false)
        {
            StopCoroutine(SpawnPowerupRoutine());
            StopCoroutine(SpawnEnemyRoutine());
        }
    }
    public void StartSpawning()
    {
        _uiManager.SpawnNextWave();
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3f);
        while (_isGameActive && _spawnEnemyWave)
        {
            for (int i = 0; i < _enemiesInCurrentWave; i++)
            {
                Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
                GameObject newEnemy = Instantiate(_enemy, posToSpawn, Quaternion.identity);
                newEnemy.transform.parent = _enemyContainer.transform;
                _currentEnemies++;
                yield return new WaitForSeconds(5f);

                if (_isGameActive == false)
                {
                    break;
                }
            }
            _enemiesInCurrentWave += 15;
            _waveNumber++;
            _spawnEnemyWave = false;
        }
    }

    public void StartEnemySpawning()
    {
        if(_waveNumber % 2 == 0)
        {
            spawnRate -= 0.2f;
            if(spawnRate <=0.4f)
            {
                spawnRate = 0.4f;
            }
        }
        StartCoroutine(SpawnEnemyRoutine());
    }

    public void EnemyKilled()
    {
        _currentEnemies--;
    }

    public int GetWaveNumber()
    {
        return _waveNumber;
    }

    public void EnableNextWaveSpawning()
    {
        _spawnEnemyWave = true;
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3f);
        while (_stopSpawning == false)
        {
            ChoosePowerUp();
            yield return new WaitForSeconds(Random.Range(3f, 7f));
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

    private void ChoosePowerUp()
    {
        float randomX = Random.Range(-8f, 8f);

        _randomNumber = Random.Range(0, _totalWeight);
        Debug.Log("Random Number is " + _randomNumber);

        for (int i = 0; i < table.Length; i++)
        {
            if (_randomNumber <= table[i])
            {
                Instantiate(_powerups[i], new Vector3(randomX, 7, 0), Quaternion.identity);
                return;
            }
            else
            {
                _randomNumber -= table[i];
            }
        }
    }
}