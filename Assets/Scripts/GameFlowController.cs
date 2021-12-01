using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static GameState;

public class GameFlowController : MonoBehaviour
{
    private float _waveElapsedTime;

    public GameState gameState;

    public GameObject titleObj;

    public GameObject stageObj;

    public GameObject controlsObj;

    public GameObject gameOver;

    public Text score;

    public Text highScore;

    public Text level;

    private void Start()
    {
        titleObj.SetActive(true);
        controlsObj.SetActive(true);
        gameOver.SetActive(false);
        level.enabled = false;

        SpawnPlayer();
    }

    private void Update()
    {
        if (gameState.playing)
        {
            level.text = $"LEVEL {gameState.currLevel}";

            if (gameState.dead || Input.GetKeyDown(KeyCode.Escape))
            {
                GameOver();
            }

            if (!gameState.bossWave)
            {
                if (_waveElapsedTime < 0)
                    SpawnWave();

                _waveElapsedTime -= Time.deltaTime;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            gameState.playing = true;
            titleObj.SetActive(false);
            controlsObj.SetActive(false);
            gameOver.SetActive(false);
            level.enabled = true;

            SpawnWave();
        }

        score.text = $"SCORE {gameState.score:00000}";
        highScore.text = $"HIGH SCORE {gameState.highScore:00000}";
    }

    private void SpawnPlayer()
    {
        var newPlayer = Instantiate(gameState.player, stageObj.transform);
        newPlayer.transform.position = new Vector3(0, -4);

        var playerController = newPlayer.GetComponent<PlayerShipController>();
        playerController.SetShip(gameState.NextShip());
    }

    private void SpawnWave()
    {
        _waveElapsedTime = gameState.waveInterval;

        foreach (var enemies in gameState.NextWave())
            StartCoroutine(SpawnEnemy(enemies));
    }

    private IEnumerator SpawnEnemy(Enemies enemies)
    {
        for (var x = 0; x < enemies.count; x++)
        {
            if (!gameState.playing) break;

            var newEnemy = Instantiate(gameState.enemy, stageObj.transform);
            var enemyController = newEnemy.GetComponent<EnemyShipController>();
            enemyController.SetEnemy(enemies);

            yield return new WaitForSeconds(gameState.spawnInterval);
        }
    }

    private void GameOver()
    {
        gameState.Reset();
        titleObj.SetActive(true);
        controlsObj.SetActive(true);
        gameOver.SetActive(true);
        level.enabled = false;

        for (var x = 0; x < stageObj.transform.childCount; x++)
            Destroy(stageObj.transform.GetChild(x).gameObject);

        SpawnPlayer();
    }
}
