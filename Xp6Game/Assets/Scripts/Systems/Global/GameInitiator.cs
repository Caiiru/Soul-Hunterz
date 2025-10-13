using System.Collections;
using UnityEditor;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Collections.Specialized;

public class GameInitiator : MonoBehaviour
{

    [Header("Bind Objects")]
    public GameObject AudioManagerPrefab;
    public GameObject GameManagerPrefab;



    [Header("Binded Objects")]
    [SerializeField] private SceneLoader _sceneLoader;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private AudioManager _audioManager;


    //Events

    EventBinding<MainMenuPlayButtonClickedEvent> menuPlayButtonBinding;

    public async void Start()
    {


        BindObjects();
        BindEvents();
        await InitializeMainMenu();
        await InitializeAudio();
        await InitializeLoadingScreen();
    }

    void BindObjects()
    {
        if (TryGetComponent<SceneLoader>(out SceneLoader comp))
        {
            _sceneLoader = comp;
        }
        else
        {
            _sceneLoader = this.transform.AddComponent<SceneLoader>();
        }
        var _gameManagerGO = Instantiate(GameManagerPrefab);
        _gameManager = _gameManagerGO.GetComponent<GameManager>();
        var _audioManagerGO = Instantiate(AudioManagerPrefab);
        _audioManager = _audioManagerGO.GetComponent<AudioManager>();


    }

    void BindEvents()
    {
        menuPlayButtonBinding = new EventBinding<MainMenuPlayButtonClickedEvent>(OnMainMenuPlayButtonClicked);
        EventBus<MainMenuPlayButtonClickedEvent>.Register(menuPlayButtonBinding);
    }

    async void OnMainMenuPlayButtonClicked()
    {
        _sceneLoader.DesactivateSceneByName("MainMenu");
        await InitializeGame();
        EventBus<GameSceneLoaded>.Raise(new GameSceneLoaded());
        _sceneLoader.ActivateSceneByName("Game");
        _sceneLoader.DesactivateSceneByName("LoadingScreen"); 
    }

    public async UniTask InitializeMainMenu()
    {
        await _sceneLoader.Initialize();
        await _sceneLoader.CreateSceneByName("MainMenu");
        _sceneLoader.ActivateSceneByName("MainMenu");

        EventBus<MainMenuLoadedEvent>.Raise(new MainMenuLoadedEvent());

    }
    public async UniTask InitializeLoadingScreen()
    {
        await _sceneLoader.CreateSceneByName("LoadingScreen");
        // _sceneLoader.ActivateSceneByName("LoadingScreen");
        await UniTask.CompletedTask;
    }
    public async UniTask InitializeGame()
    {

        await _sceneLoader.CreateSceneByName("Game");
        _sceneLoader.ActivateSceneByName("Game");


        await UniTask.CompletedTask;
    }


    public async UniTask InitializeAudio()
    {
        await _audioManager.Initialize();
    }




    #region Singleton
    public static GameInitiator Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }
    #endregion
}
