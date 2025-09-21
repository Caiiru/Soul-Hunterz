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
    public GameState CurrentGameState { get; private set; }

    #region Events
    public delegate void GameStateChangeHandler(GameState newState);
    public static event GameStateChangeHandler OnGameStateChange;

    #endregion

    [Space]
    [Header("Start Game Settings")]
    public GameObject PlayerPrefab;
    public GameObject StartAltarPrefab;

    public Transform[] altarSpawnPositions;
    void Start()
    {
        ChangeGameState(GameState.StartingGame);
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
}

public enum GameState
{
    MainMenu,
    Loading,
    StartingGame,
    Playing,
    Paused,
    GameOver
}
