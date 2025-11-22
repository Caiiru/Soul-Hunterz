using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

public class GlobalVolumeController : MonoBehaviour
{
    [Header("Settings")]
    public ActivationAltarSettings altarSettings;
    public VolumeSettings volumeSettings;

    [Header("Values")]
    public int m_PlayerHealthLimit = 30;
    private int m_PlayerCurrentHealth = 0;

    [Header("Volumes")]
    public Volume LowHealthVolume;
    public Volume AltarActivatedVolume;
    public Volume WaveClearedVolume;

    //Events

    EventBinding<OnPlayerTakeDamage> m_OnTakeDamageBinding;

    EventBinding<OnStartAltarActivation> m_OnAltarActivatedBinding;
    EventBinding<OnWaveClearedEvent> m_OnWaveClearedBinding;

    void Start()
    {
        BindEvents();
        Debug.Log("Global Volume Controller Stated");
    }
    void BindEvents()
    {
        //Take Damage
        m_OnTakeDamageBinding = new EventBinding<OnPlayerTakeDamage>(HandlePlayerTakeDamage);
        EventBus<OnPlayerTakeDamage>.Register(m_OnTakeDamageBinding);

        //Altar
        m_OnAltarActivatedBinding = new EventBinding<OnStartAltarActivation>(HandleAltarActivation);
        EventBus<OnStartAltarActivation>.Register(m_OnAltarActivatedBinding);

        //Wave Cleared
        m_OnWaveClearedBinding = new EventBinding<OnWaveClearedEvent>(async () => await HandleWaveCleared());
        EventBus<OnWaveClearedEvent>.Register(m_OnWaveClearedBinding);


    }

    #region Events
    void HandlePlayerTakeDamage(OnPlayerTakeDamage eventData)
    {
        m_PlayerCurrentHealth = eventData.currentHealth;
        if (m_PlayerCurrentHealth <= m_PlayerHealthLimit && LowHealthVolume.weight == 0)
        {

            DesactivateAllWeights();
            StartCoroutine(DoLerpToOne(LowHealthVolume, volumeSettings.LerpDuration));
            return;
        }
        if (m_PlayerCurrentHealth > m_PlayerHealthLimit && LowHealthVolume.weight != 0)
        {
            StartCoroutine(DoLerpToZero(LowHealthVolume, volumeSettings.LerpDuration));
        }



    }

    void HandleAltarActivation()
    {
        DesactivateAllWeights();
        if (AltarActivatedVolume.weight != 1)
        {
            StartCoroutine(DoLerpToOne(AltarActivatedVolume, altarSettings.activationDuration / 2));
            return;
        }

    }

    async UniTask HandleWaveCleared()
    {
        DesactivateAllWeights();

        int delay = 1;
        if (AltarActivatedVolume.weight != 0)
        {
            StartCoroutine(DoLerpToZero(AltarActivatedVolume, delay));
        }
        await UniTask.Delay(delay * 1000);


        if (WaveClearedVolume.weight == 0)
        {
            StartCoroutine(DoLerpToOne(WaveClearedVolume, volumeSettings.LerpDuration));
        }

        await UniTask.Delay(delay * 1000);

        StartCoroutine(DoLerpToZero(WaveClearedVolume, volumeSettings.LerpDuration));

        await UniTask.Delay(delay * 1000);

        if (m_PlayerCurrentHealth <= m_PlayerHealthLimit)
        {

            StartCoroutine(DoLerpToZero(LowHealthVolume, volumeSettings.LerpDuration));
        }



    }

    #endregion

    IEnumerator DoLerpToOne(Volume targetVolume, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float _timeProgress = elapsed / duration;

            // Debug.Log(_timeProgress);

            targetVolume.weight += _timeProgress;

            elapsed += Time.deltaTime;
            yield return null;

        }
    }

    IEnumerator DoLerpToZero(Volume targetVolume, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float _timeProgress = elapsed / duration;

            targetVolume.weight -= _timeProgress;

            elapsed += Time.deltaTime;
            yield return null;

        }
    }

    void DesactivateAllWeights()
    {
        LowHealthVolume.weight = 0;
        AltarActivatedVolume.weight = 0;
        WaveClearedVolume.weight = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
