using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] bool canSpawn = false;
    [SerializeField] float spawnInterval = 5f;
    float timer = 0;

    System.Random _random;


    [Header("Enemy Data")]
    public EnemySO[] RangedData;
    public EnemySO[] MeleeData;

    [Header("Positions")]
    public GameObject[] enemySpawnPosition;

    private EnemyManager _enemyManager;

    private Queue<GameObject> m_enemiesToSpawn = new Queue<GameObject>();


    void Start()
    {
        _enemyManager = GameManager.Instance.GetEnemyManager();
        enemySpawnPosition = GameObject.FindGameObjectsWithTag("EnemySpawnPos");

        if (enemySpawnPosition.Length == 0)
        {
            Debug.LogError("No enemy spawn positions found.");
        }

        _random = new System.Random();


    }
    void FixedUpdate()
    {
        if (!canSpawn) return;


        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0;
            // SpawnRandomEnemyAt(GetRandomSpawnPosition());
            SpawnRandomRangedEnemy(GetRandomSpawnPosition());
        }
    }
    public void StartSpawning()
    {
        canSpawn = true;
    }

    public void StopSpawning()
    {
        canSpawn = false;
    }
    private void SpawnRandomEnemyAt(Vector3 SpawnPosition)
    {
        // GameObject enemy = _enemyManager.GetEnemy();

        // if (enemy == null) return;

        // is melee or ranged
        int randomIndex = _random.Next(10);


        if (randomIndex < 5)
        {
            SpawnMeleeEnemy(SpawnPosition);
        }
        else
        {
            SpawnRandomRangedEnemy(SpawnPosition);
        }
    }

    private void SpawnMeleeEnemy(Vector3 spawnPosition)
    {

    }
    private void SpawnRandomRangedEnemy(Vector3 spawnPosition)
    {
        // EnemySO _enemyData = GetRandomRangedData();

        GameObject enemy = _enemyManager.GetRangedEnemy();
        if (enemy == null) return;
        enemy.transform.position = spawnPosition;
        enemy.transform.rotation = Quaternion.identity;

        enemy.SetActive(true);

        // enemy.GetComponent<RangedEnemy>().SetData(_enemyData);
        enemy.GetComponent<RangedEnemy>().Initialize();

    }

    private Vector3 GetRandomSpawnPosition()
    {
        int randomIndex = _random.Next(enemySpawnPosition.Length);
        return enemySpawnPosition[randomIndex].transform.position;
    }

    private EnemySO GetRandomRangedData()
    {
        int randomIndex = _random.Next(RangedData.Length);
        // Debug.Log(randomIndex);
        return RangedData[randomIndex];
    }


    public async void SetNewWave(WaveData data)
    {

        Debug.Log("Starting New Wave");
        for (int i = 0; i < data.m_enemies.Length; i++)
        {
            foreach (var d in data.m_enemies)
            {
                for (int j = 0; j < d.amount; j++)
                    m_enemiesToSpawn.Enqueue(d.m_prefab);

                // Instantiate(d.m_prefab);
            }
        }
        spawnInterval = data.m_spawnRate;


        await SpawnNextInQueue();
    }
    public async UniTask SpawnNextInQueue()
    {
        if (m_enemiesToSpawn.Count == 0) await UniTask.CompletedTask;


        await UniTask.Delay(TimeSpan.FromSeconds(1f * spawnInterval));
        Debug.Log("Spawning Enemy from Queue");
        GameObject enemyPrefab = m_enemiesToSpawn.Dequeue();
        Vector3 spawnPosition = GetRandomSpawnPosition();

        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        enemy.SetActive(true);
        enemy.GetComponent<Enemy<EnemySO>>().Initialize();
        if (m_enemiesToSpawn.Count != 0)
            await SpawnNextInQueue();


        await UniTask.CompletedTask;
    }


}
