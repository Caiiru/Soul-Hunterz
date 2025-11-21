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

    private bool _canInteract = true;

    PlayerInventory m_playerInventory;

    private bool m_isActivated = false;

    [Header("Timer")]

    public float m_timerBetweenInteraction = 1f;
    public float m_timer = 1f;

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


    }
    #endregion

    #region Trigger Events
    void OnTriggerExit(Collider collision)
    {

        if (collision.CompareTag("Player"))
        {
            DesactivatePopup();
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
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
        return _canInteract;
    }

    public void Interact()
    {
        // Debug.Log("Player Interacted with Win Altar");
        // _canInteract = false;
        m_isActivated = true;
        _canInteract = false;
        EventBus<OnStartAltarActivation>.Raise(new OnStartAltarActivation());


        // GameManager.Instance.WinGame();
    }


    #endregion
    #region Update

    void Update()
    {
        if (!m_isActivated)
            return;

        HandleTimer();
        if (m_timer >= m_timerBetweenInteraction)
        {
            GetSouls();
            m_timer = 0;
        }

    }

    private void HandleTimer()
    {
        m_timer += Time.deltaTime;


    }
    #endregion
    #region Souls

    private void GetSouls()
    {
        if (m_playerInventory.GetCurrency() >= m_SoulsPerInteraction)
            m_playerInventory.RemoveCurrency(m_SoulsPerInteraction);
        else
            return;
        AddSouls(m_SoulsPerInteraction);
    }

    void AddSouls(int amount)
    {
        m_CurrentSouls += amount;
        m_soulsText.text = $"{m_CurrentSouls}/{m_RequiredSouls}";
        if (m_CurrentSouls >= m_RequiredSouls)
        {
            m_isActivated = false;
            m_ActivatedEffect.SetBool("Active", true);
            _canInteract = false;
            EventBus<OnInteractLeaveEvent>.Raise(new OnInteractLeaveEvent());
            EventBus<OnAltarActivated>.Raise(
                new OnAltarActivated { m_AltarActivatedIndex = m_AltarIndex });
            // Debug.Log("Win");
        }
    }
    #endregion
    #region Reset Game

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
    #endregion
    public InteractableType GetInteractableType()
    {
        return InteractableType.Interactable;
    }
}
