using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("Inventory")]
    [SerializeField]
    Transform _inventoryTransform;
    [SerializeField] Transform _dropZone;

    [Header("Player Health")]
    //Player Health
    public Transform m_PlayerHealthCanvas;
    private int m_currentHealth;
    private int m_maxHealth;


    [Header("Texts")]
    TextMeshProUGUI _interactText;
    public TextMeshProUGUI m_playerHealthText;

    // Events
    EventBinding<GameReadyToStartEvent> m_OnGameReadyToStart;

    EventBinding<OnInteractEnterEvent> m_OnInteractEnterBinding;
    EventBinding<OnInteractLeaveEvent> m_OnInteractLeaveBinding;

    EventBinding<OnInventoryInputEvent> m_OnInventoryInputBinding;

    EventBinding<OnSetPlayerHealthEvent> m_OnSetHealthEvent;
    EventBinding<OnPlayerTakeDamage> m_OnPlayerTakeDamage;



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


    }

    void BindEvents()
    {
        //Interact Button / Action
        m_OnGameReadyToStart = new EventBinding<GameReadyToStartEvent>(HandleGameReadyToStart);
        EventBus<GameReadyToStartEvent>.Register(m_OnGameReadyToStart);


        m_OnInteractEnterBinding = new EventBinding<OnInteractEnterEvent>(OnInteractEnter);
        EventBus<OnInteractEnterEvent>.Register(m_OnInteractEnterBinding);
        m_OnInteractLeaveBinding = new EventBinding<OnInteractLeaveEvent>(OnInteractLeave);
        EventBus<OnInteractLeaveEvent>.Register(m_OnInteractLeaveBinding);

        m_OnInventoryInputBinding = new EventBinding<OnInventoryInputEvent>(InventoryToggle);
        EventBus<OnInventoryInputEvent>.Register(m_OnInventoryInputBinding);

        m_OnPlayerTakeDamage = new EventBinding<OnPlayerTakeDamage>(HandlePlayerTakeDamage);
        EventBus<OnPlayerTakeDamage>.Register(m_OnPlayerTakeDamage);

        m_OnSetHealthEvent = new EventBinding<OnSetPlayerHealthEvent>((eventData) =>
        {
            m_currentHealth = eventData.currentHealth;
            m_maxHealth = eventData.maxHealth;
            UpdatePlayerHealthText();

        });
        EventBus<OnSetPlayerHealthEvent>.Register(m_OnSetHealthEvent);



    }
    void UnbindEvents()
    {
        EventBus<GameReadyToStartEvent>.Unregister(m_OnGameReadyToStart);
        EventBus<OnInventoryInputEvent>.Unregister(m_OnInventoryInputBinding);
        EventBus<OnInteractLeaveEvent>.Unregister(m_OnInteractLeaveBinding);
        EventBus<OnInteractEnterEvent>.Unregister(m_OnInteractEnterBinding);
        EventBus<OnPlayerTakeDamage>.Unregister(m_OnPlayerTakeDamage);
        EventBus<OnSetPlayerHealthEvent>.Unregister(m_OnSetHealthEvent);
    }

    void Initialize()
    {
        BoxCollider2D _dropZoneCollider = _dropZone.GetComponent<BoxCollider2D>();
        _dropZoneCollider.size = _dropZone.GetComponent<RectTransform>().rect.size;

        ActivateAll();

        EventBus<OnInventoryInputEvent>.Raise(new OnInventoryInputEvent { isOpen = false });


    }


    private void HandleGameReadyToStart(GameReadyToStartEvent arg0)
    {
        BindObjects();
        Initialize();
        _interactTransform.gameObject.SetActive(false);


    }
    private void HandlePlayerTakeDamage(OnPlayerTakeDamage arg0)
    {
        m_currentHealth -= arg0.value;
        UpdatePlayerHealthText();

    }
    private void UpdatePlayerHealthText()
    {
        m_playerHealthText.text = $"Health: {m_currentHealth}/{m_maxHealth}";
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
        if (_isHovering) return;

        string interactType = eventData.interactableType == InteractableType.Collectable ? "to collect " : "to interact with ";
        _isHovering = true;

        SetTextToInteract($"Press {_interactInput.GetBindingDisplayString(0)} {interactType}{eventData.InteractableName}");
        ShowInteractText();
        PopupInteractText();

    }

    void OnInteractLeave()
    {

        if (!_isHovering) return;
        HideInteractText();

    }
    void OnDisable()
    {
        UnbindEvents();
    }
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
        _interactText.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InBounce).OnComplete(() =>
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

    void ShowInventory()
    {

        _inventoryTransform.GetComponent<Canvas>().enabled = true;
    }
    void HideInventory()
    {
        _inventoryTransform.GetComponent<Canvas>().enabled = false;

    }

    void DesactivateAll()
    {
        _interactTransform.gameObject.SetActive(false);
        _inventoryTransform.gameObject.SetActive(false);
        m_PlayerHealthCanvas.gameObject.SetActive(false);

    }

    void ActivateAll()
    {

        _interactTransform.gameObject.SetActive(true);
        _inventoryTransform.gameObject.SetActive(true);
        m_PlayerHealthCanvas.gameObject.SetActive(true);
    }
}
