using UnityEngine;
using Cysharp.Threading.Tasks; 

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

    EventBinding<StartGameEvent> menuPlayButtonBinding; 
    EventBinding<GameStartLoadingEvent> loadingEventBinding;

    EventBinding<LoadMenuEvent> loadMenuBinding;



    public async void Start()
    {
        BindObjects();
        BindEvents();
        await _sceneLoader.Initialize();
        await InitializeMainMenu(); 
        await InitializeLoadingScreen();
    }

    void BindObjects()
    {
        if (TryGetComponent<SceneLoader>(out SceneLoader comp))
        {
            _sceneLoader = comp;
        } 
        var _gameManagerGO = Instantiate(GameManagerPrefab);
        _gameManager = _gameManagerGO.GetComponent<GameManager>();
        var _audioManagerGO = Instantiate(AudioManagerPrefab);
        _audioManager = _audioManagerGO.GetComponent<AudioManager>();


    }

    void BindEvents()
    {
        menuPlayButtonBinding = new EventBinding<StartGameEvent>(OnMainMenuPlayButtonClicked);
        EventBus<StartGameEvent>.Register(menuPlayButtonBinding);
 


        loadingEventBinding = new EventBinding<GameStartLoadingEvent>(() =>
        {
        });
        EventBus<GameStartLoadingEvent>.Register(loadingEventBinding);



        loadMenuBinding = new EventBinding<LoadMenuEvent>((LoadMenuEvent eventData) =>
        {
            OnMainMenuRequest(eventData);
        });
        EventBus<LoadMenuEvent>.Register(loadMenuBinding);

    }

    async void OnMainMenuPlayButtonClicked()
    {
        _sceneLoader.UnloadSceneByName("Initiator");
        _sceneLoader.DesactivateSceneByName("MainMenu");
        await InitializeGame(); 
        _sceneLoader.DesactivateSceneByName("LoadingScreen");
        EventBus<GameSceneLoaded>.Raise(new GameSceneLoaded());
    }

    public async UniTask InitializeMainMenu()
    {
        Debug.Log("Initialize Main Menu");
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

        _sceneLoader.UnloadSceneByName("MainMenu");
        await _sceneLoader.CreateSceneByName("Game");
        _sceneLoader.ActivateSceneByName("Game");
        _sceneLoader.SetMainScene("Game");


        await UniTask.CompletedTask;
    }

    public async void OnMainMenuRequest(LoadMenuEvent eventData)
    {
        string sceneName = eventData.fromScene;
        if (sceneName == null) return;

        _sceneLoader.UnloadSceneByName(sceneName);
        // switch (sceneName)
        // {
        //     case "GameWin":
        //         _sceneLoader.UnloadSceneByName("GameOver");
        //         break;
        //     case "GameOver":
        //         _sceneLoader.UnloadSceneByName("GameWin");
        //         break;
        //     default:
        //         Debug.LogWarning("Not identified scene");
        //         break;
        // }
        // _sceneLoader.UnloadSceneByName("Game");

        await InitializeMainMenu();

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
