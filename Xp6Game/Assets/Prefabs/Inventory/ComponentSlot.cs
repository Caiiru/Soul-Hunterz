using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ComponentSlot : MonoBehaviour, IDropHandler, IBeginDragHandler
{
    private RectTransform _transform;

    public IComponent currentComponent;
    public ComponentUI currentComponentUI;
    public void OnDrop(PointerEventData eventData)
    {
        return;
        if (eventData.pointerDrag != null)
        {
            Debug.Log("Component Drop: " + this.transform.name);
            Vector2 targetPosition = transform.position;
            // Vector2 targetPosition = new Vector2(_transform.anchoredPosition.x - _transform.sizeDelta.x/2, _transform.anchoredPosition.y - _transform.sizeDelta.y/2);
            var component = eventData.pointerDrag.transform.gameObject;
            component.transform.SetParent(this.transform);
            // component.transform.GetComponent<RectTransform>().transform.position = targetPosition;
            // component.transform.position = Vector3.zero;
            component.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

        }
    }

    public void OverrideComponent(ComponentUI component)
    {
        // Debug.Log("Override Component");
        component.transform.position = this.transform.position;
        component.transform.SetParent(this.transform);
        component.transform.DOScale(Vector3.one, 0.1f); 
        currentComponentUI = component;
        component.SetSlot(this);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        _transform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void ClearSlot()
    {
        this.currentComponent = null;
        this.currentComponentUI = null;
    }

    public bool isEmpty()
    {
        return currentComponent == null & currentComponentUI == null;
    }

    // Update is called once per frame



}
