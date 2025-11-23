using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // --- Variáveis de Controle de Spawn ---
    [SerializeField] private bool canSpawn = false;
    [SerializeField] private float spawnInterval = 5f;
    private float timer = 0;

    // Contagem de inimigos ativos (retirados do Pool)
    [SerializeField] private int m_EnemiesActive = 0;

    // Índice da onda atual a ser spawnada (usado no Evento OnAltarActivatedHandler)
    private int m_spawnActivated = -1;

    bool m_isWaveActive = false;

    // Fonte de números aleatórios
    private System.Random _random;

    // --- Referências Essenciais ---
    [Header("Pool & Data")]
    public WaveData[] EnemiesToSpawn;
    [SerializeField] private EnemyPool m_EnemyPool; // Mudei para o nome da classe que você criou

    // Queue para armazenar as EnemyData que PRECISAM ser spawnadas (ordenadas e com duplicatas).
    private readonly Queue<EnemyData> m_enemiesToSpawnQueue = new Queue<EnemyData>();

    // --- Posições de Spawn ---
    [Header("Positions")]
    public GameObject[] enemySpawnPosition;
    public AltarSpawn[] m_AltarSpawns; // Mantido, mas não usado diretamente no spawn aqui

    private EnemyManager _enemyManager;
    private bool m_isFinalForm;

    // --- Eventos (Mantidos Ilesos) ---
    EventBinding<OnEnemyDied> m_OnEnemyDiedBinding;
    EventBinding<OnFinalAltarActivated> m_OnFinalAltarActivatedBinding;
    EventBinding<OnAltarActivated> m_OnAltarActivatedBinding;


    void Start()
    {
        // Certifique-se de que o pool manager é o componente correto (EnemyPoolManager)
        m_EnemyPool = gameObject.GetComponent<EnemyPool>();
        if (m_EnemyPool == null)
        {
            Debug.LogError("EnemyPoolManager não encontrado no GameObject! O Pool não funcionará.");
        }

        _random = new System.Random();
        BindEvents();
    }

    private void BindEvents()
    {
        // ... (Seus Event Bindings Inalterados)
        m_OnEnemyDiedBinding = new EventBinding<OnEnemyDied>(OnEnemyDiedHandler);
        EventBus<OnEnemyDied>.Register(m_OnEnemyDiedBinding); // Adicionei o registro

        m_OnFinalAltarActivatedBinding = new EventBinding<OnFinalAltarActivated>(HandlerFinalAltar);
        EventBus<OnFinalAltarActivated>.Register(m_OnFinalAltarActivatedBinding);

        m_OnAltarActivatedBinding = new EventBinding<OnAltarActivated>(OnAltarActivatedHandler);
        EventBus<OnAltarActivated>.Register(m_OnAltarActivatedBinding);
    }

    // --- Lógica de Eventos ---

    private void OnAltarActivatedHandler(OnAltarActivated arg0)
    {
        // 1. Define as posições de spawn baseadas no Altar
        m_isWaveActive = true;
        Transform spawnholder = arg0.m_SpawnPointHolder;
        enemySpawnPosition = new GameObject[spawnholder.childCount];

        for (int i = 0; i < spawnholder.childCount; i++)
        {
            enemySpawnPosition[i] = spawnholder.GetChild(i).gameObject;
        }

        // 2. Prepara a Queue de Inimigos para a Onda

        m_spawnActivated++;
        PrepareWaveQueue(m_spawnActivated);

        // 3. Começa a spawnar no FixedUpdate
        StartSpawning();
    }

    // --- Novo Método: Prepara a fila de inimigos ---
    public void PrepareWaveQueue(int waveCount)
    {
        if (waveCount < 0 || waveCount >= EnemiesToSpawn.Length)
        {
            Debug.LogError($"WaveData inválida para o índice {waveCount}.");
            return;
        }

        WaveData currentWave = EnemiesToSpawn[waveCount];

        // Limpa a fila antes de adicionar a nova onda
        m_enemiesToSpawnQueue.Clear();

        // Itera sobre CADA tipo de inimigo
        foreach (var enemyData in currentWave.m_enemies)
        {
            // Adiciona o prefab na Queue o número 'amount' de vezes
            for (int i = 0; i < enemyData.amount; i++)
            {
                m_enemiesToSpawnQueue.Enqueue(enemyData);
            }
        }

        // Define o intervalo de spawn para a onda atual
        spawnInterval = currentWave.m_spawnDelay;
    }


    // O Coroutine 'SpawnWave' não é mais necessário, pois a Queue agora alimenta o FixedUpdate
    // Se você quiser spawnar todos de uma vez (Burst), o código iria aqui.
    // Mantenha o FixedUpdate para spawns sequenciais baseados em `spawnInterval`.


    private void HandlerFinalAltar(OnFinalAltarActivated arg0)
    {
        m_enemiesToSpawnQueue.Clear();
        StopSpawning(); // Para qualquer spawn pendente
        m_isFinalForm = true;
    }

    private void OnEnemyDiedHandler(OnEnemyDied arg0)
    {
        if (!m_isWaveActive) return;


        m_EnemiesActive--;

        // Lógica: Se a fila estiver vazia E não houver inimigos ativos, a onda terminou.
        if (m_enemiesToSpawnQueue.Count == 0 && m_EnemiesActive <= 0)
        {
            EventBus<OnWaveClearedEvent>.Raise(new OnWaveClearedEvent());
            // EventBus<WaveEndEvent>.Raise(new WaveEndEvent()); // Exemplo de evento de fim de onda
            Debug.Log("Wave ended");
        }
    }

    // --- Loop de Spawn com Delay ---
    void FixedUpdate()
    {
        // Só tenta spawnar se puder, se a fila não estiver vazia e se houver posições de spawn
        if (!canSpawn || m_enemiesToSpawnQueue.Count == 0 || enemySpawnPosition.Length == 0) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0;
            SpawnNextEnemyFromQueue(GetRandomSpawnPosition());
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

    // --- Novo Método de Spawn (Usando Pool) ---
    private void SpawnNextEnemyFromQueue(Vector3 spawnPosition)
    {
        if (m_enemiesToSpawnQueue.Count == 0 || m_EnemyPool == null)
        {
            StopSpawning();
            return;
        }

        // Pega o PRÓXIMO inimigo na fila (garantindo a ordem da WaveData)
        EnemyData enemyDataToSpawn = m_enemiesToSpawnQueue.Dequeue();

        // Usa o PoolManager para pegar o inimigo
        GameObject enemy = m_EnemyPool.SpawnFromPool(enemyDataToSpawn, spawnPosition, Quaternion.identity);

        if (enemy != null)
        {
            m_EnemiesActive++;

        }
    }

    // --- Métodos Auxiliares ---

    private Vector3 GetRandomSpawnPosition()
    {
        // RandomOffset mantido para espalhar os inimigos
        Vector3 randomOffset = new Vector3(_random.Next(1, 1), 0, _random.Next(1, 1));

        int randomIndex = _random.Next(enemySpawnPosition.Length);
        return enemySpawnPosition[randomIndex].transform.position;
    }

    public int GetActiveEnemies()
    {
        return m_EnemiesActive;
    }
}
[System.Serializable]

public struct AltarSpawn

{

    public AltarDirection altarDirection;

    public Transform[] spawnPoints;

}