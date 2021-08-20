using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _powerupSpeed = 3f;

    //Powerup ID's
    [SerializeField] //0 = TripleShot, 1 = Speed, 2 = Shield
    private int _powerupID;
    [SerializeField]
    private AudioClip _audioClip;
    private Player _player;

    [SerializeField]
    private GameObject _explosion;
    [SerializeField]
    private AudioClip _explosionAudio;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _powerupSpeed * Time.deltaTime);
        if (transform.position.y < -5f)
        {
            Destroy(this.gameObject);
        }
        
        if(Input.GetKey(KeyCode.C))
        {
            if ((transform.position - _player.transform.position).magnitude < 5)
            {
                Vector3 distance = _player.transform.position - transform.position;
                transform.Translate(distance * 1.5f * Time.deltaTime);
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Player")
        {
            Player player = other.gameObject.GetComponent<Player>();

            AudioSource.PlayClipAtPoint(_audioClip, transform.position);
            if (player != null)
            {              
                switch(_powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        player.ShieldActive();
                        break;
                    case 3:
                        player.AddHealth();
                        break;
                    case 4:
                        player.AddAmmo();
                        break;
                    case 5:
                        player.SprayShotActive();
                        break;
                    case 6:
                        player.NoAmmoPowerup();
                        break;
                    case 7:
                        player.HomingProjectileSetActive();
                        break;
                }
                Destroy(this.gameObject);
            }
        }

        if (other.transform.tag == "Laser")
        {
            Destroy(other.gameObject);
            AudioSource.PlayClipAtPoint(_explosionAudio, new Vector3(0, 0, -10), 1.0f);
            Instantiate(_explosion, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}

