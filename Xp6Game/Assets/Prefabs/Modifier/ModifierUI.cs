using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ModifierUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] bool canDrag = false;
    [SerializeField] bool isDragging = false;

    private bool _isMouseOver = false;

    private Camera _playerCamera;
    void Start()
    {
        _playerCamera = GameManager.Instance.playerCamera;
        if (_playerCamera == null)
            Debug.LogError("Player Camera not found in GameManager.");


        PlayerInventory.OnPlayerInventoryToggle += HandleInventoryToggle;
    }

    private void HandleInventoryToggle(bool isOpen)
    {
        canDrag = isOpen;

    }
    void OnDestroy()
    {
        PlayerInventory.OnPlayerInventoryToggle -= HandleInventoryToggle;
    }
 
 
    public void OnBeginDrag(PointerEventData eventData)
    { 
        if(!canDrag) return;
    } 
    public void OnEndDrag(PointerEventData eventData)
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        transform.position = Input.mousePosition;   
    }

}
