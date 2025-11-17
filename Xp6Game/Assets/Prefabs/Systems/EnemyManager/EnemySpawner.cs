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

    [SerializeField] int m_EnemiesActive = 0;


    [Header("Positions")]
    public GameObject[] enemySpawnPosition;

    private EnemyManager _enemyManager;

    [SerializeField] private Queue<GameObject> m_enemiesToSpawn = new Queue<GameObject>();


    //Events
    EventBinding<OnEnemyDied> m_OnEnemyDiedBinding;


    void Start()
    {
        _enemyManager = GameManager.Instance.GetEnemyManager();
        enemySpawnPosition = GameObject.FindGameObjectsWithTag("EnemySpawnPos");

        if (enemySpawnPosition.Length == 0)
        {
            Debug.LogError("No enemy spawn positions found.");
        }

        _random = new System.Random();

        BindEvents();
    }

    private void BindEvents()
    {
        m_OnEnemyDiedBinding = new EventBinding<OnEnemyDied>(OnEnemyDiedHandler);

    }

    private void OnEnemyDiedHandler(OnEnemyDied arg0)
    {
        m_EnemiesActive--;
    }

    async void FixedUpdate()
    {
        if (!canSpawn) return;


        timer += Time.deltaTime;
        if (timer >= spawnInterval && m_enemiesToSpawn.Count > 0)
        {
            await SpawnNextInQueue();
            timer = 0;
            // SpawnRandomEnemyAt(GetRandomSpawnPosition()); 
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

        //Spread out the enemies
        Vector3 randomOffset = new Vector3(_random.Next(-5, 5), 0, _random.Next(-5, 5));
        return enemySpawnPosition[randomIndex].transform.position + randomOffset;


        // return enemySpawnPosition[randomIndex].transform.position;
    }



    public async void SetNewWave(WaveData data)
    {

        // EventBus<WaveStartEvent>.Raise(new WaveStartEvent { waveIndex = m_currentWave - 1 });
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
        if (m_enemiesToSpawn.Count == 0)
        {

            // EventBus<WaveEndEvent>.Raise(new WaveEndEvent());
            await UniTask.CompletedTask;
        }
        if (!canSpawn) await UniTask.CompletedTask;

        GameObject enemyPrefab = m_enemiesToSpawn.Dequeue();
        Vector3 spawnPosition = GetRandomSpawnPosition();

        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        m_EnemiesActive++;
        enemy.SetActive(true);
        // enemy.GetComponent<Enemy<EnemySO>>().Initialize();

        await UniTask.CompletedTask;
    }



    public int GetActiveEnemies()
    {
        return m_EnemiesActive;
    }



}
