using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FMODUnity;
using UnityEngine;


public class EnemyManager : MonoBehaviour

{

    public WaveData[] m_Waves;


    [Header("Enemy Pool")]
    [SerializeField] private int maxEnemies = 25;
    [SerializeField] List<GameObject> rangedPool;

    private EnemySpawner _enemySpawner;

    Transform _enemyHolder;

    public int m_currentWave = 0;

    //Events

    EventBinding<OnGameWin> m_OnGameWinEventBinding;
    EventBinding<OnGameOver> m_OnGameOverBinding;

    [Header("Audio")]

    public EventReference m_waveStartClip;

    public async UniTask Initialize()
    {
        if (_enemyHolder == null)
        {
            _enemyHolder = new GameObject().transform;
            _enemyHolder.SetParent(transform);
            _enemyHolder.transform.name = "EnemyHolder";
        }

        // await SpawnRangedEnemyPool();

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
        m_OnGameOverBinding = new EventBinding<OnGameOver>(() =>
        {
            EndGame();
        });
        EventBus<OnGameOver>.Register(m_OnGameOverBinding);

        m_OnGameWinEventBinding = new EventBinding<OnGameWin>(() =>
        {
            EndGame();
        });
        EventBus<OnGameWin>.Register(m_OnGameWinEventBinding);

        EventBus<OnAltarActivated>.Register(new EventBinding<OnAltarActivated>(StartNextWave));




    }

    void EndGame()
    {
        StopSpawning();
        UnbindEvents();
        // Destroy(this.gameObject);
    }

    private void UnbindEvents()
    {
        EventBus<OnGameOver>.Unregister(m_OnGameOverBinding);
        EventBus<OnGameWin>.Unregister(m_OnGameWinEventBinding);
    }

    void OnDestroy()
    {
        UnbindEvents();
    }
    // async UniTask SpawnRangedEnemyPool()
    // {
    //     int m_enemyDataIndex = 0;
    //     for (int i = 0; i < maxEnemies; i++)
    //     {
    //         GameObject enemy = Instantiate(m_EnemiesToSpawn[0].prefabs[m_enemyDataIndex], _enemyHolder);
    //         rangedPool.Add(enemy);
    //         enemy.SetActive(false);

    //         m_enemyDataIndex++;
    //         if (m_enemyDataIndex >= m_EnemiesToSpawn[0].prefabs.Length)
    //         {
    //             m_enemyDataIndex = 0;
    //         }
    //     }
    //     await UniTask.CompletedTask;
    // }
    // async UniTask SpawnMeeleEnemyPool()
    // {
    //     int m_enemyDataIndex = 0;
    //     for (int i = 0; i < maxEnemies; i++)
    //     {
    //         GameObject enemy = Instantiate(m_EnemiesToSpawn[1].m_prefab[m_enemyDataIndex], _enemyHolder);
    //         rangedPool.Add(enemy);
    //         enemy.SetActive(false);

    //         m_enemyDataIndex++;
    //         if (m_enemyDataIndex >= m_EnemiesToSpawn[0].m_prefab.Length)
    //         {
    //             m_enemyDataIndex = 0;
    //         }
    //     }
    //     await UniTask.CompletedTask;
    // }
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
    public void StartNextWave()
    {
        // _enemySpawner.StartSpawning();
        _enemySpawner.SetNewWave(m_Waves[m_currentWave]);
        EventBus<WaveStartEvent>.Raise(new WaveStartEvent { waveIndex = m_currentWave });
        m_currentWave++;

        if (AudioManager.Instance == null) return;
        AudioManager.Instance.PlayOneShotAtPosition(m_waveStartClip, Camera.main.transform.position);

    }
    public void StopSpawning()
    {
        _enemySpawner.StopSpawning();
    }
    public EnemySpawner GetSpawner()
    {
        return _enemySpawner;
    }

    public int GetCurrentWave()
    {
        return m_currentWave;
    }
    public void SetCurrentWave(int val)
    {
        m_currentWave = val;
    }


}

[Serializable]
public struct EnemyData
{
    public string name;
    public int amount;
    public GameObject m_prefab;
}

[Serializable]
public struct WaveData
{
    public int waveNumber;
    public float m_spawnRate;
    public EnemyData[] m_enemies;
}

