using System.Collections;
using UnityEditor;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class GameInitiator : MonoBehaviour
{
    [SerializeField] private SceneAsset mainMenuScene;
    [SerializeField] private SceneAsset gameScene;
    [SerializeField] private SceneAsset endScene;


    public async void Start()
    {
        await InitializeMainMenu();
    }

    private async UniTask InitializeMainMenu()
    {
        await SceneManager.LoadSceneAsync(mainMenuScene.name, LoadSceneMode.Additive);
    }
    public async UniTask InitializeGame()
    {
        await SceneManager.LoadSceneAsync(gameScene.name, LoadSceneMode.Additive);
        // GameManager.Instance.

        // GameManager.Instance.ChangeGameState(GameState.MainMenu);
 
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
            DontDestroyOnLoad(this.gameObject);
        }
    }
    #endregion
}
