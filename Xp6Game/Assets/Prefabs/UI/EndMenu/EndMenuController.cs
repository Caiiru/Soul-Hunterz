using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndMenuController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void BackToMenu()
    {
        // EventBus<ChangeMenuStateEvent>.Raise(new ChangeMenuStateEvent { newState = MenuState.MainMenu });//
        // SceneManager.LoadScene("Game");
        RetryGame();
    }
    public void RetryGame()
    {

        // await LoadGameScene();

        EventBus<ChangeMenuStateEvent>.Raise(new ChangeMenuStateEvent { newState = MenuState.MainMenu });//
        // EventBus<StartGameEvent>.Raise(new StartGameEvent());
    }

    async UniTask LoadGameScene()
    {

        await SceneManager.LoadSceneAsync("Game");
        await UniTask.CompletedTask;
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
