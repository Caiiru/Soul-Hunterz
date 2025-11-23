using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    #region Singleton
    private static PlayerHUD _instance;
    public static PlayerHUD Instance => _instance;
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
    }

    #endregion

    [Header("Settings")]
    public const float k_FadeDelay = 2.5f;

    [Header("Interact")]
    [SerializeField]
    Transform _interactTransform;
    bool _isHovering = false;
    public TextMeshProUGUI _interactText;

    [Header("Message")]
    [SerializeField]
    Transform _messageTransform;
    bool _isShowingMessage = false;
    public TextMeshProUGUI _messageText;

    [Header("Inventory")]
    [SerializeField]
    Transform _inventoryTransform;
    [SerializeField] Transform _dropZone;

    [Header("Player Health")]
    //Player Health
    public bool m_isHealthTextActivated = false;
    public Transform m_PlayerHealthCanvas;
    private int m_currentHealth;
    private int m_maxHealth;
    public TextMeshProUGUI m_playerHealthText;

    //Player Health Image
    public bool m_isHealthImageActivated = false;
    public Image m_playerHealthImage;
    public Transform m_PlayerHealthImageTransform;



    [Header("Ammo")]
    public GameObject m_ammoVisualHolder;
    public TextMeshProUGUI m_ammoText;
    private int m_currentAmmo;
    private int m_maxAmmo;

    [Header("Current Weapon")]
    public Image m_currentWeaponImage;

    [Header("Currency")]
    public TextMeshProUGUI m_playerCurrencyText;
    public const float k_currencyHideDelay = 5;

    [Header("Backpack")]
    private bool m_isTutorial = true;
    public Transform m_backpackVisualHolder;
    public Transform m_backpackLight;

    [Header("Map")]
    public MapsData[] m_MapData;


    //Message Queue
    Queue<String> m_messageQueue = new Queue<String>();

    //Token

    CancellationTokenSource m_HideCurrencyTk;

    CancellationTokenSource m_HideHealthTk;


    // Events
    #region Events
    EventBinding<OnGameReadyToStart> m_OnGameReadyToStart;

    EventBinding<OnGameStart> m_OnGameStartBinding;
    EventBinding<OnGameWin> m_OnGameWinBinding;
    EventBinding<OnPlayerDied> m_OnPlayerDiedBinding;

    EventBinding<OnInteractEnterEvent> m_OnInteractEnterBinding;
    EventBinding<OnInteractUpdateEvent> m_OnInteractUpdateBinding;
    EventBinding<OnInteractLeaveEvent> m_OnInteractLeaveBinding;

    EventBinding<OnInventoryInputEvent> m_OnInventoryInputBinding;

    EventBinding<OnSetPlayerHealthEvent> m_OnSetHealthEvent;
    EventBinding<OnPlayerTakeDamage> m_OnPlayerTakeDamage;
    EventBinding<OnAmmoChanged> m_OnAmmoChangedBinding;
    EventBinding<OnUpdateSouls> m_OnUpdateSouls;

    EventBinding<OnMapCollected> m_OnMapCollectedBinding;
    //Message HUD
    EventBinding<OnDisplayMessage> m_OnDisplayMessageBinding;

    //Player State
    EventBinding<OnPlayerChangeState> m_OnPlayerStateChangedBinding;
    //
    EventBinding<OnAltarActivated> m_OnAltarActivatedBinding;


    #endregion


#if ENABLE_INPUT_SYSTEM
    InputAction _interactInput;
    InputAction _inventoryInput;
