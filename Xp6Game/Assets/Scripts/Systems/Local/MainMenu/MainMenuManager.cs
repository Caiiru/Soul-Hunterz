using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject gameOverMenuUI;
    [SerializeField] private GameObject settingsUI;
    [SerializeField] private Camera menuCamera;
    [SerializeField] private EventSystem eventSystem;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public async void StartGame()
    {
        await GameInitiator.Instance.InitializeGame();
        Debug.Log("Game Started");
        GameManager.Instance.ChangeGameState(GameState.StartingGame);
        DisableMenu();

    }

    public void OpenSettings()
    {
        mainMenuUI.SetActive(false);
        settingsUI.SetActive(true);
    }
    public void CloseSettings()
    {
        settingsUI.SetActive(false);
        mainMenuUI.SetActive(true);
    }
    public void DisableMenu()
    {
        mainMenuUI.SetActive(false);
        menuCamera.transform.gameObject.SetActive(false);
        if (settingsUI != null)
            settingsUI.SetActive(false);

        eventSystem.gameObject.SetActive(false);
    }
    public void EnableMenu()
    {
        mainMenuUI.SetActive(true);
        menuCamera.transform.gameObject.SetActive(true);
        eventSystem.gameObject.SetActive(true);
    }
    public void EnableGameOverMenu()
    {
        mainMenuUI.SetActive(false);
        gameOverMenuUI.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
