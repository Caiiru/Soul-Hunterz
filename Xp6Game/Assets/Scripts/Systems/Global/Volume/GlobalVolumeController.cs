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
    public Volume WaveVolume;
    public Volume WaveClearedVolume;

    //Events

    EventBinding<OnPlayerTakeDamage> m_OnTakeDamageBinding;

    EventBinding<OnStartAltarActivation> m_OnAltarStartedBinding;
    EventBinding<OnAltarActivated> m_OnAltarActivatedBinding;
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

        //Altar Started
        m_OnAltarStartedBinding = new EventBinding<OnStartAltarActivation>(HandleAltarStarted);
        EventBus<OnStartAltarActivation>.Register(m_OnAltarStartedBinding);

        //Wave Started
        m_OnAltarActivatedBinding = new EventBinding<OnAltarActivated>(() =>
        {
            StartCoroutine(HandleWaveStarted());
        });
        EventBus<OnAltarActivated>.Register(m_OnAltarActivatedBinding);

        EventBinding<OnFinalAltarActivated> _FinalAltarBinding = new EventBinding<OnFinalAltarActivated>(() =>
        {
            StartCoroutine(HandleFinalWave());
        });
        EventBus<OnFinalAltarActivated>.Register(_FinalAltarBinding);


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

    IEnumerator HandleFinalWave()
    {
        StartCoroutine(DoLerpToZero(AltarActivatedVolume, 1f));
        yield return new WaitForSeconds(1f);
        StartCoroutine(DoLerpToOne(WaveVolume, 2f));
    }

    void HandleAltarStarted()
    {
        DesactivateAllWeights();
        if (AltarActivatedVolume.weight != 1)
        {
            StartCoroutine(DoLerpToOne(AltarActivatedVolume, altarSettings.activationDuration / 2));
            return;
        }

    }
    IEnumerator HandleWaveStarted()
    {
        StartCoroutine(DoLerpToZero(AltarActivatedVolume, altarSettings.activationDuration / 2));
        yield return new WaitForSeconds(altarSettings.activationDuration / 2);
        StartCoroutine(DoLerpToOne(WaveVolume, volumeSettings.LerpDuration));

    }

    IEnumerator HandleWaveCleared()
    {
        // DesactivateAllWeights();
        if (AltarActivatedVolume.weight != 0)
        {
            StartCoroutine(DoLerpToZero(WaveVolume, volumeSettings.waveClearedLerpDuration));
        }
        StartCoroutine(DoLerpToOne(WaveClearedVolume, volumeSettings.waveClearedLerpDuration));


        yield return new WaitForSeconds(volumeSettings.waveClearedLerpDuration);

        StartCoroutine(DoLerpToZero(WaveClearedVolume, volumeSettings.waveClearedLerpDuration));

        yield return new WaitForSeconds(volumeSettings.waveClearedLerpDuration);
        DesactivateAllWeights();



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
        if (targetVolume.weight != 0)
            targetVolume.weight = 0;

    }

    void DesactivateAllWeights()
    {
        LowHealthVolume.weight = 0;
        AltarActivatedVolume.weight = 0;
        WaveClearedVolume.weight = 0;
        WaveVolume.weight = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
