using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
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
 
    EventBinding<GameStartLoadingEvent> gameStartLoadingBinding;
    EventBinding<GameStartEvent> gameStartEventBinding;
    //INGAME
    EventBinding<EnemyDiedEvent> enemyDiedEventBinding;

    EventBinding<MainMenuPlayButtonClickedEvent> m_OnMainMenuPlayButtonClicked;

    #endregion


    [Space]
    [Header("Bind Objects")]

    public GameObject EventSystemPrefab;
    public GameObject GlobalVolumePrefab;

    [Space]
    public GameObject EnemyManagerPrefab;
    public GameObject PopupManagerPrefab;

    [Space]
    public GameObject PlayerPrefab; 
    public GameObject StartZonePrefab;
    public GameObject EndZonePrefab;
    public GameObject EnemyPrefab;
    public GameObject EnvironmentPrefab;



    [Space]
    [Header("Binded Objects")]
    [SerializeField] private GameObject _playerGO;
    private GameObject _popupManagerGO; 
    private GameObject _environment;
    private GameObject _startZone;
    private GameObject _endZone;
    private EnemyManager _enemyManager;

    [Space]
    [Header("Start Settings")]

    public List<GameObject> altarSpawnPositions;

    [Space]
    [Header("Win Settings")]
    public Transform[] winAltarSpawnPositions;
    public int enemiesToDefeatToWin = 10;
    [SerializeField] private int enemiesDefeated = 0;


    #region Begin Game
    private async void Start()
    {
        BindEvents();
        BindEnemiesEvents(); 
        // BindObjects();
        // await SpawnObjects();
        // PrepareGame();

        // await BeginGame();
    }


    void BindEvents()
    {
        m_OnMainMenuPlayButtonClicked = new EventBinding<MainMenuPlayButtonClickedEvent>(StartGameHandler);
        EventBus<MainMenuPlayButtonClickedEvent>.Register(m_OnMainMenuPlayButtonClicked);

    
 
        gameStartLoadingBinding = new EventBinding<GameStartLoadingEvent>(() =>
        {
            ChangeGameState(GameState.Loading);

        });



    }

    private async void StartGameHandler(MainMenuPlayButtonClickedEvent arg0)
    {
        await Initialize();
    }

    void BindEnemiesEvents()
    {
        enemyDiedEventBinding = new EventBinding<EnemyDiedEvent>(OnEnemyDied);
        EventBus<EnemyDiedEvent>.Register(enemyDiedEventBinding); 
    }


    #region Events Methods
    

    private void OnEnemyDied(EnemyDiedEvent arg0)
    { 
        enemiesDefeated++;
        if (!IsGameState(GameState.Playing)) return;
        if (enemiesDefeated >= enemiesToDefeatToWin)
        {
            ActivateWinAltar();
        }
    }
    #endregion

    private async UniTask Initialize()
    {
        BindObjects();
        await SpawnObjects();
        PrepareGame();

        EventBus<GameReadyToStartEvent>.Raise(new GameReadyToStartEvent());
        ChangeGameState(GameState.StartingGame);

        await BeginGame();
        Debug.Log("Initialize");

        EventBus<GameStartEvent>.Raise(new GameStartEvent());
        await UniTask.CompletedTask;
    }

    private void BindObjects()
    { 
        Instantiate(GlobalVolumePrefab);
        Instantiate(EventSystemPrefab);

        _popupManagerGO = Instantiate(PopupManagerPrefab);
        _enemyManager = Instantiate(EnemyManagerPrefab).GetComponent<EnemyManager>();
    }
    private async UniTask SpawnObjects()
    {
        _environment = Instantiate(EnvironmentPrefab);
        altarSpawnPositions = GameObject.FindGameObjectsWithTag("AltarSpawnPos").ToList();

        SpawnStartAltar();
        await SpawnWinAltar();
        SpawnPlayer();
        await _enemyManager.Initialize();
        //Spawn enemies pool 
        await UniTask.CompletedTask;
    }
    private void PrepareGame()
    {
        ActivateStartAltar();
        ChangePlayerPosition(_startZone.transform.position + Vector3.up);

    }
    async UniTask BeginGame()
    {
        ChangeGameState(GameState.Playing);
        _enemyManager.StartSpawning();
        await UniTask.CompletedTask;
    }
    #endregion


    private void FixedUpdate()
    {
        // HandleSpawning();
    }
    void SpawnPlayer()
    {
        GameObject player = Instantiate(PlayerPrefab);
        _playerGO = player; 

    }
    void ChangePlayerPosition(Vector3 newPosition)
    {
        _playerGO.transform.position = newPosition;

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
                // Handle Win 

                break;

            default:
                break;
        }
    }

    #region Altar Handling

    async UniTask SpawnWinAltar()
    {
        _endZone = Instantiate(EndZonePrefab, Vector3.zero, Quaternion.identity);

        await UniTask.WaitForSeconds(0.1f);
        Debug.Log("Win Altar Spawned and Desactivated");
        _endZone.SetActive(false);
    }
    void ActivateWinAltar()
    {
        if (_endZone.activeSelf) return;
        Debug.Log("Win altar instancied");
        _endZone.SetActive(true);
        _endZone.transform.position = GetRandomAltarSpawn().transform.position;
    }

    void SpawnStartAltar()
    {
        _startZone = Instantiate(StartZonePrefab);
        _startZone.SetActive(false);
    }
    void ActivateStartAltar()
    {
        _startZone.transform.position = GetRandomAltarSpawn().transform.position;
        _startZone.SetActive(true);
    }

    GameObject GetRandomAltarSpawn()
    {
        System.Random random = new System.Random();
        int spawnIndex = random.Next(altarSpawnPositions.Count);
        GameObject altarSpawnPosition = altarSpawnPositions[spawnIndex];
        altarSpawnPositions.RemoveAt(spawnIndex);

        return altarSpawnPosition;
    }
    #endregion

    public void WinGame()
    {
        ChangeGameState(GameState.Win);
        EventBus<GameWinEvent>.Raise(new GameWinEvent());
    }
    public void LoseGame()
    {
        ChangeGameState(GameState.GameOver);
    }

    public GameObject GetPlayer()
    {
        return _playerGO;
    }

    public EnemyManager GetEnemyManager()
    {
        return _enemyManager;
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