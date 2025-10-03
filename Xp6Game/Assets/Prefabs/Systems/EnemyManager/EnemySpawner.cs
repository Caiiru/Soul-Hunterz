using System; 
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] bool canSpawn = false; 
    [SerializeField] float spawnInterval = 5f;
    float timer = 0;

    System.Random _random;


    [Header("Enemy Data")]
    
    [Header("Positions")]
    public GameObject[] enemySpawnPosition;

    private EnemyManager _enemyManager;
 

    void Start()
    {
        _enemyManager = GameManager.Instance.GetEnemyManager;
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
            SpawnEnemy();
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
    private void SpawnEnemy()
    {
        GameObject enemy = _enemyManager.GetEnemy();

        if (enemy == null) return;
        
        int randomIndex = _random.Next(enemySpawnPosition.Length);
        enemy.transform.position = enemySpawnPosition[randomIndex].transform.position;
        enemy.SetActive(true); 

    } 


    }
