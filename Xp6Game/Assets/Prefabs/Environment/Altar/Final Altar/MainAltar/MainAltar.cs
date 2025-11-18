using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;

public class MainAltar : MonoBehaviour, Interactable
{
    public bool m_isActivated;

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

    [Header("VFX")]
    public VisualEffect m_MainAltar;
    public VisualEffect[] m_MiniAltares;


    PlayerInventory m_playerInventory;


    private bool m_isFinalForm = false;


    #region Events
    EventBinding<OnGameStart> m_OnGameStartBinding;
    EventBinding<OnPlayerDied> m_OnPlayerDiedBinding;
    EventBinding<OnGameWin> m_OnGameWinBinding;

    EventBinding<OnAltarActivated> m_OnNewAltarActivated;



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

            m_soulsText.gameObject.SetActive(true);
            if (!m_soulsText.enabled)
            {
                m_soulsText.enabled = true;
            }
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

        m_OnNewAltarActivated = new EventBinding<OnAltarActivated>(HandleNewAltarActivation);
        EventBus<OnAltarActivated>.Register(m_OnNewAltarActivated);
    }



    UniTask UnbindEvents()
    {
        EventBus<OnGameStart>.Unregister(m_OnGameStartBinding);
        EventBus<OnPlayerDied>.Unregister(m_OnPlayerDiedBinding);
        EventBus<OnGameWin>.Unregister(m_OnGameWinBinding);
        EventBus<OnAltarActivated>.Unregister(m_OnNewAltarActivated);

        return UniTask.CompletedTask;
    }

    void Initialize()
    {
        transform.name = "Altar";
        _canInteract = true;
        if (m_soulsText == null) return;

        m_soulsText.gameObject.SetActive(true);
        m_soulsText.text = $"{m_CurrentSouls}/{m_RequiredSouls}";
        m_soulsText.enabled = false;

        foreach (VisualEffect _miniAltar in m_MiniAltares)
        {
            _miniAltar.SetBool("Active", false);
        }

        m_MainAltar.SetBool("Active", false);

        m_isFinalForm = false;

        //Find player inventory
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        m_playerInventory = player.GetComponent<PlayerInventory>();

    }

    void OnTriggerExit(Collider collision)
    {

        if (m_isActivated) return;
        if (collision.CompareTag("Player"))
        {
            DesactivatePopup();
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (m_isActivated) return;
        if (other.CompareTag("Player"))
        {
            ActivatePopup();
            if (m_playerInventory == null)
                m_playerInventory = other.GetComponent<PlayerInventory>();
        }
    }
    void OnTriggerStay(Collider other)
    {

        if (m_playerInventory == null)
            m_playerInventory = other.GetComponent<PlayerInventory>();
    }

    private void HandleNewAltarActivation(OnAltarActivated arg0)
    {
        ActivateMiniAltar(arg0.m_AltarActivatedIndex);


        if (arg0.m_AltarActivatedIndex == 3)
        {
            _canInteract = true;
            m_soulsText.gameObject.SetActive(true);
            m_soulsText.enabled = true;
            m_soulsText.alpha = 1f;
            m_isFinalForm = true;
            m_soulsText.text = "...";
        }
    }
    void ActivatePopup()
    {
        if (!CanInteract()) return;


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
    public bool CanInteract()
    {
        return _canInteract;
    }

    public void Interact()
    {
        // Debug.Log("Player Interacted with Win Altar");
        // _canInteract = false;
        if (m_isFinalForm)
        {
            EventBus<OnFinalAltarActivated>.Raise(new OnFinalAltarActivated());
            EventBus<OnDisplayMessage>.Raise(new OnDisplayMessage { m_Message = "!" });

            m_MainAltar.SetBool("Active", true);
            EventBus<OnGameWin>.Raise(new OnGameWin());
            return;
            _canInteract = false;
            m_soulsText.gameObject.SetActive(false);
            return;
        }
        if (m_playerInventory.GetCurrency() >= m_SoulsPerInteraction)
            m_playerInventory.RemoveCurrency(m_SoulsPerInteraction);
        else
            return;
        AddSouls(m_SoulsPerInteraction);
        // GameManager.Instance.WinGame();
    }


    void AddSouls(int amount)
    {
        m_CurrentSouls += amount;
        m_soulsText.text = $"{m_CurrentSouls}/{m_RequiredSouls}";
        if (m_CurrentSouls >= m_RequiredSouls)
        {
            ActivateAltar();

        }
    }

    void ActivateAltar()
    {
        // m_MainAltar.SetBool("Active", true);
        // ActivateMiniAltar(0);

        EventBus<OnInteractLeaveEvent>.Raise(new OnInteractLeaveEvent());
        EventBus<OnAltarActivated>.Raise(
            new OnAltarActivated { m_AltarActivatedIndex = m_AltarIndex });

        m_isActivated = true;
        _canInteract = false;

        m_soulsText.gameObject.SetActive(false);

    }

    void ActivateMiniAltar(int index)
    {
        m_MiniAltares[index].SetBool("Active", true);
    }

    public async void ResetGame()
    {
        m_CurrentSouls = 0;
        m_soulsText.text = $"{m_CurrentSouls}/{m_RequiredSouls}";
        _canInteract = false;

        m_soulsText.gameObject.SetActive(false);

        m_isFinalForm = false;




    }
    public InteractableType GetInteractableType()
    {
        return InteractableType.Interactable;
    }
}
