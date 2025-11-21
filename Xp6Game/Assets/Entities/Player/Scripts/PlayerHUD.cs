using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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

    [Header("Backpack")]
    public Transform m_backpackVisualHolder;


    //Message Queue
    Queue<String> m_messageQueue = new Queue<String>();


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
    //Message HUD
    EventBinding<OnDisplayMessage> m_OnDisplayMessageBinding;


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
        m_OnUpdateSouls = new EventBinding<OnUpdateSouls>(HandleUpdateCurrencyEvent);
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

    void Initialize()
    {
        BoxCollider2D _dropZoneCollider = _dropZone.GetComponent<BoxCollider2D>();
        _dropZoneCollider.size = _dropZone.GetComponent<RectTransform>().rect.size;

        ActivateAll();
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
        m_playerHealthText.text = $"Health: {m_currentHealth}/{m_maxHealth}";
    }
    private void UpdatePlayerHealthImage()
    {
        m_playerHealthImage.fillAmount = (float)m_currentHealth / (float)m_maxHealth;
    }


    private void HandleUpdateCurrencyEvent(OnUpdateSouls eventData)
    {
        int currentCurrency = eventData.amount;
        m_playerCurrencyText.text = $"{currentCurrency.ToString()}";
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


    private void HandleAmmoChanged(OnAmmoChanged arg0)
    {

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
        ActivateAll();
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


    void DesactivateAll()
    {
        Debug.Log("Desactivating all");
        _interactTransform.gameObject.SetActive(false);
        _inventoryTransform.GetComponent<Canvas>().enabled = false;
        m_playerCurrencyText.gameObject.SetActive(false);
        _messageTransform.gameObject.SetActive(false);
        m_ammoVisualHolder.SetActive(false);

        m_PlayerHealthCanvas.gameObject.SetActive(false);
        m_PlayerHealthImageTransform.gameObject.SetActive(false);

        m_playerCurrencyText.transform.parent.gameObject.SetActive(false);


        m_backpackVisualHolder.gameObject.SetActive(false);
    }

    void ActivateAll()
    {
        Debug.Log("Activating All");
        m_ammoVisualHolder.SetActive(true);
        _messageTransform.gameObject.SetActive(false);
        // _inventoryTransform.GetComponent<Canvas>().enabled = true;
        if (m_isHealthTextActivated)
            m_PlayerHealthCanvas.gameObject.SetActive(true);

        if (m_isHealthImageActivated)
            m_PlayerHealthImageTransform.gameObject.SetActive(true);


        m_playerCurrencyText.gameObject.SetActive(true);
        m_PlayerHealthImageTransform.gameObject.SetActive(true);

        m_playerCurrencyText.transform.parent.gameObject.SetActive(true);

        m_backpackVisualHolder.gameObject.SetActive(true);
    }

    #endregion

    void OnDisable()
    {
        UnbindEvents();
    }
}