#endif

    [Header("Debug")]

    public bool m_isDebug = false;
    void Start()
    {
        if (m_isDebug)
        {
            BindObjects();
            BindEvents();
            Initialize();
            this.m_currentHealth = 100;
            this.m_maxHealth = 100;
            UpdatePlayerHealthText();
            DesactivateAll();
            return;
        }
        BindEvents();
        DesactivateAll();



    }
    void BindObjects()
    {
        _interactTransform.gameObject.SetActive(false);

        //Text
        if (_interactText == null)
        {
            _interactText = _interactTransform.GetComponentInChildren<TextMeshProUGUI>();
        }

        if (!_dropZone)
        {
            _dropZone = _inventoryTransform.Find("DropZone");

        }
        _interactInput = StarterAssetsInputs.Instance.GetInteractAction().action;

        _messageTransform.gameObject.SetActive(false);
        _messageText = _messageTransform.GetComponentInChildren<TextMeshProUGUI>();




    }

    #region Bind Events
    void BindEvents()
    {
        //Game Start
        m_OnGameReadyToStart = new EventBinding<OnGameReadyToStart>(HandleGameReadyToStart);
        EventBus<OnGameReadyToStart>.Register(m_OnGameReadyToStart);
        m_OnGameStartBinding = new EventBinding<OnGameStart>(HandleGameStart);
        EventBus<OnGameStart>.Register(m_OnGameStartBinding);
        //Game Win
        m_OnGameWinBinding = new EventBinding<OnGameWin>(() =>
        {
            DesactivateAll();
        });
        EventBus<OnGameWin>.Register(m_OnGameWinBinding);
        //Game Lose
        m_OnPlayerDiedBinding = new EventBinding<OnPlayerDied>(() =>
        {
            DesactivateAll();
        });
        EventBus<OnPlayerDied>.Register(m_OnPlayerDiedBinding);


        //Enter & Leave Interactable Zone
        m_OnInteractEnterBinding = new EventBinding<OnInteractEnterEvent>(OnInteractEnter);
        EventBus<OnInteractEnterEvent>.Register(m_OnInteractEnterBinding);
        m_OnInteractUpdateBinding = new EventBinding<OnInteractUpdateEvent>(OnInteractUpdate);
        EventBus<OnInteractUpdateEvent>.Register(m_OnInteractUpdateBinding);

        m_OnInteractLeaveBinding = new EventBinding<OnInteractLeaveEvent>(OnInteractLeave);
        EventBus<OnInteractLeaveEvent>.Register(m_OnInteractLeaveBinding);

        //Inventory INput
        m_OnInventoryInputBinding = new EventBinding<OnInventoryInputEvent>(InventoryToggle);
        EventBus<OnInventoryInputEvent>.Register(m_OnInventoryInputBinding);
        //Take Damage & Set Health
        m_OnPlayerTakeDamage = new EventBinding<OnPlayerTakeDamage>(HandlePlayerTakeDamage);
        EventBus<OnPlayerTakeDamage>.Register(m_OnPlayerTakeDamage);

        m_OnSetHealthEvent = new EventBinding<OnSetPlayerHealthEvent>((eventData) =>
        {
            m_currentHealth = eventData.currentHealth;
            m_maxHealth = eventData.maxHealth;
            UpdatePlayerHealthText();
            UpdatePlayerHealthImage();

        });
        EventBus<OnSetPlayerHealthEvent>.Register(m_OnSetHealthEvent);

        //Update Currency
        m_OnUpdateSouls = new EventBinding<OnUpdateSouls>(async (data) =>
        {
            // Debug.Log("Update Souls Event Called");
            await HandleUpdateCurrencyEvent(data);
        });
        EventBus<OnUpdateSouls>.Register(m_OnUpdateSouls);

        //Set Message

        EventBus<OnAltarActivated>.Register(new EventBinding<OnAltarActivated>(HandleMessageAltarActivated));

        m_OnDisplayMessageBinding = new EventBinding<OnDisplayMessage>(HandleDisplayMessage);
        EventBus<OnDisplayMessage>.Register(m_OnDisplayMessageBinding);

        //Reset

        EventBus<OnPlayerDied>.Register(new EventBinding<OnPlayerDied>(() =>
        {
            DesactivateAll();
        }));
        EventBus<OnGameWin>.Register(new EventBinding<OnGameWin>(() =>
        {
            DesactivateAll();

        }));
        m_OnAmmoChangedBinding = new EventBinding<OnAmmoChanged>(HandleAmmoChanged);
        EventBus<OnAmmoChanged>.Register(m_OnAmmoChangedBinding);

        //MAP

        m_OnMapCollectedBinding = new EventBinding<OnMapCollected>(HandleMapCollected);
        EventBus<OnMapCollected>.Register(m_OnMapCollectedBinding);

        EventBus<OnTutorialFinished>.Register(new EventBinding<OnTutorialFinished>(() =>
        {
            // m_isTutorial = false;

            m_backpackLight.gameObject.SetActive(false);
        }));

        m_OnAltarActivatedBinding = new EventBinding<OnAltarActivated>((data) =>
        {
            foreach (var map in m_MapData)
            {
                if (map.direction == data.m_Direction)
                {
                    map.activatedIcon.gameObject.SetActive(true);
                    map.altarIcon.gameObject.SetActive(false);

                }

            }

        });

        EventBus<OnAltarActivated>.Register(m_OnAltarActivatedBinding);

        //Change State
        //
        m_OnPlayerStateChangedBinding = new EventBinding<OnPlayerChangeState>(HandlePlayerStateChanged);
        EventBus<OnPlayerChangeState>.Register(m_OnPlayerStateChangedBinding);
    }


    void UnbindEvents()
    {
        EventBus<OnGameReadyToStart>.Unregister(m_OnGameReadyToStart);
        EventBus<OnInventoryInputEvent>.Unregister(m_OnInventoryInputBinding);
        EventBus<OnInteractEnterEvent>.Unregister(m_OnInteractEnterBinding);
        EventBus<OnInteractUpdateEvent>.Unregister(m_OnInteractUpdateBinding);
        EventBus<OnInteractLeaveEvent>.Unregister(m_OnInteractLeaveBinding);
        EventBus<OnPlayerTakeDamage>.Unregister(m_OnPlayerTakeDamage);
        EventBus<OnSetPlayerHealthEvent>.Unregister(m_OnSetHealthEvent);
        EventBus<OnUpdateSouls>.Unregister(m_OnUpdateSouls);
        EventBus<OnAmmoChanged>.Unregister(m_OnAmmoChangedBinding);
        EventBus<OnDisplayMessage>.Unregister(m_OnDisplayMessageBinding);
    }

    #endregion
    void Initialize()
    {
        BoxCollider2D _dropZoneCollider = _dropZone.GetComponent<BoxCollider2D>();
        _dropZoneCollider.size = _dropZone.GetComponent<RectTransform>().rect.size;

        // ActivateAll();
        _interactTransform.gameObject.SetActive(false);
        EventBus<OnInventoryInputEvent>.Raise(new OnInventoryInputEvent { isOpen = false });


    }
    #region Event Handlers

    private void HandleGameReadyToStart()
    {
        BindObjects();
        Initialize();
        _interactTransform.gameObject.SetActive(false);
        GetComponent<Canvas>().enabled = false;


    }
    private void HandlePlayerTakeDamage(OnPlayerTakeDamage arg0)
    {
        m_currentHealth -= arg0.value;
        UpdatePlayerHealthText();
        UpdatePlayerHealthImage();

    }
    private void UpdatePlayerHealthText()
    {
        ShowHealth();
        m_playerHealthText.text = $"Health: {m_currentHealth}/{m_maxHealth}";
    }
    private void UpdatePlayerHealthImage()
    {
        ShowHealth();
        m_playerHealthImage.fillAmount = (float)m_currentHealth / (float)m_maxHealth;
    }


    private async UniTask HandleUpdateCurrencyEvent(OnUpdateSouls eventData)
    {
        //TODO: Use iDisposable
        if (m_isTutorial) return;

        m_playerCurrencyText.transform.parent.gameObject.SetActive(true);



        int currentCurrency = eventData.amount;
        m_playerCurrencyText.text = $"{currentCurrency.ToString()}";

        m_HideCurrencyTk?.Cancel();

        m_HideCurrencyTk = new CancellationTokenSource();

        const float delayTime = k_currencyHideDelay * 1000;

        try
        {
            await UniTask.Delay((int)delayTime, cancellationToken: m_HideCurrencyTk.Token);

            m_playerCurrencyText.transform.parent.gameObject.SetActive(false);
        }
        catch
        {

        }
        finally
        {

        }

    }


    private void InventoryToggle(OnInventoryInputEvent eventData)
    {
        if (eventData.isOpen)
            ShowInventory();
        else
            HideInventory();

    }


    void OnInteractEnter(OnInteractEnterEvent eventData)
    {
        // if (_isHovering) return;

        string interactType = eventData.interactableType == InteractableType.Collectable ? "to collect " : "to interact with ";
        _isHovering = true;

        SetTextToInteract($"Press {_interactInput.GetBindingDisplayString(0)} {interactType}{eventData.InteractableName}");
        ShowInteractText();
        PopupInteractText();

    }
    void OnInteractUpdate(OnInteractUpdateEvent eventData)
    {
        SetTextToInteract($"Press {_interactInput.GetBindingDisplayString(0)} to interact with {eventData.InteractableName}");
    }

    void OnInteractLeave()
    {

        if (!_isHovering) return;
        HideInteractText();

    }

    private void HandlePlayerStateChanged(OnPlayerChangeState args)
    {
        if (args.newState == PlayerStates.Combat)
        {
            ShowWeapon();
            ShowHealth();
        }
        if (args.newState == PlayerStates.Exploring)
        {
            HideWeapon();
            FadeHealth();
        }
    }
    private void HandleAmmoChanged(OnAmmoChanged arg0)
    {
        ShowWeapon();
        m_currentAmmo = arg0.currentAmmo;
        m_maxAmmo = arg0.maxAmmo;
        UpdateAmmoText();


    }

    private void HandleMessageAltarActivated(OnAltarActivated arg0)
    {
        DisplayMessage("Altar has been activated!");
    }

    private void HandleDisplayMessage(OnDisplayMessage arg0)
    {
        DisplayMessage(arg0.m_Message);
    }

    void DisplayMessage(string m_text)
    {
        if (_isShowingMessage)
        {
            Debug.Log($"Enqueuing..{m_text}");
            m_messageQueue.Enqueue(m_text);
            return;
        }

        _isShowingMessage = true;
        _messageTransform.transform.localScale = Vector3.zero;
        _messageTransform.gameObject.SetActive(true);
        _messageText.alpha = 0f;
        // _messageText.transform.localScale = Vector3.zero;
        _messageText.text = $"{m_text}";
        _messageTransform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InSine).OnComplete(async () =>
        {
            // _messageText.DOText("The Altar has been activated!", 0.3f).SetEase(Ease.Linear);
            // cameraShakeManager.instance.CameraShake(new Unity.Cinemachine.CinemachineImpulseSource());
            _messageText.DOFade(1f, 0.3f).SetEase(Ease.Linear);
            await UniTask.Delay(TimeSpan.FromSeconds(3f));
            // _messageTransform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
            // {
            //     _messageTransform.gameObject.SetActive(false);
            //     _isShowingMessage = false;
            // });
            _messageText.DOFade(0f, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
            {
                _messageTransform.gameObject.SetActive(false);
                _isShowingMessage = false;
            });
        });

        if (m_messageQueue.Count > 0)
            DisplayMessage(m_messageQueue.Dequeue());
    }

    private void HandleGameStart(OnGameStart arg0)
    {
        GetComponent<Canvas>().enabled = true;

        //Desactivate All maps
        // ActivateAll();
        DesactivateSoulsMap();
    }

    void DesactivateSoulsMap()
    {
        foreach (var _data in m_MapData)
        {
            _data.activatedIcon.gameObject.SetActive(false);

        }
    }
    private void HandleMapCollected()
    {
        m_isTutorial = false;
        m_backpackVisualHolder.transform.localScale = Vector3.zero;
        m_backpackVisualHolder.gameObject.SetActive(true);
        m_backpackVisualHolder.transform.DOScale(1f, 1.5f).SetEase(Ease.OutBounce);

        m_backpackLight.transform.localScale = Vector3.zero;
        m_backpackLight.gameObject.SetActive(true);
        m_backpackLight.transform.DOScale(1f, 1.5f).SetEase(Ease.OutBounce);

        m_backpackLight.GetComponent<Animator>().SetTrigger("InventoryEvent");

    }

    #endregion

    #region Ammo Methods
    void UpdateAmmoText()
    {
        m_ammoText.text = $"{m_currentAmmo}/{m_maxAmmo}";
    }


    #endregion
    #region Interact Methods
    void ShowInteractText()
    {
        _interactTransform.gameObject.SetActive(true);
    }
    void SetTextToInteract(string text)
    {
        _interactText.text = text;

    }
    void PopupInteractText()
    {
        if (!_interactTransform.gameObject.activeSelf)
            _interactTransform.gameObject.SetActive(true);
        _interactText.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
       {
           // _interactTransform.DOScale(Vector3.one - new Vector3(0.1f, 0.1f, 0.1f), 0.5f).SetEase(Ease.InSine).Flip();
           // _interactTransform.transform.localScale = Vector3.one;
       });
    }

    void HideInteractText()
    {
        _interactText.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.Flash).OnComplete(() =>
        {
            _isHovering = false;
            _interactTransform.gameObject.SetActive(false);
        });
    }
    #endregion
    #region Inventory Methods

    void ShowInventory()
    {
        m_backpackVisualHolder.gameObject.SetActive(false);
        _inventoryTransform.GetComponent<Canvas>().enabled = true;
    }
    void HideInventory()
    {
        m_backpackVisualHolder.gameObject.SetActive(true);
        _inventoryTransform.GetComponent<Canvas>().enabled = false;

    }
    #endregion


    #region Desactivate & Activate
    void ShowHealth()
    {
        m_PlayerHealthImageTransform.gameObject.SetActive(true);
        for (int i = 0; i < m_PlayerHealthImageTransform.childCount; i++)
        {
            m_PlayerHealthImageTransform.GetChild(i).GetComponent<Image>().DOFade(255, 1f);
        }
    }

    void FadeHealth()
    {

        for (int i = 0; i < m_PlayerHealthImageTransform.childCount; i++)
        {
            m_PlayerHealthImageTransform.GetChild(i).GetComponent<Image>().DOFade(0, k_FadeDelay).SetEase(Ease.Linear);
        }
    }

    void ShowWeapon()
    {
        float m_fadeTime = 1f;
        m_ammoVisualHolder.gameObject.SetActive(true);
        m_ammoText.DOFade(255, m_fadeTime).SetEase(Ease.Linear);
        m_currentWeaponImage.DOFade(255, m_fadeTime).SetEase(Ease.Linear);
    }

    void HideWeapon()
    {
        m_ammoText.DOFade(0, k_FadeDelay).SetEase(Ease.InSine);
        m_currentWeaponImage.DOColor(new Color(255, 255, 255, 0), k_FadeDelay);



    }

    void DesactivateAll()
    {
        // Debug.Log("Desactivating all");
        m_playerCurrencyText.transform.parent.gameObject.SetActive(false);
        _interactTransform.gameObject.SetActive(false);
        _inventoryTransform.GetComponent<Canvas>().enabled = false;
        _messageTransform.gameObject.SetActive(false);
        m_ammoVisualHolder.SetActive(false);


        m_PlayerHealthCanvas.gameObject.SetActive(false);

        m_playerHealthText.gameObject.SetActive(false);

        m_PlayerHealthImageTransform.gameObject.SetActive(false);

        //Desactivate health image 
        for (int i = 0; i < m_PlayerHealthImageTransform.childCount; i++)
        {
            m_PlayerHealthImageTransform.GetChild(i).GetComponent<Image>().color = new Color(255, 255, 255, 0);
        }

        m_backpackVisualHolder.gameObject.SetActive(false);
    }

    void ActivateAll()
    {
        // m_ammoVisualHolder.SetActive(true);
        _messageTransform.gameObject.SetActive(false);
        // _inventoryTransform.GetComponent<Canvas>().enabled = true;

        // m_PlayerHealthImageTransform.gameObject.SetActive(true);

        // ShowHealth();
        // m_playerCurrencyText.transform.parent.gameObject.SetActive(true);

        if (!m_isTutorial)
            m_backpackVisualHolder.gameObject.SetActive(true);
    }

    #endregion

    void OnDisable()
    {
        UnbindEvents();
    }
}

[System.Serializable]
public struct MapsData
{
    public AltarDirection direction;
    public Transform altarIcon;
    public Transform activatedIcon;
}
