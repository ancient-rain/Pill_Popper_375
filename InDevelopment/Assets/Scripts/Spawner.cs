using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    public bool devMode;
    public Wave[] waves;
    public Enemy enemy;
    public Powerup powerup;
    int enemiesRemainingToSpawn;
    float timeNextSpawn;
    float campingCheck = 2;
    float nextCheckTime;
    float campThresh = 1.5f;
    float powerUpThreshHold = 98;
    bool camping;
    bool disable;
    Vector3 CampPostionOld;
    Wave currentWave;
    int waveNumber;
    int enemiesRemaining;
    MapGenerator map;
    LivingEntity player;
    Transform playerT;

    public event System.Action<int> onNewWave;

    void Start()
    {
        camping = false;
        disable = false;
        map = FindObjectOfType<MapGenerator>();
        player = FindObjectOfType<Player>();
        playerT = player.transform;
        nextCheckTime = campingCheck + Time.time;
        CampPostionOld = playerT.position;
        player.onDeath += onPlayerDeath;
        nextWave();
    }

    void Update()
    {
        if (!disable)
        {
            if (Time.time > nextCheckTime)
            {
                nextCheckTime = Time.time + campingCheck;
                camping = (Vector3.Distance(playerT.position, CampPostionOld) < campThresh);
                CampPostionOld = playerT.position;
            }

            if(currentWave.infinite && Time.time > timeNextSpawn)
            {
                enemiesRemainingToSpawn--;
                timeNextSpawn = Time.time + currentWave.timeBetweenSpawns;
                StartCoroutine("spawnEnemy");
                if (currentWave.timeBetweenSpawns > 0.25)
                {
                    currentWave.timeBetweenSpawns -= 0.005f;
                }
            }

            if ((enemiesRemainingToSpawn > 0)&& Time.time > timeNextSpawn)
            {
                enemiesRemainingToSpawn--;
                timeNextSpawn = Time.time + currentWave.timeBetweenSpawns;
                StartCoroutine("spawnEnemy");
            }

            float randomForPowerup = Random.Range(0, 100);
            if(randomForPowerup > this.powerUpThreshHold)
            {
                StartCoroutine("spawnPoewerup");
            }
        }

        if (devMode)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                StopCoroutine("spawnEnemy");
                foreach(Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    GameObject.Destroy(enemy.gameObject);
                }
                nextWave();
            }
        }
    }

    void resetPlayer()
    {
        playerT.position = map.getTileFromPostion(Vector3.zero).position + Vector3.up * 3;
    }

    IEnumerator spawnEnemy()
    {
        float spawnDelay = 1;
        float flashSpeed = 4;
        Transform randTile = map.getRandomOpenTile();
        if (camping)
        {
            randTile = map.getTileFromPostion(playerT.position); 
        }
        Material tileMat = randTile.GetComponent<Renderer>().material;
        Color initColor = Color.white;
        Color flash = Color.red;
        float spawnTimer = 0;

        while(spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(initColor, flash, Mathf.PingPong(spawnTimer * flashSpeed, 1));
            spawnTimer += Time.deltaTime;
            yield return null;
        }
        Enemy spawnedEnemy = (Enemy)Instantiate(enemy, randTile.position + Vector3.up, Quaternion.identity);
        spawnedEnemy.onDeath += onEnemyDeath;
        spawnedEnemy.setCharacteristics(currentWave.moveSpeed, currentWave.hitsToKillPlayer, currentWave.enemyHealth, currentWave.skinColor);
    }

    IEnumerator spawnPoewerup()
    {
        float spawnDelay = 1;
        float flashSpeed = 4;
        Transform randTile = map.getRandomOpenTile();

        Material tileMat = randTile.GetComponent<Renderer>().material;
        Color initColor = Color.white;
        Color flash = Color.green;
        float spawnTimer = 0;

        while (spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(initColor, flash, Mathf.PingPong(spawnTimer * flashSpeed, 1));
            spawnTimer += Time.deltaTime;
            yield return null;
        }
        Powerup spawnedPowerup = (Powerup)Instantiate(powerup, randTile.position + Vector3.up, Quaternion.identity);
    }

    void onEnemyDeath()
    {
        enemiesRemaining--;

        if(enemiesRemaining == 0)
        {
            nextWave();
        }
    }

    void onPlayerDeath()
    {
        Cursor.visible = true;
        disable = true;
    }

    void nextWave()
    {
        if(waveNumber > 0)
        {
            AudioManager.instance.playSound2D("Level Complete");
        }

        waveNumber++;
        PlayerPrefs.SetInt("Wave", waveNumber);

        if(waveNumber - 1 < waves.Length)
        {
            currentWave = waves[waveNumber - 1];
            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemaining = enemiesRemainingToSpawn;
            player.startingHealth = 5;

            if(onNewWave != null)
            {
                onNewWave(waveNumber);
            }
        }
        resetPlayer();
    }

    [System.Serializable]
    public class Wave {
        public bool infinite;
        public int enemyCount;
        public int hitsToKillPlayer;
        public float timeBetweenSpawns;
        public float moveSpeed;
        public float enemyHealth;
        public Color skinColor;
    }

}
