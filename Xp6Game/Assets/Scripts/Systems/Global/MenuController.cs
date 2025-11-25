using UnityEngine;

public class MenuController : MonoBehaviour
{
    public MenuState m_MenuState;
    [Space]

    public GameObject m_MainMenu;
    public GameObject m_VictoryMenu;
    public GameObject m_GameOverMenu;

    [Space]
    public GameObject m_MenuCamera;

    //Events

    EventBinding<StartGameEvent> m_StartButtonClicked;
    EventBinding<OnGameOver> m_OnGameOverEvent;
    EventBinding<OnGameWin> m_OnGameWinEvent;
    EventBinding<ChangeMenuStateEvent> m_OnMenuStateChanged;




    #region Singleton
    //Singleton

    public static MenuController Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // DontDestroyOnLoad(gameObject);
    }


    #endregion

    void Start()
    {
        DesactivateAll();
        ChangeMenuState(m_MenuState);
        BindEvents();
        if (m_MenuState == MenuState.Game)
        {

            //debug -> start game 
            EventBus<StartGameEvent>.Raise(new StartGameEvent());
        }
    }

    private void BindEvents()
    {
        m_StartButtonClicked = new EventBinding<StartGameEvent>(() =>
        {
            // m_MenuCamera.gameObject.SetActive(false);
            ChangeMenuState(MenuState.Game);
        });
        EventBus<StartGameEvent>.Register(m_StartButtonClicked);

        m_OnGameOverEvent = new EventBinding<OnGameOver>(() =>
        {
            ChangeMenuState(MenuState.GameOver);
        });
        EventBus<OnGameOver>.Register(m_OnGameOverEvent);


        m_OnGameWinEvent = new EventBinding<OnGameWin>(() =>
        {
            ChangeMenuState(MenuState.Victory);
            Debug.Log("Win Menu Controller");
        });
        EventBus<OnGameWin>.Register(m_OnGameWinEvent);

        m_OnMenuStateChanged = new EventBinding<ChangeMenuStateEvent>(ChangeMenuEvent);
        EventBus<ChangeMenuStateEvent>.Register(m_OnMenuStateChanged);
    }
    void OnDestroy()
    {
        UnbindEvents();
    }
    private void UnbindEvents()
    {
        EventBus<StartGameEvent>.Unregister(m_StartButtonClicked);
        EventBus<OnGameOver>.Unregister(m_OnGameOverEvent);
        EventBus<OnGameWin>.Unregister(m_OnGameWinEvent);
        EventBus<ChangeMenuStateEvent>.Unregister(m_OnMenuStateChanged);

    }


    public void ChangeMenuState(MenuState newstate)
    {
        m_MenuState = newstate;
        HandleMenuState();
    }

    public void ChangeMenuEvent(ChangeMenuStateEvent args)
    {
        ChangeMenuState(args.newState);
    }

    private void HandleMenuState()
    {
        DesactivateAll();

        switch (m_MenuState)
        {
            case MenuState.MainMenu:
                m_MainMenu.SetActive(true);
                // m_MenuCamera.gameObject.SetActive(true);
                break;
            case MenuState.Victory:
                m_VictoryMenu.SetActive(true);
                // m_MenuCamera.gameObject.SetActive(true);
                m_VictoryMenu.SetActive(true);
                break;
            case MenuState.GameOver:
                // m_MenuCamera.gameObject.SetActive(true);
                m_GameOverMenu.SetActive(true);
                break;

            case MenuState.Game:
                break;
            default:
                break;

        }
    }
    private void DesactivateAll()
    {
        m_MainMenu.SetActive(false);
        m_VictoryMenu.SetActive(false);
        m_GameOverMenu.SetActive(false);
    }





}

public enum MenuState
{
    MainMenu,
    Victory,
    GameOver,
    Game
}
