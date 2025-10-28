using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ComponentUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] bool _canDrag = true;
    [SerializeField] bool _isInventoryOpen = false;

    Vector3 _startDragPosition;
    [SerializeField]
    Transform _inventoryCanvas;
    [SerializeField]
    Transform _oldParent;
    Transform _hudTransform;

    ComponentSlot currentSlot;



    Vector3 normalScale;
    Vector3 dragScale;

    public ComponentSO componentData;

    //Event
    EventBinding<OnInventoryInputEvent> onInventoryToggleBinding;

    void Start()
    {

        Initialize();
        SetDraggable(true);
    }

    public void Initialize()
    {
        // Debug.Log("Initialize");
        // PlayerInventory.OnPlayerInventoryToggle += HandleInventoryToggle;
        onInventoryToggleBinding = new EventBinding<OnInventoryInputEvent>(HandleInventoryToggle);
        EventBus<OnInventoryInputEvent>.Register(onInventoryToggleBinding);

        // if (GameManager.Instance == null) return;
        // inventoryCanvas = transform.parent.parent.parent;
        _inventoryCanvas = GetPlayerInventory();
        
        
        
        if (PlayerHUD.Instance != null)
            _hudTransform = PlayerHUD.Instance.transform;
        
    }



    private void HandleInventoryToggle(OnInventoryInputEvent eventData)
    {
        // Debug.Log($"Open {isOpen}");
        _isInventoryOpen = eventData.isOpen;

    }
    void OnDestroy()
    {
        // PlayerInventory.OnPlayerInventoryToggle -= HandleInventoryToggle;
        EventBus<OnInventoryInputEvent>.Unregister(onInventoryToggleBinding);
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin Drag");
        if (!isDraggable()) return;


        _startDragPosition = this.transform.position;
        _oldParent = transform.parent;
        transform.SetParent(_inventoryCanvas);
        normalScale = transform.lossyScale;

        dragScale = normalScale * 0.8f;

        transform.DOScale(dragScale, 0.1f);

        currentSlot.ClearSlot();

        // this.transform.SetParent(canvasParent);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggable()) return;


        Collider2D hitCollider = Physics2D.OverlapPoint(transform.position);
        if (hitCollider != null && hitCollider.TryGetComponent(out ComponentSlot componentSlot))
        {
            if (componentSlot.isEmpty())
            {
                currentSlot.GetComponent<ComponentSlot>().ClearSlot();
                componentSlot.OverrideComponent(this);
                return;
            }
        }

        transform.DOScale(normalScale, 0.1f);
        this.transform.DOMove(_startDragPosition, 0.2f).OnComplete(() =>
        {
            transform.SetParent(_oldParent);
            _oldParent.GetComponent<ComponentSlot>().OverrideComponent(this);
            
        });


    }

    public void OnDrag(PointerEventData eventData)
    {
        // Debug.Log("on drag");
        if (!isDraggable()) return;
        transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1);
        // transform.SetParent(null);

    }

    public void SetSlot(ComponentSlot slot)
    {
        currentSlot = slot;
    }

    public void SetVisualByData()
    {
        this.GetComponent<Image>().sprite = componentData.Icon;
    }

    public void SetComponentVisual(ComponentSO componentData)
    {
        this.componentData = componentData;
        SetVisualByData();
    }
    public void SetComponentSprite(Sprite newIcon)
    {
        this.GetComponent<Image>().sprite = newIcon;
    }
    private Transform GetPlayerInventory()
    {
        if (PlayerHUD.Instance != null)
            return PlayerHUD.Instance.transform;
        return null;
    }

    public bool isDraggable()
    {
        return _canDrag && _isInventoryOpen;
    }

    public void SetDraggable(bool draggable)
    {
        _canDrag = draggable;
    }

}
