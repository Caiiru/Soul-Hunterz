using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    #endregion
    public GameState CurrentGameState;

    #region Events
    public delegate void GameStateChangeHandler(GameState newState);
    public static event GameStateChangeHandler OnGameStateChange;

    #endregion

    [Space]
    [Header("Start Game Settings")]
    public GameObject PlayerPrefab;
    public GameObject StartAltarPrefab;
    public Transform[] altarSpawnPositions;

    [Space]
    [Header("Enemies Settings")]
    public Transform[] enemySpawnPositions;
    public GameObject enemyPrefab;
    public int maxEnemies = 5;
    public float enemySpawnInterval = 5f;
    private float enemySpawnTimer = 0f;
    private int currentEnemyCount = 0;
    public List<GameObject> activeEnemies = new List<GameObject>();

    [Space]
    [Header("Win Settings")]
    public GameObject winAltarPrefab;
    public Transform[] winAltarSpawnPositions;

    public int enemiesToDefeatToWin = 10;
    private int enemiesDefeated = 0;

    void Start()
    {
        // ChangeGameState(GameState.StartingGame);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeGameState(GameState newState)
    {
        if (CurrentGameState != newState)
        {
            CurrentGameState = newState;
            OnGameStateChange?.Invoke(newState);
            Debug.Log("Game State changed to: " + newState);
        }

        HandleGameState();
    }

    public bool IsGameState(GameState state)
    {
        return CurrentGameState == state;
    }
    public void HandleGameState()
    {
        switch (CurrentGameState)
        {
            case GameState.MainMenu:
                // Handle Main Menu logic
                break;
            case GameState.Loading:
                // Handle Loading logic
                break;
            case GameState.StartingGame:
                // Handle Starting Game logic
                StartGame();
                break;
            case GameState.Playing:
                // Handle Playing logic
                break;
            case GameState.Paused:
                // Handle Paused logic
                break;
            case GameState.GameOver:
                // Handle Game Over logic
                break;
            case GameState.Win:
                // Handle Win logic
                SpawnWinAltar();

                break;

            default:
                break;
        }
    }

    void StartGame()
    {
        System.Random random = new System.Random();
        int spawnIndex = random.Next(altarSpawnPositions.Length);
        Debug.Log("Selected Altar Spawn Index: " + spawnIndex);

        Transform altarSpawnPosition = altarSpawnPositions[spawnIndex];
        GameObject altar = Instantiate(StartAltarPrefab, altarSpawnPosition.position, Quaternion.identity);

        GameObject player = Instantiate(PlayerPrefab, altarSpawnPosition.position + new Vector3(0, 1.5f, 0), Quaternion.identity);

        ChangeGameState(GameState.Playing);
    }

    void SpawnWinAltar()
    {
        System.Random random = new System.Random();
        int spawnIndex = random.Next(winAltarSpawnPositions.Length);
        Transform altarSpawnPosition = winAltarSpawnPositions[spawnIndex];
        GameObject altar = Instantiate(winAltarPrefab, altarSpawnPosition.position, Quaternion.identity);
    }

    private void FixedUpdate()
    {
        HandleSpawning();
    }
    private void HandleSpawning()
    {
        if (IsGameState(GameState.Playing))
        {
            enemySpawnTimer += Time.fixedDeltaTime;
            if (enemySpawnTimer >= enemySpawnInterval && currentEnemyCount < maxEnemies)
            {
                SpawnEnemy();
                enemySpawnTimer = 0f;
            }
        }
    }
    private void SpawnEnemy()
    {
        if (enemySpawnPositions.Length == 0 || enemyPrefab == null)
            return;

        System.Random random = new System.Random();
        int spawnIndex = random.Next(enemySpawnPositions.Length);
        Transform spawnPosition = enemySpawnPositions[spawnIndex];

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition.position, Quaternion.identity);
        activeEnemies.Add(newEnemy);
        currentEnemyCount++;
    }


    public void EnemyDefeated(Enemy enemy)
    {
        if (activeEnemies.Contains(enemy.gameObject))
        {
            activeEnemies.Remove(enemy.gameObject);
            currentEnemyCount--;
            enemiesDefeated++;
        }

        if (currentEnemyCount < 0)
            currentEnemyCount = 0;


        // Check for win condition
        if (!IsGameState(GameState.Playing)) return;
        if (enemiesDefeated >= enemiesToDefeatToWin)
        {
            Debug.Log("All required enemies defeated! You win!");
            ChangeGameState(GameState.Win);
        }
    }

}

public enum GameState
{
    MainMenu,
    Loading,
    StartingGame,
    Playing,
    Paused,
    Win,
    GameOver
}
