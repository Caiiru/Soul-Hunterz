using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
            // DontDestroyOnLoad(this.gameObject);
        }
    }
    #endregion
    public GameState CurrentGameState;

    public int m_MaxAltars = 3;
    public int m_AltarsActivated = 0;

    #region Events
    public delegate void GameStateChangeHandler(GameState newState);
    public static event GameStateChangeHandler OnGameStateChange;

    //INGAME
    EventBinding<OnEnemyDied> enemyDiedEventBinding;

    EventBinding<OnPlayerDied> m_OnPlayerDiedBinding;

    EventBinding<StartGameEvent> m_OnMainMenuPlayButtonClicked;

    EventBinding<OnAltarActivated> m_OnAltarActivatedBinding;

    EventBinding<OnFinalAltarActivated> m_OnFinalAltarActivatedBinding;

    EventBinding<OnMapCollected> m_OnMapCollectedBinding;



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
    public GameObject EndZonePrefab; 

    [Space]
    [Header("Generic Manager")]
    public GameObject GenericManager;




    [Space]
    [Header("Binded Objects")]
    public GameObject _playerGO;
    private GameObject _environment;
    private GameObject _startZone;
    private GameObject _endZone;
    private EnemyManager _enemyManager;
    private GameObject m_GenericManager;

 
    private List<GameObject> altarSpawnPositions; 

    [Header("Game Scene")]
    public Transform m_FOGHolder;
    private Vector3 m_FOGStartScale;




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

        m_OnPlayerDiedBinding = new EventBinding<OnPlayerDied>(() =>
        {
            LoseGame();
        });
        EventBus<OnPlayerDied>.Register(m_OnPlayerDiedBinding);
        m_OnAltarActivatedBinding = new EventBinding<OnAltarActivated>(HandleAltarActivated);
        EventBus<OnAltarActivated>.Register(m_OnAltarActivatedBinding);


        m_OnFinalAltarActivatedBinding = new EventBinding<OnFinalAltarActivated>(HandleFinalAltarActivated);
        EventBus<OnFinalAltarActivated>.Register(m_OnFinalAltarActivatedBinding);

        m_OnMapCollectedBinding = new EventBinding<OnMapCollected>(HandleTutorialFinished);
        EventBus<OnMapCollected>.Register(m_OnMapCollectedBinding);

    }


    void UnbindEvents()
    {
        EventBus<StartGameEvent>.Unregister(m_OnMainMenuPlayButtonClicked);
        EventBus<OnAltarActivated>.Unregister(m_OnAltarActivatedBinding);
        EventBus<OnPlayerDied>.Unregister(m_OnPlayerDiedBinding);
        EventBus<OnEnemyDied>.Unregister(enemyDiedEventBinding);
        EventBus<OnFinalAltarActivated>.Unregister(m_OnFinalAltarActivatedBinding);
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
        if (m_FOGHolder != null)
        {
            m_FOGStartScale = m_FOGHolder.transform.localScale;
        }

    }
    private async UniTask SpawnObjects()
    {
        // _environment = Instantiate(EnvironmentPrefab);
        altarSpawnPositions = GameObject.FindGameObjectsWithTag("AltarSpawnPos").ToList();

        // SpawnStartAltar();
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
    }


    #region Events Methods

 

    private void HandleFinalAltarActivated(OnFinalAltarActivated arg0)
    {
        m_FOGHolder.gameObject.SetActive(true);
        m_FOGHolder.transform.DOScale(m_FOGStartScale, 0.5f);
    }

    private async void HandleTutorialFinished(OnMapCollected arg0)
    {
        if (m_FOGHolder != null && m_FOGHolder.gameObject.activeSelf)
            m_FOGHolder.DOScale(new Vector3(100, 100, 100), 10f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                m_FOGHolder.gameObject.SetActive(false);
            });

        EventBus<OnDisplayMessage>.Raise(new OnDisplayMessage { m_Message = "Use your map to find all altars" });
        await SpawnAllAltars();

        // cameraShakeManager.instance.CameraShake(GetComponent<CinemachineImpulseSource>());
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

    }

    public bool IsGameState(GameState state)
    {
        return CurrentGameState == state;
    }


    #region Altar Handling

    async UniTask SpawnNewAltar()
    {


        GameObject newAltar = Instantiate(EndZonePrefab, Vector3.zero, Quaternion.identity);
        newAltar.SetActive(false);
        newAltar.transform.position = GetRandomAltarSpawn().transform.position;
        newAltar.transform.position = new Vector3(
            newAltar.transform.position.x,
            newAltar.transform.position.y - 5,
            newAltar.transform.position.z);

        newAltar.GetComponent<WinAltar>().m_AltarIndex = m_AltarsActivated;

        await UniTask.WaitForSeconds(0.1f);
        newAltar.SetActive(true);
        newAltar.transform.DOMoveY(newAltar.transform.position.y + 4, 3f).SetEase(Ease.OutBounce);

        // _endZone.SetActive(false);
    }

    async UniTask SpawnAllAltars()
    {
        for (int i = 0; i < altarSpawnPositions.Count; i++)
        {
            GameObject newAltar = Instantiate(EndZonePrefab, altarSpawnPositions[i].transform.position, Quaternion.identity);

            newAltar.transform.position = new Vector3(
            newAltar.transform.position.x,
            newAltar.transform.position.y - 5,
            newAltar.transform.position.z);

            newAltar.GetComponent<WinAltar>().m_AltarIndex = i;


            await UniTask.WaitForSeconds(0.1f);
            newAltar.SetActive(true);
            newAltar.transform.DOMoveY(newAltar.transform.position.y + 4, 3f).SetEase(Ease.OutBounce);

        }
    }
 

    private async void HandleAltarActivated(OnAltarActivated arg0)
    {
        m_AltarsActivated++;

        if (m_AltarsActivated == m_MaxAltars)
        {
            // WinGame();
            EventBus<OnDisplayMessage>.Raise(new OnDisplayMessage { m_Message = "Return to activate the final altar" });
            return;
        }
        await SpawnNewAltar();




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
        ResetGame();
        EventBus<OnGameWin>.Raise(new OnGameWin());



        // EventBus<>.Raise(new OnGameWin());
    }
    public void LoseGame()
    {
        ChangeGameState(GameState.GameOver);
        ResetGame();
        EventBus<OnGameOver>.Raise(new OnGameOver());

        // UnbindEvents();
    }

    private async UniTask DestroyObjects()
    {
        // Destroy(_enemyManager.gameObject);
        await UniTask.Delay(100);
        // Destroy(m_GenericManager);
        // Destroy(_popupManagerGO);
        // Destroy(_environment);
        // Destroy(_startZone);
        // Destroy(_endZone);
        // Destroy(_playerGO);
    }

    private void ResetGame()
    { 
        m_AltarsActivated = 0;
        _enemyManager.SetCurrentWave(0);
        m_FOGHolder.gameObject.SetActive(true);
        m_FOGHolder.transform.localScale = m_FOGStartScale;
        // EventBus<OnGameOver>.Raise(new OnGameOver());

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

public static class GameSettings
{
    public const float k_AltarTimeActivation = 10;
}