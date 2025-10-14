using UnityEngine;
using UnityEngine.SceneManagement;

public class EndMenuManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BackToMainMenu()
    {
        // ;        GameManager.Instance.ChangeGameState(GameState.MainMenu);
        // SceneManager.LoadScene("Initiator", LoadSceneMode.Single);
        string sceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"FROM {sceneName}");
        EventBus<LoadMenuEvent>.Raise(new LoadMenuEvent() { fromScene =  sceneName});
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
