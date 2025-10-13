using Cysharp.Threading.Tasks;
using FMODUnity;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [SerializeField] private AnimationEventSound[] sounds;

    #region Singleton
    private static AudioManager instance;

    public static AudioManager Instance => instance;
    void Awake()
    {
        if (instance != this)
        {
            Destroy(this);
        }
        instance = this;
        DontDestroyOnLoad(this);
    }
    #endregion
    public async UniTask Initialize()
    {

        await UniTask.CompletedTask;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
[System.Serializable]
public struct AnimationEventSound
{
    public string eventName;                 // só pra identificar no inspector
    public EventReference soundEvent;        // referência do evento FMOD
}