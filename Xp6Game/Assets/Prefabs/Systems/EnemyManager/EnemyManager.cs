using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;


public class EnemyManager : MonoBehaviour
{
    [Header("Enemy Pool")]
    [SerializeField] private int maxEnemies = 50;
    [SerializeField] List<GameObject> enemyPool;

    [SerializeField] private GameObject enemyPrefab;

    [SerializeField] private EnemySpawner _enemySpawner;

    Transform _enemyHolder;

    public async UniTask Initialize()
    {
        _enemyHolder = new GameObject().transform;
        _enemyHolder.SetParent(transform);
        _enemyHolder.transform.name = "EnemyHolder";

        await SpawnEnemyPool();

        if (TryGetComponent<EnemySpawner>(out EnemySpawner comp))
        {
            _enemySpawner = comp;
        }
        else
        {
            _enemySpawner = gameObject.AddComponent<EnemySpawner>();
        }
    }

    async UniTask SpawnEnemyPool()
    {
        for (int i = 0; i < maxEnemies; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab, _enemyHolder);
            enemyPool.Add(enemy);
            enemy.SetActive(false);
        }
        await UniTask.CompletedTask;
    }
    public GameObject GetEnemy()
    {
        foreach (GameObject enemy in enemyPool)
        {
            if (!enemy.activeSelf)
            {
                return enemy;
            }
        }
        return null;
    }
    public void StartSpawning()
    {
        Debug.Log("Enemy Manager Start Spawning");
        _enemySpawner.StartSpawning();
    }
    public void StopSpawning()
    {
        _enemySpawner.StopSpawning();
    }
    public EnemySpawner GetSpawner()
    {
        return _enemySpawner;
    }



}
