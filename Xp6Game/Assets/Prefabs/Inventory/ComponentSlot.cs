using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ComponentSlot : MonoBehaviour 
{
    private RectTransform _transform;

    public IComponent currentComponent;
    public ComponentUI currentComponentUI; 

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

    public void ClearSlot()
    {
        this.currentComponent = null;
        this.currentComponentUI = null;
    }

    public bool isEmpty()
    {
        return currentComponent == null & currentComponentUI == null;
    }
 



}
