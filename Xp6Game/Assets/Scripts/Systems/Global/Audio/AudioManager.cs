using Cysharp.Threading.Tasks;
using FMODUnity;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    // [SerializeField] private AnimationEventSound[] sounds;

    private static AudioManager instance;

    public static AudioManager Instance => instance;

    public AnimationEventSound AltarActivationEvent;

    public bool m_DebugEvent;

    //Events
    EventBinding<OnStartAltarActivation> m_OnStartAltarActivationBinding;


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
    }

    private void UnbindEvents()
    {
        EventBus<OnStartAltarActivation>.Unregister(m_OnStartAltarActivationBinding);
    }

    //handle events

    void HandleAltarActivation()
    {
        Debug.Log("Altar Activated Sound event");
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