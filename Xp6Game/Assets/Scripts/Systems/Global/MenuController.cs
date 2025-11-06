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

    EventBinding<MainMenuPlayButtonClickedEvent> m_OnMainMenuPlayButtonClicked;
    EventBinding<GameOverEvent> m_OnGameOverEvent;
    EventBinding<GameWinEvent> m_OnGameWinEvent;




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
        DontDestroyOnLoad(gameObject);
    }


    #endregion

    void Start()
    {
        DesactivateAll();
        ChangeMenuState(m_MenuState);
        BindEvents();
        if(m_MenuState == MenuState.Game)
        {

            //debug -> start game 
            EventBus<MainMenuPlayButtonClickedEvent>.Raise(new MainMenuPlayButtonClickedEvent());
        }
    }

    private void BindEvents()
    {
        m_OnMainMenuPlayButtonClicked = new EventBinding<MainMenuPlayButtonClickedEvent>(() =>
        {
            m_MenuCamera.gameObject.SetActive(false);
            ChangeMenuState(MenuState.Game);
        });
        EventBus<MainMenuPlayButtonClickedEvent>.Register(m_OnMainMenuPlayButtonClicked);

        m_OnGameOverEvent = new EventBinding<GameOverEvent>(() =>
        {
            ChangeMenuState(MenuState.GameOver);
        });
        EventBus<GameOverEvent>.Register(m_OnGameOverEvent);


        m_OnGameWinEvent = new EventBinding<GameWinEvent>(() =>
        {
            ChangeMenuState(MenuState.Victory);
        });
        EventBus<GameWinEvent>.Register(m_OnGameWinEvent);
    }
    void OnDestroy()
    {
        UnbindEvents();
    }
    private void UnbindEvents()
    {
        EventBus<MainMenuPlayButtonClickedEvent>.Unregister(m_OnMainMenuPlayButtonClicked);

    }


    public void ChangeMenuState(MenuState newstate)
    {
        m_MenuState = newstate;
        HandleMenuState();
    }

    private void HandleMenuState()
    {
        DesactivateAll();
        
        switch (m_MenuState)
        {
            case MenuState.MainMenu:
                m_MainMenu.SetActive(true);
                m_MenuCamera.gameObject.SetActive(true);
                break;
            case MenuState.Victory:
                m_VictoryMenu.SetActive(true);
                m_MenuCamera.gameObject.SetActive(true);
                break;
            case MenuState.GameOver:
                m_MenuCamera.gameObject.SetActive(true);
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
