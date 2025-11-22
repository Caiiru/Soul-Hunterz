using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    [Header("Staten Driven")]
    public CinemachineStateDrivenCamera m_StatenDrivenCam;

    [SerializeField] CinemachineCamera m_CinemachineCamera;
    [SerializeField] Transform m_DrivenCameraTransform;

    //

    Vector3 m_StartPosition;
    Quaternion m_StartRotation;

    //Menu Camera
    Vector3 m_MenuCamStartPosition;
    Quaternion m_MenuCamStartRotation;
    [SerializeField] GameObject m_MenuCam;

    //Altar sequence camera
    [Header("Main Menu Perlin")]
    public CinemachineBasicMultiChannelPerlin m_MenuPerlin;

    [Header("Altar Screen Shake Settings")]
    public ActivationAltarSettings m_ActivationAltarSettings;
    public bool m_isShaking = false;


    //Events
    EventBinding<OnGameStart> m_OnGameStart;
    EventBinding<OnGameOver> m_OnGameOver;
    EventBinding<OnGameWin> m_OnGameWin;

    EventBinding<OnStartAltarActivation> m_OnAltarActivated;
    EventBinding<OnAltarActivated> m_OnAltarEndedActivation;

    //Tutorial

    EventBinding<OnMapCollected> m_OnMapCollectedBinding;





    public bool m_isDebug = false;
    void Start()
    {
        if (m_isDebug)
        {
            m_CinemachineCamera = this.GetComponentInChildren<CinemachineCamera>();

            m_CinemachineCamera.Target.TrackingTarget = GameObject.FindWithTag("Player").transform;
        }


        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;

        m_MenuCam = m_DrivenCameraTransform.Find("mainMenuCam").gameObject;
        if (m_MenuCam)
        {
            m_MenuCamStartPosition = m_MenuCam.transform.position;
            m_MenuCamStartRotation = m_MenuCam.transform.rotation;
        }

        BindEvents();
        m_isShaking = false;


    }

    void BindEvents()
    {
        m_OnGameStart = new EventBinding<OnGameStart>(HandleGameReadyToStart);
        EventBus<OnGameStart>.Register(m_OnGameStart);

        // m_OnGameOver = new EventBinding<OnGameOver>(HandleGameOver);
        // EventBus<OnGameOver>.Register(m_OnGameOver);

        // m_OnGameWin = new EventBinding<OnGameWin>(HandleGameOver);
        // EventBus<OnGameWin>.Register(m_OnGameWin);

        m_OnAltarActivated = new EventBinding<OnStartAltarActivation>(HandleAltarActivation);
        EventBus<OnStartAltarActivation>.Register(m_OnAltarActivated);

        m_OnAltarEndedActivation = new EventBinding<OnAltarActivated>(async () =>
        {
            await HandleAltarEndedActivation();
        });
        EventBus<OnAltarActivated>.Register(m_OnAltarEndedActivation);


        m_OnMapCollectedBinding = new EventBinding<OnMapCollected>(async () =>
        {
            await HandleTutorialShake(m_ActivationAltarSettings.tutorialShakeDuration);
        });
        EventBus<OnMapCollected>.Register(m_OnMapCollectedBinding);



    }

    private async UniTask HandleTutorialShake(int duration)
    {
        StartCoroutine(DoScreenShake(1, 1, duration, m_ActivationAltarSettings.tutorialCurve));
        await UniTask.Delay(duration * 1000);
        await HandleAltarEndedActivation();
    }

    private async UniTask HandleAltarEndedActivation()
    {
        // Atraso inicial (se necessário)
        await UniTask.Delay(1000);
        float m_perlinFadeOutDuration = 1;

        // Supondo que você precisa encontrar o componente Perlin antes de interagir com o Target.
        m_CinemachineCamera = this.GetComponentInChildren<CinemachineCamera>();

        if (m_CinemachineCamera != null)
        {
            // Encontra o alvo do jogador para o Tracking
            Transform playerTarget = GameObject.FindWithTag("Player")?.transform;

            if (playerTarget != null)
            {
                // Atualiza o Target do Cinemachine (se o CinemachineCamera tiver essa propriedade)
                // Nota: Se CinemachineCamera for um script customizado, use VCam.Follow ou VCam.LookAt
                // m_CinemachineCamera.Target.TrackingTarget = playerTarget; 
            }
        }

        // --- Parte de Transição do Perlin ---

        // Lista para armazenar todos os componentes Perlin que precisam de Fade-Out
        var perlinsToFade = new System.Collections.Generic.List<CinemachineBasicMultiChannelPerlin>();

        for (int i = 0; i < m_DrivenCameraTransform.childCount; i++)
        {
            if (m_DrivenCameraTransform.GetChild(i).TryGetComponent<CinemachineBasicMultiChannelPerlin>(out var perlin))
            {
                if (perlin != null && perlin != m_MenuPerlin)
                {
                    // Guarda o componente para o loop de Lerp
                    perlinsToFade.Add(perlin);

                    // Opcional: ReSeed aqui para garantir que o padrão de ruído não seja repetitivo
                    perlin.ReSeed();
                }
            }
        }

        // Se houver Perlin Noise para atenuar, inicia o Fade-Out
        if (perlinsToFade.Count > 0)
        {
            float elapsed = 0f;

            // Loop principal de atenuação (Fade-Out)
            while (elapsed < m_perlinFadeOutDuration)
            {
                // Calcula o progresso (t) de 0 a 1
                float t = elapsed / m_perlinFadeOutDuration;

                // Opção 1: Lerp Simples (Linear)
                // float fadeValue = Mathf.Lerp(1f, 0f, t); 

                // Opção 2: Lerp com Ease-Out (Mais suave ao final)
                // Este método de Lerp garante que a atenuação comece na força total (1) e termine suavemente (0).
                float fadeValue = 1f - t; // Ou use Mathf.SmoothStep(0f, 1f, t) para um ease mais acentuado

                foreach (var perlin in perlinsToFade)
                {
                    // Interpola o AmplitudeGain de 1.0 (ou o valor atual) para 0, seguindo o tempo.
                    // Usamos o 'fadeValue' para atenuar a amplitude original para 0.
                    perlin.AmplitudeGain *= fadeValue; // Se o valor original era 1.0, agora vai de 1.0 -> 0.0
                    perlin.FrequencyGain *= fadeValue;
                }

                elapsed += Time.deltaTime;
                await UniTask.Yield(); // Espera o próximo frame (equivalente a yield return null)
            }

            // Garante que o Ganho seja exatamente zero no final (limpeza)
            foreach (var perlin in perlinsToFade)
            {
                perlin.AmplitudeGain = 0f;
                perlin.FrequencyGain = 0f;
            }
        }
    }

    private void HandleAltarActivation()
    {
        StartCoroutine(DoScreenShake(1, 1, m_ActivationAltarSettings.activationDuration, m_ActivationAltarSettings.shakeCurve));
        // m_CinemachineCamera = this.GetComponentInChildren<CinemachineCamera>();

        // m_CinemachineCamera.Target.TrackingTarget = GameObject.FindWithTag("Player").transform;

        // for (int i = 0; i < m_DrivenCameraTransform.childCount; i++)
        // {
        //     // m_DrivenCameraTransform.GetChild(i).GetComponent<CinemachineCamera>().Target.TrackingTarget = m_CinemachineCamera.Target.TrackingTarget;
        //     m_DrivenCameraTransform.GetChild(i).TryGetComponent<CinemachineBasicMultiChannelPerlin>(out var perlin);
        //     if (perlin == null || perlin == m_MenuPerlin) continue;


        //     perlin.AmplitudeGain = 1;
        //     perlin.FrequencyGain = 1;

        // }
    }

    private IEnumerator DoScreenShake(float baseAmplitude, float baseFrequency, float duration, AnimationCurve curve)
    {
        m_isShaking = true;
        float elapsed = 0;

        m_CinemachineCamera = this.GetComponentInChildren<CinemachineCamera>();

        m_CinemachineCamera.Target.TrackingTarget = GameObject.FindWithTag("Player").transform;


        while (elapsed < duration)
        {
            float _timeProgress = elapsed / duration;

            float _attenuation = curve.Evaluate(_timeProgress);


            for (int i = 0; i < m_DrivenCameraTransform.childCount; i++)
            {
                // m_DrivenCameraTransform.GetChild(i).GetComponent<CinemachineCamera>().Target.TrackingTarget = m_CinemachineCamera.Target.TrackingTarget;
                m_DrivenCameraTransform.GetChild(i).TryGetComponent<CinemachineBasicMultiChannelPerlin>(out var perlin);
                if (perlin == null || perlin == m_MenuPerlin) continue;


                perlin.AmplitudeGain = baseAmplitude * _attenuation *
                    m_ActivationAltarSettings.shakeIntensityMultiplier;

                perlin.FrequencyGain = baseFrequency * _attenuation *
                    m_ActivationAltarSettings.shakeIntensityMultiplier;



            }

            elapsed += Time.deltaTime;
            yield return null;
        }
        m_isShaking = false;

    }


    private void HandleGameOver()
    {
        for (int i = 0; i < m_DrivenCameraTransform.childCount; i++)
        {
            m_DrivenCameraTransform.GetChild(i).GetComponent<CinemachineCamera>().Target.TrackingTarget = null;
        }
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        if (m_MenuCam)
        {
            m_MenuCam.transform.position = m_MenuCamStartPosition;
            m_MenuCam.transform.rotation = m_MenuCamStartRotation;
        }

    }

    private void HandleGameReadyToStart(OnGameStart arg0)
    {
        //     m_CinemachineCamera = this.GetComponentInChildren<CinemachineCamera>();

        //     m_CinemachineCamera.Target.TrackingTarget = GameObject.FindWithTag("Player").transform;

        //     for (int i = 0; i < m_DrivenCameraTransform.childCount; i++)
        //     {
        //         // m_DrivenCameraTransform.GetChild(i).GetComponent<CinemachineCamera>().Target.TrackingTarget = m_CinemachineCamera.Target.TrackingTarget;
        //         m_DrivenCameraTransform.GetChild(i).TryGetComponent<CinemachineBasicMultiChannelPerlin>(out var perlin);
        //         if (perlin != null)
        //         {
        //             perlin.AmplitudeGain = 1;
        //             perlin.FrequencyGain = 1;
        //         }
        //     }

        // m_SequencerCam.DOPlay(); 

    }

    // Update is called once per frame
    void Update()
    {

    }
}
