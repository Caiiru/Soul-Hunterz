using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;


public class EnemyManager : MonoBehaviour

{
    [Header("Enemy Empty Prefab")]
    [SerializeField] private GameObject EmptyEnemyRanged;
    [SerializeField] private GameObject EmptyEnemyMelee;

    [Header("Enemy Pool")]
    [SerializeField] private int maxEnemies = 50;
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
        for (int i = 0; i < maxEnemies; i++)
        {
            GameObject enemy = Instantiate(EmptyEnemyRanged, _enemyHolder);
            rangedPool.Add(enemy);
            enemy.SetActive(false);
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
