using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugController : MonoBehaviour
{
    public KeyCode ResetGameKey = KeyCode.P;
    public KeyCode EndWaveKey = KeyCode.N;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ResetGameDebug();
        EndWaveDebug();
    }
    async void ResetGameDebug()
    {
        if (Input.GetKeyDown(ResetGameKey))
        {
            EventBus<OnGameWin>.Raise(new OnGameWin());
            EventBusUtil.ClearAllBuses();
            await SceneManager.LoadSceneAsync("Game");

        }
    }

    void EndWaveDebug()
    {
        if (Input.GetKeyDown(EndWaveKey))
        {
            EventBus<OnWaveClearedEvent>.Raise(new OnWaveClearedEvent());
        }
    }

}
