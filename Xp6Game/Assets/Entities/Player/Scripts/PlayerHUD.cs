using System;
using DG.Tweening;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHUD : MonoBehaviour
{
    //Singleton
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

    [SerializeField]
    Transform _inventoryTransform;
    [SerializeField]
    Transform _interactTransform;
    bool _isHovering = false;

#if ENABLE_INPUT_SYSTEM
    InputAction _interactInput;
    InputAction _inventoryInput;
#endif
    // Events
    EventBinding<OnInteractEnterEvent> onInteractEnterBinding;
    EventBinding<OnInteractLeaveEvent> onInteractLeaveBinding;

    EventBinding<OnInventoryInputEvent> onInventoryInputBinding;


    void OnEnable()
    {
        onInteractEnterBinding = new EventBinding<OnInteractEnterEvent>(OnInteractEnter);
        EventBus<OnInteractEnterEvent>.Register(onInteractEnterBinding);
        onInteractLeaveBinding = new EventBinding<OnInteractLeaveEvent>(OnInteractLeave);
        EventBus<OnInteractLeaveEvent>.Register(onInteractLeaveBinding);

        onInventoryInputBinding = new EventBinding<OnInventoryInputEvent>(InventoryToggle);
        EventBus<OnInventoryInputEvent>.Register(onInventoryInputBinding);

        _interactTransform.gameObject.SetActive(false);
    }
    void OnDisable()
    {
        EventBus<OnInteractLeaveEvent>.Unregister(onInteractLeaveBinding);
        EventBus<OnInteractEnterEvent>.Unregister(onInteractEnterBinding);
    }

    void Start()
    {
        _interactInput = StarterAssetsInputs.Instance.GetInteractAction().action;
        // StarterAssetsInputs.OnPlayerInventoryToggle += InventoryToggle;
     
     
        EventBus<OnInventoryInputEvent>.Raise(new OnInventoryInputEvent { isOpen = false });
    }

    private void InventoryToggle(OnInventoryInputEvent eventData)
    {
        // _inventoryTransform.gameObject.SetActive(eventData.isOpen);
        _inventoryTransform.GetComponent<Canvas>().enabled = eventData.isOpen;
    }


    void OnInteractEnter(OnInteractEnterEvent eventData)
    {
        if (_isHovering) return;
        Debug.Log($"Hovering {eventData.InteractableName}");
        _isHovering = true;
        _interactTransform.gameObject.SetActive(true);
#if ENABLE_INPUT_SYSTEM
        _interactTransform.GetComponentInChildren<TextMeshProUGUI>().text = $"Press {_interactInput.GetBindingDisplayString(0)} to interact with {eventData.InteractableName}";
#endif
        _interactTransform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InBounce).OnComplete(() =>
        {
            // _interactTransform.DOScale(Vector3.one - new Vector3(0.1f, 0.1f, 0.1f), 0.5f).SetEase(Ease.InSine).Flip();
            _interactTransform.transform.localScale = Vector3.one;
        });
    }

    void OnInteractLeave()
    {
        if (!_isHovering) return;
        _interactTransform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.Flash).OnComplete(() =>
        {
            _isHovering = false;
            _interactTransform.gameObject.SetActive(false);
        });
    }
}
