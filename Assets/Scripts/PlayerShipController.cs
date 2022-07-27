using System.Collections;
using System.Linq;
using UnityEngine;
using static GameState;

public class PlayerShipController : MonoBehaviour
{
    private const int POWER_UP_POINTS = 1000;

    private Vector2 _screenBounds;

    private float _speed;

    private Bullet _bullet;

    private int _life;

    private bool _autoShoot;

    private AudioSource _audioSource;

    public GameState gameState;

    public AudioClip powerUp;

    public AudioClip downGrade;

    private void Start()
    {
        _screenBounds = Camera.main.ScreenToWorldPoint(
            new Vector3(
                Screen.width,
                Screen.height,
                Camera.main.transform.position.z));

        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        var direction = new Vector3();
        if (Input.GetKey(KeyCode.LeftArrow) && transform.position.x > -_screenBounds.x)
        {
            direction = new Vector3(-1, 0, 0);
        }

        if (Input.GetKey(KeyCode.RightArrow) && transform.position.x < _screenBounds.x)
        {
            direction = new Vector3(1, 0, 0);
        }

        if (Input.GetKey(KeyCode.DownArrow) && transform.position.y > -_screenBounds.y)
        {
            direction = new Vector3(0, -1, 0);
        }

        if (Input.GetKey(KeyCode.UpArrow) && transform.position.y < _screenBounds.y)
        {
            direction = new Vector3(0, 1, 0);
        }

        transform.position += _speed * Time.deltaTime * direction;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            _autoShoot = false;
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.A) && !_autoShoot)
            StartCoroutine(AutoShoot());
    }

    public void SetShip(PlayerShip playerShip)
    {
        _speed = playerShip.speed;
        _bullet = playerShip.bullets.First();

        var renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = playerShip.texture;
    }

    private void Shoot()
    {
        var newBullet = Instantiate(gameState.bullet, transform.parent);
        newBullet.transform.position = transform.position + Vector3.up;

        var bulletController = newBullet.GetComponent<BulletController>();
        bulletController.SetBullet(_bullet);
    }

    private IEnumerator AutoShoot()
    {
        _autoShoot = true;
        while (_autoShoot)
        {
            Shoot();
            yield return new WaitForSeconds(_bullet.interval);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            _life--;
            if (_life < 0)
            {
                gameState.dead = true;
            }
            else
            {
                _audioSource.clip = downGrade;
                _audioSource.Play();

                var prevShip = gameState.PreviousShip();
                SetShip(prevShip);
            }
        }
        else if (other.gameObject.CompareTag("PowerUp"))
        {
            _audioSource.clip = powerUp;
            _audioSource.Play();

            _life++;

            var nextShip = gameState.NextShip();
            SetShip(nextShip);

            gameState.Score(POWER_UP_POINTS);
            Destroy(other.gameObject);
        }
    }
}
