using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class WinAltar : MonoBehaviour, Interactable
{

    public int m_RequiredSouls = 10;
    public int m_CurrentSouls = 0;
    public int m_SoulsPerInteraction = 5;
    public int m_AltarIndex = 0;

    [Header("Text Settigs")]
    [SerializeField]
    TextMeshProUGUI m_soulsText;

    [SerializeField]
    private float m_DistanceOffsetY = 1f;

    [SerializeField]
    private float interactTimeTween = 0.5f;

    [SerializeField] bool _canInteract = true;

    PlayerInventory m_playerInventory;

    private bool m_isActivated = false;

    [Header("Timer")]

    public ActivationAltarSettings m_AltarSettings;

    private int m_soulsRemove = 0;

    public VisualEffect m_ActivatedEffect;
    #region Events
    EventBinding<OnGameStart> m_OnGameStartBinding;
    EventBinding<OnPlayerDied> m_OnPlayerDiedBinding;
    EventBinding<OnGameWin> m_OnGameWinBinding;



    #endregion

    void Start()
    {
        BindEvents();

        Initialize();

    }

    void BindEvents()
    {
        m_OnGameStartBinding = new EventBinding<OnGameStart>(() =>
        {
            if (!m_soulsText.enabled)
                m_soulsText.enabled = true;

            _canInteract = true;
        });
        EventBus<OnGameStart>.Register(m_OnGameStartBinding);

        m_OnPlayerDiedBinding = new EventBinding<OnPlayerDied>(() =>
        {
            ResetGame();
        });
        EventBus<OnPlayerDied>.Register(m_OnPlayerDiedBinding);

        m_OnGameWinBinding = new EventBinding<OnGameWin>(() =>
        {
            ResetGame();
        });
        EventBus<OnGameWin>.Register(m_OnGameWinBinding);
    }

    UniTask UnbindEvents()
    {
        EventBus<OnGameStart>.Unregister(m_OnGameStartBinding);
        EventBus<OnPlayerDied>.Unregister(m_OnPlayerDiedBinding);
        EventBus<OnGameWin>.Unregister(m_OnGameWinBinding);

        return UniTask.CompletedTask;
    }
    #region Initialize
    void Initialize()
    {
        transform.name = "???";
        _canInteract = true;
        if (m_soulsText == null) return;

        m_soulsText.text = $"{m_CurrentSouls}/{m_RequiredSouls}";
        m_soulsText.enabled = false;

        m_ActivatedEffect.SetBool("Active", false);

        m_playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();


    }
    #endregion

    #region Trigger Events
    void OnTriggerExit(Collider collision)
    {

        if (collision.CompareTag("Player") && !m_isActivated)
        {
            DesactivatePopup();
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !m_isActivated)
        {
            ActivatePopup();
            if (m_playerInventory == null)
                m_playerInventory = other.GetComponent<PlayerInventory>();
        }
    }

    #endregion
    #region Popup
    void ActivatePopup()
    {
        if (m_isActivated) return;
        m_soulsText.enabled = true;
        m_soulsText.alpha = 1f;
        m_soulsText.transform.DOMoveY(m_soulsText.transform.position.y + m_DistanceOffsetY, interactTimeTween).SetEase(Ease.InOutSine);

        // m_soulsText.text = $"Press {StarterAssetsInputs.Instance.GetInteractAction().action.GetBindingDisplayString(0)} to interact";
        if (!CanInteract())
        {
            m_soulsText.alpha = 0.5f;
        }
    }

    void DesactivatePopup()
    {
        m_soulsText.transform.DOMoveY(m_soulsText.transform.position.y - m_DistanceOffsetY, interactTimeTween).SetEase(Ease.InOutSine);
        m_soulsText.DOFade(0f, interactTimeTween).OnComplete(() => m_soulsText.enabled = false);
    }

    #endregion
    #region Interact
    public bool CanInteract()
    {
        return (m_playerInventory.GetCurrency() >= m_RequiredSouls) && _canInteract;
        // return _canInteract;
    }

    public void Interact()
    {
        _canInteract = false;

        StartAltarActivation();
        m_isActivated = true;
        EventBus<OnStartAltarActivation>.Raise(new OnStartAltarActivation());
        // ActivatePopup()

    }


    #endregion
    #region Souls

    private void GetSouls()
    {
        if (m_playerInventory.GetCurrency() < m_RequiredSouls)
            return;

        int souls = m_RequiredSouls / 10;
        m_playerInventory.RemoveCurrency(souls);
        AddSouls(souls);
    }

    void AddSouls(int amount)
    {
        m_CurrentSouls += amount;
        m_soulsText.text = $"{m_CurrentSouls}/{m_RequiredSouls}";
        if (m_CurrentSouls >= m_RequiredSouls)
        {
            // ActivateAltar();
            // Debug.Log("Activated");
            // Debug.Log("Win");
        }
    }
    #endregion
    #region Activation
    void FinishActivation()
    {
        // m_isActivated = true;
        m_ActivatedEffect.SetBool("Active", true);
        _canInteract = false;
        EventBus<OnInteractLeaveEvent>.Raise(new OnInteractLeaveEvent());
        EventBus<OnAltarActivated>.Raise(
            new OnAltarActivated { m_AltarActivatedIndex = m_AltarIndex });

        EventBus<OnWaveClearedEvent>.Raise(new OnWaveClearedEvent());

        // DesactivatePopup();
    }

    void StartAltarActivation()
    {
        // m_ActivatedEffect.SetBool("Active", true);
        _canInteract = false;
        StartCoroutine(AltarActivationTransition());

    }

    IEnumerator AltarActivationTransition()
    {
        int totalSoulsToTransfer = m_RequiredSouls;
        m_soulsRemove = 0;


        float elapsed = 0f;
        float _duration = m_AltarSettings.activationDuration - 1f;


        while (elapsed < _duration)
        {
            float _timeProgress = elapsed / _duration;

            float _attenuation = m_AltarSettings.altarActivationCurve.Evaluate(_timeProgress);

            int targetSoulsRemoved = Mathf.CeilToInt(totalSoulsToTransfer * _attenuation);

            int soulsToRemoveThisFrame = targetSoulsRemoved - m_soulsRemove;
            if (soulsToRemoveThisFrame > 0)
            {
                m_playerInventory.RemoveCurrency(soulsToRemoveThisFrame);
                m_soulsRemove = targetSoulsRemoved;
            }


            float lerpSouls = Mathf.Lerp(m_CurrentSouls, m_RequiredSouls, _attenuation);


            int _souls = Mathf.CeilToInt(lerpSouls);

            // m_playerInventory.RemoveCurrency(1);
            // AddSouls(_souls);

            m_soulsText.text = $"{_souls}/{m_RequiredSouls}";


            elapsed += Time.deltaTime;
            yield return null;
        }

        int remainSoulsToCleanUp = totalSoulsToTransfer - m_soulsRemove;
        if (remainSoulsToCleanUp > 0)
            m_playerInventory.RemoveCurrency(remainSoulsToCleanUp);


        FinishActivation();


    }
    #endregion

    public async void ResetGame()
    {
        m_CurrentSouls = 0;
        m_soulsText.text = $"{m_CurrentSouls}/{m_RequiredSouls}";
        _canInteract = false;
        if (m_AltarIndex == 0) { return; }

        await UnbindEvents();

        Debug.Log($"Destroying Altar {m_AltarIndex}");
        Destroy(gameObject);

    }
    public InteractableType GetInteractableType()
    {
        return InteractableType.Interactable;
    }
}
