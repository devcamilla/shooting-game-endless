using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class GameState : ScriptableObject
{
    private readonly List<Wave> _waves = new List<Wave>
    {
        new Wave(new Squad("LeftCurve", "RS", 3), new Squad("RightCurve", "RS", 3)),
        new Wave(new Squad("TopLeft", "YS", 1), new Squad("TopCenter", "RS", 3), new Squad("TopRight", "YS", 1)),
        new Wave(new Squad("LeftUp", "BS", 3), new Squad("LeftDown", "RS", 3)),
        new Wave(new Squad("TopLeft", "RS", 3), new Squad("TopCenter", "YS", 1), new Squad("TopRight", "RS", 3)),
        new Wave(new Squad("RightUp", "BS", 3), new Squad("RightDown", "RS", 3)),
        new Wave(new Squad("TopLeft", "YS", 1), new Squad("TopCenter", "YS", 1), new Squad("TopRight", "YS", 1), new Squad("LeftDown", "RS", 3), new Squad("RightDown", "RS", 3)),
        new Wave(new Squad("TopCenter", "RB", 1)),

        new Wave(new Squad("LeftUp", "RM", 3), new Squad("LeftDown", "RM", 3), new Squad("RightCurve", "RS", 3)),
        new Wave(new Squad("TopLeft", "YS", 1), new Squad("TopCenter", "YM", 1), new Squad("TopRight", "YS", 1)),
        new Wave(new Squad("RightUp", "BM", 3), new Squad("RightDown", "RM", 3), new Squad("TopLeft", "RS", 3)),
        new Wave(new Squad("LeftUp", "BM", 3), new Squad("LeftDown", "RM", 3), new Squad("TopRight", "RS", 3)),
        new Wave(new Squad("LeftCurve", "RM", 3), new Squad("TopCenter", "YM", 1), new Squad("RightCurve", "RM", 3)),
        new Wave(new Squad("TopLeft", "YM", 1), new Squad("TopRight", "YM", 1), new Squad("RightDown", "RM", 5)),
        new Wave(new Squad("TopCenter", "YB", 1)),

        new Wave(new Squad("TopCenter", "RM", 5), new Squad("TopRight", "RM", 5), new Squad("TopLeft", "YM", 1)),
        new Wave(new Squad("LeftUp", "BM", 3), new Squad("LeftDown", "RM", 5)),
        new Wave(new Squad("TopLeft", "YM", 1), new Squad("TopCenter", "YM", 1), new Squad("RightDown", "RS", 5)),
        new Wave(new Squad("LeftCurve", "RM", 3), new Squad("RightCurve", "RM", 3)),
        new Wave(new Squad("TopLeft", "RM", 5), new Squad("TopCenter", "RM", 5), new Squad("TopRight", "YM", 1)),
        new Wave(new Squad("LeftUp", "BM", 1),  new Squad("TopCenter", "RM", 5), new Squad("RightUp", "BM", 1)),
        new Wave(new Squad("TopCenter", "BB", 1)),
    };

    [Header("Enemy Settings")]
    public float waveInterval;

    public float spawnInterval;

    [Header("State")]
    public int highScore;

    public bool playing;

    public bool dead;

    public int score;

    public int currShip;

    public int currWave;

    public int currLevel;

    public int currDifficulty;

    public bool bossWave;

    public int defeatedBoss;

    [Header("Prefabs")]
    public GameObject player;

    public GameObject enemy;

    public GameObject bullet;

    public GameObject enemyBullet;

    public GameObject powerUp;

    public GameObject explosion;

    public List<Flank> flanks = new List<Flank>();

    public List<EnemyShip> enemyShips = new List<EnemyShip>();

    public List<PlayerShip> playerShips = new List<PlayerShip>();

    private void OnEnable()
    {
        Reset();
    }

    public void Reset()
    {
        playing = false;
        dead = false;
        score = 0;
        currShip = -1;
        currWave = 0;
        currDifficulty = 1;
        currLevel = 1;
        bossWave = false;
        defeatedBoss = 0;
    }

    public List<Enemies> NextWave()
    {
        if (currWave > _waves.Count - 1)
        {
            currDifficulty++;
            currWave = 0;
        }

        var wave = _waves[currWave];
        currWave++;
        bossWave = false;

        return wave.squads
            .Select(x =>
            {
                var flank = flanks.SingleOrDefault(z => z.code == x.flankCode);
                var enemyShip = enemyShips.SingleOrDefault(z => z.code == x.enemyCode);

                if (enemyShip.boss)
                    bossWave = true;

                return new Enemies(
                    flank.spawnPoint,
                    Quaternion.Euler(0, 0, flank.spawnRotationZ),
                    flank.curve,
                    enemyShip,
                    x.count * currDifficulty);
            })
            .ToList();
    }

    public PlayerShip NextShip()
    {
        currShip = Math.Min(currShip + 1, playerShips.Count - 1);
        return playerShips[currShip];
    }

    public PlayerShip PreviousShip()
    {
        currShip = Math.Max(currShip - 1, 0);
        return playerShips[currShip];
    }

    public void DefeatBoss()
    {
        defeatedBoss++;
        if (defeatedBoss == currDifficulty)
        {
            bossWave = false;
            currLevel++;
            defeatedBoss = 0;
        }
    }

    public void Score(int points)
    {
        score += points;
        highScore = Math.Max(highScore, score);
    }

    [Serializable]
    public class EnemyShip
    {
        public string code;

        public Texture texture;

        public float speed;

        public int life;

        public bool boss;

        public List<Bullet> bullets = new List<Bullet>();
    }

    [Serializable]
    public class Flank
    {
        public string code;

        public Vector3 spawnPoint;

        public float spawnRotationZ;

        public AnimationCurve curve;
    }

    [Serializable]
    public class Bullet
    {
        public Texture texture;

        public float speed;

        public float accuracy;

        public float interval;

        public bool spinning;

        public AudioClip audioClip;
    }

    [Serializable]
    public class PlayerShip
    {
        public string code;

        public Texture texture;

        public float speed;

        public List<Bullet> bullets = new List<Bullet>();
    }

    private class Squad
    {
        public string flankCode;

        public string enemyCode;

        public int count;

        public Squad(string flankCode, string enemyCode, int count)
        {
            this.flankCode = flankCode;
            this.enemyCode = enemyCode;
            this.count = count;
        }
    }

    private class Wave
    {
        public List<Squad> squads = new List<Squad>();

        public Wave(params Squad[] squads)
        {
            this.squads = squads.ToList();
        }
    }

    public class Enemies
    {
        public Vector3 spawnPoint;

        public Quaternion spawnRotation;

        public AnimationCurve movement;

        public EnemyShip enemyShip;

        public int count;

        public Enemies(Vector3 spawnPoint, Quaternion spawnRotation, AnimationCurve movement, EnemyShip enemyShip, int count)
        {
            this.spawnPoint = spawnPoint;
            this.spawnRotation = spawnRotation;
            this.movement = movement;
            this.enemyShip = enemyShip;
            this.count = count;
        }
    }
}
