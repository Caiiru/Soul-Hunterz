using Cysharp.Threading.Tasks;
using FMODUnity;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    // [SerializeField] private AnimationEventSound[] sounds;

    #region Singleton
    private static AudioManager instance;

    public static AudioManager Instance => instance;

    void Awake()
    {
        if (instance != this && instance != null)
        {
            Destroy(this.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion
    public async UniTask Initialize()
    {
        await UniTask.CompletedTask;
    }

    
    public void PlayOneShotAtPosition(EventReference eventRef, Vector3 position)
    { 
        RuntimeManager.PlayOneShot(eventRef, position);
    }


}
[System.Serializable]
public struct AnimationEventSound
{
    public string eventName;                 // só pra identificar no inspector
    public EventReference soundEvent;        // referência do evento FMOD
}