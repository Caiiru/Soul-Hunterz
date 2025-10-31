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

#if ENABLE_INPUT_SYSTEM
    InputAction _interactInput;
    InputAction _inventoryInput;
#endif
    // Events
    EventBinding<OnInteractEnterEvent> onInteractEnterBinding;
    EventBinding<OnInteractLeaveEvent> onInteractLeaveBinding;

    EventBinding<OnInventoryInputEvent> onInventoryInputBinding;


    // [Header("Texts")]
    TextMeshProUGUI _interactText;



    async void Start()
    {
        BindObjects();
        BindEvents();
        await Initialize();
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

    }

    void BindEvents()
    {
        //Interact Button / Action

        _interactInput = StarterAssetsInputs.Instance.GetInteractAction().action;

        onInteractEnterBinding = new EventBinding<OnInteractEnterEvent>(OnInteractEnter);
        EventBus<OnInteractEnterEvent>.Register(onInteractEnterBinding);
        onInteractLeaveBinding = new EventBinding<OnInteractLeaveEvent>(OnInteractLeave);
        EventBus<OnInteractLeaveEvent>.Register(onInteractLeaveBinding);

        onInventoryInputBinding = new EventBinding<OnInventoryInputEvent>(InventoryToggle);
        EventBus<OnInventoryInputEvent>.Register(onInventoryInputBinding);
    }
    void UnbindEvents()
    {

        EventBus<OnInteractLeaveEvent>.Unregister(onInteractLeaveBinding);
        EventBus<OnInteractEnterEvent>.Unregister(onInteractEnterBinding);
    }

    async Task<UniTask> Initialize()
    {
        BoxCollider2D _dropZoneCollider = _dropZone.GetComponent<BoxCollider2D>();
        _dropZoneCollider.size = _dropZone.GetComponent<RectTransform>().rect.size;

        EventBus<OnInventoryInputEvent>.Raise(new OnInventoryInputEvent { isOpen = false });
        return UniTask.CompletedTask;
    }

    private void InventoryToggle(OnInventoryInputEvent eventData)
    {
        // _inventoryTransform.gameObject.SetActive(eventData.isOpen);
        _inventoryTransform.GetComponent<Canvas>().enabled = eventData.isOpen;
    }


    void OnInteractEnter(OnInteractEnterEvent eventData)
    {
        if (_isHovering) return;

        string interactType = eventData.interactableType == InteractableType.Collectable ? "to collect " : "to interact with ";
        _isHovering = true;
        _interactTransform.gameObject.SetActive(true);

        _interactText.text = $"Press {_interactInput.GetBindingDisplayString(0)} {interactType}{eventData.InteractableName}";

        _interactText.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InBounce).OnComplete(() =>
        {
            // _interactTransform.DOScale(Vector3.one - new Vector3(0.1f, 0.1f, 0.1f), 0.5f).SetEase(Ease.InSine).Flip();
            // _interactTransform.transform.localScale = Vector3.one;
        });
    }

    void OnInteractLeave()
    {
        if (!_isHovering) return;
        _interactText.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.Flash).OnComplete(() =>
        {
            _isHovering = false;
            _interactTransform.gameObject.SetActive(false);
        });
    }
    void OnDisable()
    {
        UnbindEvents();
    }
}
