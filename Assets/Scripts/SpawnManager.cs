using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _enemy;
    [SerializeField]
    private GameObject _bossEnemyPrefab;
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
        10,  //No Ammo
        //10, //PowerDown
        5,  //Health
        5   //SprayShot
    };

    public int[] enemyTable =
    {
        25, //Basic
        25, //ZigZag
        25, //Dodge
        25 //Aggressive
    };

    private int _powerupTotalWeight;
    private int _enemyTotalWeight;
    private int _powerupRandomNumber;
    private int _enemyRandomNumber;
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

        foreach(var item in table)
        {
            _powerupTotalWeight += item;
        }
        foreach(var item in enemyTable)
        {
            _enemyTotalWeight += item;
        }
    }

    private void Update()
    {
        if (_currentEnemies <= 0 && _spawnEnemyWave == false)
        {
           if (_waveNumber == 2)
            {
                Vector3 bossSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
                GameObject newBoss = Instantiate(_bossEnemyPrefab, bossSpawn, Quaternion.identity);
                newBoss.transform.parent = _enemyContainer.transform;
                _currentEnemies++;
                return;
            }
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
                ChooseEnemy();
                _currentEnemies++;
                yield return new WaitForSeconds(5f);

                if (_isGameActive == false)
                {
                    break;
                }
            }
            _enemiesInCurrentWave += 3;
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

        _powerupRandomNumber = Random.Range(0, _powerupTotalWeight);
        //Debug.Log("Powerup Random Number is " + _powerupRandomNumber);

        for (int i = 0; i < table.Length; i++)
        {
            if (_powerupRandomNumber <= table[i])
            {
                Instantiate(_powerups[i], new Vector3(randomX, 7, 0), Quaternion.identity);
                return;
            }
            else
            {
                _powerupRandomNumber -= table[i];
            }
        }
    }

    private void ChooseEnemy()
    {
        _enemyRandomNumber = Random.Range(0, _enemyTotalWeight);
        //Debug.Log("Enemy Random Number is " + _enemyRandomNumber);
        for (int i = 0; i < enemyTable.Length; i++)
        {
            if (_enemyRandomNumber <= enemyTable[i])
            {
                Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
                GameObject newEnemy = Instantiate(_enemy[i], posToSpawn, Quaternion.identity);
                newEnemy.transform.parent = _enemyContainer.transform;
                return;
            }
            else
            {
                _enemyRandomNumber -= enemyTable[i];
            }
        }
    }
}