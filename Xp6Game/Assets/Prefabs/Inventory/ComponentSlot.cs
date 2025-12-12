using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ComponentSlot : MonoBehaviour, IPointerEnterHandler
{
    private RectTransform _transform;

    public ComponentSO currentComponent;
    public ComponentUI currentComponentUI;

    private AbstractWeapon weapon;

    int slotPosition;

    public bool m_isInventory;

    [Header("SFX")]
    public AnimationEventSound m_ComponentAppliedSFX;
    public AnimationEventSound m_HoverSFX;


    void Start()
    {
        _transform = GetComponent<RectTransform>();
        Vector2 _compSize = new Vector2(transform.GetComponent<RectTransform>().rect.width, transform.GetComponent<RectTransform>().rect.height);
        transform.GetComponent<BoxCollider2D>().size = new Vector2(60, 60);
    }

    public void OverrideComponent(ComponentUI component)
    {
        // Debug.Log("Override Component");
        component.transform.position = this.transform.position;
        component.transform.SetParent(this.transform);

        currentComponentUI = component;
        component.SetSlot(this);



        currentComponent = component.componentData;
        if (weapon == null) return;


        weapon.m_weaponComponents[slotPosition] = currentComponent;

        EventBus<OnComponentUpdate>.Raise(new OnComponentUpdate());

        AudioManager.Instance.PlayOneShotAtPosition(m_ComponentAppliedSFX.soundEvent, transform.position);


        // weapon.ReadComponents();
    }



    public void ClearSlot()
    {

        this.currentComponent = null;
        this.currentComponentUI = null;
        if (weapon == null) return;

        weapon.m_weaponComponents[slotPosition] = null;
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlayOneShotAtPosition(m_HoverSFX.soundEvent, transform.position);
    }
}
