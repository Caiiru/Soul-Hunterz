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

    EventBinding<OnStartAltarActivation> m_OnStartAltarActivationBinding;


    EventBinding<OnFinalAltarActivated> m_OnFinalAltarActivatedBinding;

    EventBinding<OnWaveClearedEvent> m_OnWaveClearedEventBinding;


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
    private EnemyManager _enemyManager;
    private GameObject m_GenericManager;

    public GameObject[] m_Altars;



    [Header("Game Scene")]
    public Transform m_FOGHolder;
    private Vector3 m_FOGStartScale;

    public Material m_FogStartMaterial;
    public Material m_FogAltarMaterial;
    //261C4B
    //red 891B17



    #region Begin Game
    private void Start()
    {
        BindEvents();
        BindEnemiesEvents();

        m_FOGHolder.GetComponent<MeshRenderer>().material = m_FogStartMaterial;
        // mat.DOColor(m_FogStartColor, 1f);
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

        ChangeGameState(GameState.Playing);

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

        m_OnStartAltarActivationBinding = new EventBinding<OnStartAltarActivation>((args) =>
        {

            m_FOGHolder.GetComponent<MeshRenderer>().material = m_FogAltarMaterial;
            foreach (var altar in m_Altars)
            {
                if (args.m_Direction == altar.GetComponent<WinAltar>().m_AltarDirection)
                {
                    SetFogPosition(altar.transform.position);
                    SetFogScale(new Vector3(6f, 6f, 1f), 1f, false);

                    return;
                }
            }
        });
        EventBus<OnStartAltarActivation>.Register(m_OnStartAltarActivationBinding);

        m_OnWaveClearedEventBinding = new EventBinding<OnWaveClearedEvent>(() =>
        {
            SetFogScale(Vector3.one * 300, 5f, true);
        });
        EventBus<OnWaveClearedEvent>.Register(m_OnWaveClearedEventBinding);




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

    void SetFogScale(Vector3 scale, float duration, bool disappear)
    {
        if (!disappear)
            m_FOGHolder.gameObject.SetActive(true);
        m_FOGHolder.transform.DOScale(scale, duration).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            if (disappear)
            {
                m_FOGHolder.gameObject.SetActive(false);
            }
        });
    }
    void SetFogPosition(Vector3 pos)
    {
        m_FOGHolder.transform.position = pos;
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
        m_FOGHolder.transform.position = new Vector3(-7.79f, -4.49f, 6.76f);
        m_FOGHolder.gameObject.SetActive(true);
        m_FOGHolder.transform.DOScale(m_FOGStartScale, 0.5f);
    }

    private async void HandleTutorialFinished(OnMapCollected arg0)
    {
        SetFogScale(Vector3.one * 200f, 5f, true);
        EventBus<OnDisplayMessage>.Raise(new OnDisplayMessage { m_Message = "Use o mapa para encontrar e ativar todos os altares " });


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



    private async void HandleAltarActivated(OnAltarActivated arg0)
    {
        m_AltarsActivated++;


        return;
        if (m_AltarsActivated == m_MaxAltars)
        {
            // WinGame();
            EventBus<OnDisplayMessage>.Raise(new OnDisplayMessage { m_Message = "Retorne para ativar o portal" });
            return;
        }


        // await SpawnNewAltar();




    }


    #endregion
    #region Game End Methods
    public void WinGame()
    {
        // EventBus<OnGameWin>.Raise(new OnGameWin());
        EventBus<OnGameOver>.Raise(new OnGameOver());



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


    void UnbindEvents()
    {
        EventBus<StartGameEvent>.Unregister(m_OnMainMenuPlayButtonClicked);
        EventBus<OnAltarActivated>.Unregister(m_OnAltarActivatedBinding);
        EventBus<OnPlayerDied>.Unregister(m_OnPlayerDiedBinding);
        EventBus<OnEnemyDied>.Unregister(enemyDiedEventBinding);
        EventBus<OnFinalAltarActivated>.Unregister(m_OnFinalAltarActivatedBinding);
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