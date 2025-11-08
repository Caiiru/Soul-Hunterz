using System;
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




}
[Serializable]
public struct SpawnerInfo
{
    public int rangedAmount;
    public int meleeAmount;
    public int bossAmount;

}
