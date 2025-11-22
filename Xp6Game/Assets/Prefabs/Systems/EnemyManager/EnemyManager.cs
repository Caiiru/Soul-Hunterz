using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FMODUnity;
using UnityEngine;


public class EnemyManager : MonoBehaviour

{

    public WaveData[] m_Waves;
    public WaveData m_FinalWave;


    [Header("Enemy Pool")]
    [SerializeField] List<GameObject> rangedPool;

    private EnemySpawner _enemySpawner;

    Transform _enemyHolder;

    public int m_currentWave = 0;


    //Events

    EventBinding<OnGameWin> m_OnGameWinEventBinding;
    EventBinding<OnGameOver> m_OnGameOverBinding;
    EventBinding<OnEnemyDied> m_OnEnemyDiedBinding;
    EventBinding<OnFinalAltarActivated> m_OnFinalAltarActivatedBinding;

    EventBinding<OnMapCollected> m_OnTutorialFinished;

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

        m_OnTutorialFinished = new EventBinding<OnMapCollected>(() =>
        {
            _enemySpawner.StartSpawning();
            StartNextWave();
        });
        EventBus<OnMapCollected>.Register(m_OnTutorialFinished);

        EventBus<OnAltarActivated>.Register(new EventBinding<OnAltarActivated>((OnAltarActivated data) =>
        {
            if (data.m_AltarActivatedIndex == 0)
            {
                //Start First Wave 

            }
        }));

        m_OnFinalAltarActivatedBinding = new EventBinding<OnFinalAltarActivated>(() =>
        {
            // _enemySpawner.SetNewWave(m_FinalWave);
            _enemySpawner.StartSpawning();
        });
        EventBus<OnFinalAltarActivated>.Register(m_OnFinalAltarActivatedBinding);


        // m_OnEnemyDiedBinding = new EventBinding<OnEnemyDied>(OnEnemyDiedHandler);
        // EventBus<OnEnemyDied>.Register(m_OnEnemyDiedBinding);


    }

    private void OnEnemyDiedHandler(OnEnemyDied arg0)
    {
        if (_enemySpawner.GetActiveEnemies() <= 7)
        {
            Debug.Log("NEXT WAVE");
            EventBus<WaveEndEvent>.Raise(new WaveEndEvent());
            StartNextWave();
        }
    }

    async void EndGame()
    {
        StopSpawning();
        await UnbindEvents();
        Destroy(this.gameObject);
    }

    private UniTask UnbindEvents()
    {
        EventBus<OnGameOver>.Unregister(m_OnGameOverBinding);
        EventBus<OnGameWin>.Unregister(m_OnGameWinEventBinding);

        return UniTask.CompletedTask;
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
        return;


        // _enemySpawner.SetNewWave(m_Waves[m_currentWave]);
        // EventBus<WaveStartEvent>.Raise(new WaveStartEvent { waveIndex = m_currentWave });
        // m_currentWave = m_Waves.Length < m_currentWave - 1 ? m_currentWave++ : m_currentWave;

        // if (AudioManager.Instance == null) return;
        // AudioManager.Instance.PlayOneShotAtPosition(m_waveStartClip, Camera.main.transform.position);

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

