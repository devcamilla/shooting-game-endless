using System.Collections;
using UnityEngine;
using static GameState;

public class EnemyShipController : MonoBehaviour
{
    private const int ENEMY_POINTS = 100;

    private const int BOSS_POS_Y = 3;

    private const float BOSS_MOVE_TIME = 15;

    private Transform _point;

    private EnemyShip _enemyShip;

    private AnimationCurve _movement;

    private int _hit;

    private float _targetPosX;

    private float _elapsedBossMoveTime;

    public GameState gameState;

    public float smoothing;

    public float lifetime;

    private void Start()
    {
        _point = transform.Find("Point").GetComponent<Transform>();
        _targetPosX = Random.value > .5f ? -3 : 3;
    }

    private void Update()
    {
        var targetRotZ = _movement.Evaluate(_enemyShip.speed * Time.deltaTime);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.Euler(0, 0, targetRotZ),
            _enemyShip.speed * Time.deltaTime);

        var boss = _enemyShip.boss;
        var targetPos = _point.position;

        if (boss)
        {
            if (targetPos.y < BOSS_POS_Y)
            {
                var x = _targetPosX;
                if (_elapsedBossMoveTime > BOSS_MOVE_TIME)
                {
                    _targetPosX = -_targetPosX;
                    _elapsedBossMoveTime = 0;
                }
                targetPos = new Vector3(x, BOSS_POS_Y);
            }
            _elapsedBossMoveTime += Time.deltaTime;
        }
        else
        {
            lifetime -= Time.deltaTime;
            if (lifetime < 0)
                Destroy(gameObject);
        }

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            _enemyShip.speed * Time.deltaTime);
    }

    public void SetEnemy(Enemies enemies)
    {
        _enemyShip = enemies.enemyShip;
        _movement = enemies.movement;

        transform.position = enemies.spawnPoint;
        transform.rotation = enemies.spawnRotation;

        var renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = enemies.enemyShip.texture;

        foreach (var bullet in enemies.enemyShip.bullets)
            StartCoroutine(Shoot(bullet));
    }

    private IEnumerator Shoot(Bullet bullet)
    {
        while (true)
        {
            var newBullet = Instantiate(gameState.enemyBullet, transform.parent);
            newBullet.transform.position = transform.position + Vector3.down;

            var bulletController = newBullet.GetComponent<BulletController>();
            bulletController.SetBullet(bullet);

            yield return new WaitForSeconds(bullet.interval);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var newExplosion = Instantiate(gameState.explosion, transform.parent);
            newExplosion.transform.position = transform.position;

            _hit++;
            var life = _enemyShip.life;
            if (_hit == life)
            {
                gameState.Score(ENEMY_POINTS * life);
                if (_enemyShip.boss)
                {
                    gameState.DefeatBoss();

                    var newPowerUp = Instantiate(gameState.powerUp, transform.parent);
                    newPowerUp.transform.position = transform.position;
                }

                Destroy(gameObject);
            }

            if (other.TryGetComponent<BulletController>(out _))
                Destroy(other.gameObject);
        }
    }
}
