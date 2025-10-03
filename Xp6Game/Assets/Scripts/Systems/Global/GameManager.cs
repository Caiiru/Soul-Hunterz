using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            // DontDestroyOnLoad(this.gameObject);
        }
    }
    #endregion
    public GameState CurrentGameState;

    #region Events
    public delegate void GameStateChangeHandler(GameState newState);
    public static event GameStateChangeHandler OnGameStateChange;

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
    public GameObject CameraPrefab;
    public GameObject StartZonePrefab;
    public GameObject EndZonePrefab;
    public GameObject EnemyPrefab;
    public GameObject EnvironmentPrefab;



    [Space]
    [Header("Binded Objects")]
    [SerializeField] private GameObject _playerGO;
    private GameObject _popupManagerGO;
    private GameObject _cameraGO;
    private CinemachineCamera _cinemachineCamera;
    private GameObject _environment;
    private GameObject _startZone;
    private GameObject _endZone;
    [SerializeField] private EnemyManager _enemyManager;

    public EnemyManager GetEnemyManager => _enemyManager;

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
        BindObjects();
        await SpawnObjects();
        PrepareGame();

        await BeginGame();
    }

    private void BindObjects()
    {
        _cameraGO = Instantiate(CameraPrefab);
        _cinemachineCamera = _cameraGO.GetComponentInChildren<CinemachineCamera>();

        Instantiate(GlobalVolumePrefab);
        Instantiate(EventSystemPrefab);

        _popupManagerGO = Instantiate(PopupManagerPrefab);
        var enemyManagerGO = Instantiate(EnemyManagerPrefab);

        _enemyManager = enemyManagerGO.GetComponent<EnemyManager>();

    }
    private async UniTask SpawnObjects()
    {
        _environment = Instantiate(EnvironmentPrefab);
        await _enemyManager.Initialize();
        altarSpawnPositions = GameObject.FindGameObjectsWithTag("AltarSpawnPos").ToList();
        SpawnStartAltar();
        SpawnWinAltar();
        SpawnPlayer();
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
        _cinemachineCamera.Target.TrackingTarget = _playerGO.transform;

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
                SceneManager.LoadScene("EndMenuScene", LoadSceneMode.Single);
                break;

            default:
                break;
        }
    }

    #region Altar Handling

    void SpawnWinAltar()
    {
        _endZone = Instantiate(EndZonePrefab, Vector3.zero, Quaternion.identity);
        _endZone.SetActive(false);
    }
    void ActivateWinAltar()
    {
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

    public void EnemyDefeated()
    {
        Debug.Log("One more enemy defeated");
        // Check for win condition
        if (!IsGameState(GameState.Playing)) return;
        enemiesDefeated++;
        if (enemiesDefeated >= enemiesToDefeatToWin)
        {
            ActivateWinAltar();
        }
    }
    public void WinGame()
    {
        ChangeGameState(GameState.Win);
    }
    public void LoseGame()
    {
        ChangeGameState(GameState.GameOver);
    }

    public GameObject GetPlayer()
    {
        return _playerGO;
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
