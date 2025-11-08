using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;


public class EnemyManager : MonoBehaviour

{
    public EnemyData[] m_EnemiesToSpawn;
    [Header("Enemy Pool")]
    [SerializeField] private int maxEnemies = 25;
    [SerializeField] List<GameObject> rangedPool;

    private EnemySpawner _enemySpawner;

    Transform _enemyHolder;

    //Events

    EventBinding<GameWinEvent> m_OnGameWinEventBinding;
    EventBinding<GameOverEvent> m_OnGameOverBinding;

    public async UniTask Initialize()
    {
        if (_enemyHolder == null)
        {
            _enemyHolder = new GameObject().transform;
            _enemyHolder.SetParent(transform);
            _enemyHolder.transform.name = "EnemyHolder";
        }

        await SpawnRangedEnemyPool();

        if (TryGetComponent<EnemySpawner>(out EnemySpawner comp))
        {
            _enemySpawner = comp;
        }
        else
        {
            _enemySpawner = gameObject.AddComponent<EnemySpawner>();
        }

        BindEvents();
        await UniTask.CompletedTask;
    }

    private void BindEvents()
    {
        m_OnGameOverBinding = new EventBinding<GameOverEvent>(() =>
        {
            EndGame();
        });
        EventBus<GameOverEvent>.Register(m_OnGameOverBinding);

        m_OnGameWinEventBinding = new EventBinding<GameWinEvent>(() =>
        {
            EndGame();
        });
        EventBus<GameWinEvent>.Register(m_OnGameWinEventBinding);

    }

    void EndGame()
    {
        StopSpawning();
        UnbindEvents();
        Destroy(this.gameObject);
    }

    private void UnbindEvents()
    {
        EventBus<GameOverEvent>.Unregister(m_OnGameOverBinding);
        EventBus<GameWinEvent>.Unregister(m_OnGameWinEventBinding);
    }

    void OnDestroy()
    {
        UnbindEvents();
    }
    async UniTask SpawnRangedEnemyPool()
    {
        int m_enemyDataIndex = 0;
        for (int i = 0; i < maxEnemies; i++)
        {
            GameObject enemy = Instantiate(m_EnemiesToSpawn[0].prefabs[m_enemyDataIndex], _enemyHolder);
            rangedPool.Add(enemy);
            enemy.SetActive(false);

            m_enemyDataIndex++;
            if (m_enemyDataIndex >= m_EnemiesToSpawn[0].prefabs.Length)
            {
                m_enemyDataIndex = 0;
            }
        }
        await UniTask.CompletedTask;
    }
    async UniTask SpawnMeeleEnemyPool()
    {
        int m_enemyDataIndex = 0;
        for (int i = 0; i < maxEnemies; i++)
        {
            GameObject enemy = Instantiate(m_EnemiesToSpawn[1].prefabs[m_enemyDataIndex], _enemyHolder);
            rangedPool.Add(enemy);
            enemy.SetActive(false);

            m_enemyDataIndex++;
            if (m_enemyDataIndex >= m_EnemiesToSpawn[0].prefabs.Length)
            {
                m_enemyDataIndex = 0;
            }   
        }
        await UniTask.CompletedTask;
    }
    public GameObject GetRangedEnemy()
    {
        foreach (GameObject enemy in rangedPool)
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

[Serializable]
public struct EnemyData
{
    public string name;
    public GameObject[] prefabs;
}

