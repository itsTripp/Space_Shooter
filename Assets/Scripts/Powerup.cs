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

    // Start is called before the first frame update
    void Start()
    {
             
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _powerupSpeed * Time.deltaTime);
        if (transform.position.y < -5f)
        {
            Destroy(this.gameObject);
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
                }
                Destroy(this.gameObject);
            }
        }
    }
}

