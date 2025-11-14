using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ComponentSlot : MonoBehaviour
{
    private RectTransform _transform;

    public ComponentSO currentComponent;
    public ComponentUI currentComponentUI;

    private AbstractWeapon weapon;

    int slotPosition;

    public bool m_isInventory;


    void Start()
    {
        _transform = GetComponent<RectTransform>();
        Vector2 _compSize = new Vector2(transform.GetComponent<RectTransform>().rect.width, transform.GetComponent<RectTransform>().rect.height);
        transform.GetComponent<BoxCollider2D>().size = _compSize;
    }

    public void OverrideComponent(ComponentUI component)
    {
        // Debug.Log("Override Component");
        component.transform.position = this.transform.position;
        component.transform.SetParent(this.transform);
        if (m_isInventory)
            component.transform.DOScale(Vector3.one * 2, 0.1f);
        else
        {
            component.transform.DOScale(Vector3.one, 0.1f);

        }
        currentComponentUI = component;
        component.SetSlot(this);



        currentComponent = component.componentData;
        if (weapon != null)
            weapon.weaponComponents[slotPosition] = currentComponent;
    }



    public void ClearSlot()
    {

        this.currentComponent = null;
        this.currentComponentUI = null;
        if (weapon == null) return;

        weapon.weaponComponents[slotPosition] = null;
    }

    public bool isEmpty()
    {
        return currentComponent == null & currentComponentUI == null;
    }

    internal void SetWeapon(AbstractWeapon weapon, int slotPosition)
    {
        this.weapon = weapon;
        this.slotPosition = slotPosition;
    }

    internal void ClearComponent()
    {
        if (currentComponentUI != null)
        {
            Destroy(currentComponentUI.gameObject);
        }
        currentComponent = null;
        currentComponentUI = null;
        weapon = null;
    }
}
