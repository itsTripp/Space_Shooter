using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private Text _ammoText;
    [SerializeField]
    private Image _livesImg;
    [SerializeField]
    private Sprite[] _liveSprites;
    [SerializeField]
    private Text _gameOverText;
    [SerializeField]
    private Text _restartLevelText;
    [SerializeField]
    private Text _waveText;
    [SerializeField]
    private Slider _thrusterSlider;
    public float maxFuel = 100f;
    public float currentFuel;
    [SerializeField]
    public Slider _bossHealthSlider;
    private GameManager _gameManager;
    private SpawnManager _spawnManager;

    // Start is called before the first frame update
    void Start()
    {
        currentFuel = maxFuel;
        _scoreText.text = "Score: " + 0;
        _gameOverText.gameObject.SetActive(false);
        _restartLevelText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        if (_gameManager == null)
        {
            Debug.LogError("Game Manager is null");
        }

        if(_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is Null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        _thrusterSlider.value = currentFuel / maxFuel;
    }

    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore;
    }

    public void UpdateLives(int currentLives)
    {
        _livesImg.sprite = _liveSprites[currentLives];

        if (currentLives == 0)
        {
            GameOverSequence();

        }
    }

    void GameOverSequence()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        _restartLevelText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            _gameOverText.text = "Game Over";
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void SpawnNextWave()
    {
        StartCoroutine(WaveTextEnableRoutine());
    }

    IEnumerator WaveTextEnableRoutine()
    {
        _waveText.text = "Wave " + _spawnManager.GetWaveNumber();
        _waveText.gameObject.SetActive(true);
        _spawnManager.EnableNextWaveSpawning();
        yield return new WaitForSeconds(3f);
        _waveText.gameObject.SetActive(false);
    }
    public void UpdateAmmoCount(int ammoCount, int maximumAmmo)
    {
        _ammoText.text = "Ammo: " + ammoCount + " / " + maximumAmmo;

        if(ammoCount == 0)
        {
            _ammoText.color = Color.red;
        }
        else
        {
            _ammoText.color = Color.white;
        }
    }

    public void SetBossMaxHealth(int health)
    {
        _bossHealthSlider.maxValue = health;
        _bossHealthSlider.value = health;
    }

    public void SetBossHealth(int health)
    {
        _bossHealthSlider.value = health;
    }
}
