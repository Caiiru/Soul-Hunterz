using Cysharp.Threading.Tasks;
using FMODUnity;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    // [SerializeField] private AnimationEventSound[] sounds;

    private static AudioManager instance;

    public static AudioManager Instance => instance;

    public AnimationEventSound AltarActivationEvent;
    public AnimationEventSound WaveClearedEvent;

    public bool m_DebugEvent;

    //Events
    EventBinding<OnStartAltarActivation> m_OnStartAltarActivationBinding;
    EventBinding<OnWaveClearedEvent> m_OnWaveClearedBinding;
    EventBinding<OnFinalAltarActivated> m_OnFinalAltarActivatedBinding;
    EventBinding<OnPlayerDied> _playerDiedBinding;
    EventBinding<OnGameWin> _gameWinBinding;





    #region Singleton

    void Awake()
    {
        if (instance != this && instance != null)
        {
            Destroy(this.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        BindEvents();
    }


    void OnDestroy()
    {
        UnbindEvents();
    }
    #endregion

    void Update()
    {
        if (m_DebugEvent)
        {
            m_DebugEvent = false;
            HandleAltarActivation();
        }
    }
    private void BindEvents()
    {
        m_OnStartAltarActivationBinding = new EventBinding<OnStartAltarActivation>(HandleAltarActivation);
        EventBus<OnStartAltarActivation>.Register(m_OnStartAltarActivationBinding);

        m_OnWaveClearedBinding = new EventBinding<OnWaveClearedEvent>(() =>
        {
            PlayOneShotAtPosition(WaveClearedEvent.soundEvent, transform.position);
        });
        EventBus<OnWaveClearedEvent>.Register(m_OnWaveClearedBinding);

        m_OnFinalAltarActivatedBinding = new EventBinding<OnFinalAltarActivated>(() =>
        {
            PlayOneShotAtPosition(AltarActivationEvent.soundEvent, transform.position);
        });
        EventBus<OnFinalAltarActivated>.Register(m_OnFinalAltarActivatedBinding);

        //Game Over & Game win

        _playerDiedBinding = new EventBinding<OnPlayerDied>(() =>
       {
           PlayOneShotAtPosition(WaveClearedEvent.soundEvent, transform.position);
       });
        EventBus<OnPlayerDied>.Register(_playerDiedBinding);

        _gameWinBinding = new EventBinding<OnGameWin>(() =>
      {

          PlayOneShotAtPosition(WaveClearedEvent.soundEvent, transform.position);
      });
        EventBus<OnGameWin>.Register(_gameWinBinding);
    }

    private void UnbindEvents()
    {
        EventBus<OnStartAltarActivation>.Unregister(m_OnStartAltarActivationBinding);
        EventBus<OnWaveClearedEvent>.Unregister(m_OnWaveClearedBinding);
        EventBus<OnFinalAltarActivated>.Unregister(m_OnFinalAltarActivatedBinding);
        EventBus<OnPlayerDied>.Unregister(_playerDiedBinding);
        EventBus<OnGameWin>.Unregister(_gameWinBinding);

    }

    //handle events

    void HandleAltarActivation()
    {
        PlayOneShotAtPosition(AltarActivationEvent.soundEvent, transform.position);
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