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

    private void Start()
    {
        
    }
    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3f);
        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
            GameObject newEnemy = Instantiate(_enemy, posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(5f);
        }       
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
            if(_randomNumber <= table[i])
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
