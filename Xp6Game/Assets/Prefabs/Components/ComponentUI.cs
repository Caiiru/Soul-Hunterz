using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ComponentUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] bool canDrag = true;
    [SerializeField] bool isInventoryOpen = false;

    Vector3 startDragPosition;
    [SerializeField]
    Transform inventoryCanvas;
    [SerializeField]
    Transform oldParent;

    ComponentSlot currentSlot;

    Vector3 normalScale;
    Vector3 dragScale;

    void Start()
    {
        PlayerInventory.OnPlayerInventoryToggle += HandleInventoryToggle;
        inventoryCanvas = transform.parent.parent.parent;

        inventoryCanvas = GetPlayerInventory();
        // canDrag = true;

        // canvasParent = transform.parent.parent;
    }



    private void HandleInventoryToggle(bool isOpen)
    {
        isInventoryOpen = isOpen;

    }
    void OnDestroy()
    {
        PlayerInventory.OnPlayerInventoryToggle -= HandleInventoryToggle;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDraggable()) return;


        startDragPosition = this.transform.position;
        oldParent = transform.parent;
        transform.SetParent(inventoryCanvas);
        normalScale = transform.lossyScale;

        dragScale = normalScale * 0.8f;

        transform.DOScale(dragScale, 0.1f);

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
        this.transform.DOMove(startDragPosition, 0.2f).OnComplete(() => transform.SetParent(oldParent));

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable()) return;
        transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1);
        // transform.SetParent(null);

    }

    public void SetSlot(ComponentSlot slot)
    {
        currentSlot = slot;
    }

    public void SetComponentVisual(ComponentSO visualData)
    {
        this.GetComponent<Image>().sprite = visualData.Icon;
    }
    public void SetComponentSprite(Sprite newIcon)
    {
        this.GetComponent<Image>().sprite = newIcon;
    }
    private Transform GetPlayerInventory()
    {
        GameObject player = GameManager.Instance.GetPlayer();
        return player.GetComponentInChildren<PlayerInventory>().GetInventoryTransform();
    }

    public bool isDraggable()
    {
        return canDrag && isInventoryOpen;
    }

    public void SetDraggable(bool draggable)
    {
        canDrag = draggable;
    }   

}
