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

    [SerializeField]
    Transform _interactTransform;
    bool _isHovering = false;

    [Header("Inventory")]
    [SerializeField]
    Transform _inventoryTransform;
    [SerializeField] Transform _dropZone;

    public TextMeshProUGUI m_playerHealth;

#if ENABLE_INPUT_SYSTEM
    InputAction _interactInput;
    InputAction _inventoryInput;
#endif


    // Events
    EventBinding<GameReadyToStartEvent> m_OnGameReadyToStart;

    EventBinding<OnInteractEnterEvent> onInteractEnterBinding;
    EventBinding<OnInteractLeaveEvent> onInteractLeaveBinding;

    EventBinding<OnInventoryInputEvent> onInventoryInputBinding;


    // [Header("Texts")]
    TextMeshProUGUI _interactText;


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


        onInteractEnterBinding = new EventBinding<OnInteractEnterEvent>(OnInteractEnter);
        EventBus<OnInteractEnterEvent>.Register(onInteractEnterBinding);
        onInteractLeaveBinding = new EventBinding<OnInteractLeaveEvent>(OnInteractLeave);
        EventBus<OnInteractLeaveEvent>.Register(onInteractLeaveBinding);

        onInventoryInputBinding = new EventBinding<OnInventoryInputEvent>(InventoryToggle);
        EventBus<OnInventoryInputEvent>.Register(onInventoryInputBinding);
    }

    private void HandleGameReadyToStart(GameReadyToStartEvent arg0)
    {
        BindObjects();
        Initialize();
        Debug.Log("HUD start");
    }

    void UnbindEvents()
    {
        EventBus<GameReadyToStartEvent>.Unregister(m_OnGameReadyToStart);
        EventBus<OnInventoryInputEvent>.Unregister(onInventoryInputBinding);
        EventBus<OnInteractLeaveEvent>.Unregister(onInteractLeaveBinding);
        EventBus<OnInteractEnterEvent>.Unregister(onInteractEnterBinding);
    }

    void Initialize()
    {
        BoxCollider2D _dropZoneCollider = _dropZone.GetComponent<BoxCollider2D>();
        _dropZoneCollider.size = _dropZone.GetComponent<RectTransform>().rect.size;

        ActivateAll();

        EventBus<OnInventoryInputEvent>.Raise(new OnInventoryInputEvent { isOpen = false });


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

    }

    void ActivateAll()
    {
        
        _interactTransform.gameObject.SetActive(true);
        _inventoryTransform.gameObject.SetActive(true);
    }
}
