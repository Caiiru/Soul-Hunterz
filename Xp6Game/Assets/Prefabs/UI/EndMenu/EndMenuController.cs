using UnityEngine;

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
        EventBus<ChangeMenuStateEvent>.Raise(new ChangeMenuStateEvent { newState = MenuState.MainMenu });
    }
    public void RetryGame()
    {
        EventBus<StartGameEvent>.Raise(new StartGameEvent());
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
