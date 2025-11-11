using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject settingsUI;
    [SerializeField] private Camera menuCamera;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject blackPanel;

    #region Events 

    #endregion

    public void OnStartButtonPressed()
    {
        EventBus<StartGameEvent>.Raise(new StartGameEvent());

    }

    void Awake()
    {
        blackPanel.SetActive(true);
        
    }
    void Start()
    {
        blackPanel.GetComponent<Image>().DOFade(0, 5f).OnComplete(() =>
        {
            blackPanel.SetActive(false);
        });
    }
    void Update()
    {

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

    public void QuitGame()
    {
        Application.Quit();
    }
}
