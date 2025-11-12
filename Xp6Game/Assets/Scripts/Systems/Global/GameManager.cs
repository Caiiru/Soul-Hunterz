using System.Collections.Generic;
using System.Linq; 
using Cysharp.Threading.Tasks; 
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

    EventBinding<OnGameStart> gameStartEventBinding;
    //INGAME
    EventBinding<OnEnemyDied> enemyDiedEventBinding;

    EventBinding<OnGameOver> m_OnGameOverBinding;

    EventBinding<StartGameEvent> m_OnMainMenuPlayButtonClicked;

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
    public GameObject EnvironmentPrefab;

    [Space]
    [Header("Generic Manager")]
    public GameObject GenericManager;




    [Space]
    [Header("Binded Objects")]
    public GameObject _playerGO;
    private GameObject _popupManagerGO;
    private GameObject _environment;
    private GameObject _startZone;
    private GameObject _endZone;
    private EnemyManager _enemyManager;
    private GameObject m_GenericManager;

    [Space]
    [Header("Start Settings")]

    public List<GameObject> altarSpawnPositions;

    [Space]
    [Header("Win Settings")]
    public Transform[] winAltarSpawnPositions;
    public int enemiesToDefeatToWin = 10;
    [SerializeField] private int enemiesDefeated = 0;


    #region Begin Game
    private void Start()
    {
        BindEvents();
        BindEnemiesEvents();
        // EventBus<OnGameReadyToStart>.Raise(new OnGameReadyToStart());
    }

    private async UniTask Initialize()
    {
        BindObjects();
        await SpawnObjects();
        PrepareGame();

        ChangeGameState(GameState.StartingGame);

        await BeginGame(); 

        EventBus<OnGameStart>.Raise(new OnGameStart());
        await UniTask.CompletedTask;
    }


    void BindEvents()
    {
        m_OnMainMenuPlayButtonClicked = new EventBinding<StartGameEvent>(StartGameHandler);
        EventBus<StartGameEvent>.Register(m_OnMainMenuPlayButtonClicked);

        m_OnGameOverBinding = new EventBinding<OnGameOver>(() =>
        {
            LoseGame();
        });
        EventBus<OnGameOver>.Register(m_OnGameOverBinding);

    }


    private void BindObjects()
    {
        _enemyManager = Instantiate(EnemyManagerPrefab).GetComponent<EnemyManager>();
        // m_GenericManager = Instantiate(GenericManager);
        if (this.transform.childCount > 0)
            m_GenericManager = this.transform.GetChild(0).gameObject;
        else
        {
            m_GenericManager = Instantiate(GenericManager);
            m_GenericManager.transform.parent = this.transform;
        }
    }
    private async UniTask SpawnObjects()
    {
        // _environment = Instantiate(EnvironmentPrefab);
        altarSpawnPositions = GameObject.FindGameObjectsWithTag("AltarSpawnPos").ToList();

        SpawnStartAltar();
        // await SpawnWinAltar();
        // SpawnPlayer();
        await _enemyManager.Initialize();
        //Spawn enemies pool 
        await UniTask.CompletedTask;
    }
    private void PrepareGame()
    {
        // ActivateStartAltar();
        // ChangePlayerPosition(_startZone.transform.position + Vector3.up);

    }
    async UniTask BeginGame()
    {
        ChangeGameState(GameState.Playing);
        // _enemyManager.StartSpawning();
        await UniTask.CompletedTask;
    }
    #endregion


    private void FixedUpdate()
    {
        // HandleSpawning();
    }



    private async void StartGameHandler(StartGameEvent arg0)
    {
        await Initialize();
    }

    void BindEnemiesEvents()
    {
        enemyDiedEventBinding = new EventBinding<OnEnemyDied>(OnEnemyDied);
        EventBus<OnEnemyDied>.Register(enemyDiedEventBinding);
    }


    #region Events Methods


    private void OnEnemyDied(OnEnemyDied arg0)
    {
        enemiesDefeated++;
        if (!IsGameState(GameState.Playing)) return;
        if (enemiesDefeated >= enemiesToDefeatToWin)
        {
            ActivateWinAltar();
        }
    }
    #endregion

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
        _endZone.SetActive(false);
    }
    void ActivateWinAltar()
    {
        if (_endZone.activeSelf) return; 
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
    #region Game End Methods
    public void WinGame()
    {
        ChangeGameState(GameState.Win);
        EventBus<OnGameWin>.Raise(new OnGameWin());
    }
    public async void LoseGame()
    {
        ChangeGameState(GameState.GameOver);
        await DestroyObjects();

    }

    private async UniTask DestroyObjects()
    {
        // Destroy(_enemyManager.gameObject);
        await UniTask.Delay(100);
        // Destroy(m_GenericManager);
        // Destroy(_popupManagerGO);
        Destroy(_environment);
        Destroy(_startZone);
        Destroy(_endZone);
        Destroy(_playerGO);
    }
    #endregion
    #region Getters
    public GameObject GetPlayer()
    {
        return _playerGO;
    }

    public EnemyManager GetEnemyManager()
    {
        return _enemyManager;
    }

    #endregion
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