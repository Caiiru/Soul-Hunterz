using System;
using UnityEngine;

public class InventoryVisual : MonoBehaviour
{
    public Transform weaponsPanel;
    public Transform componentsPanel;

    public Transform dropZone;

    public GameObject weaponVisualPrefab;
    [Space]
    [Header("Components")]
    public int componentCount = 10;
    public GameObject componentSlotPrefab;
    public GameObject componentUIPrefab;


    private GameObject[] componentsArray;

    //PRIVATE
    private Canvas _canvas;

    //events

    public delegate void UpdateWeaponVisualDelegate(int slot, ComponentSO component);
    public static event UpdateWeaponVisualDelegate OnUpdateWeaponVisual;

    EventBinding<OnInventoryInputEvent> onInventoryToggleBinding;

    EventBinding<OnCollectComponent> m_OnPlayerCollectComponent;




    void Start()
    {

        // PlayerInventory.OnPlayerInventoryToggle += HandleInventoryToggle;

        // HandleInventoryToggle(false);
        BindObjects();
        BindEvents();
        Initialize();
    }

    void BindObjects()
    {
        _canvas = GetComponent<Canvas>();
        componentsArray = new GameObject[componentCount];
    }
    void BindEvents()
    {

        onInventoryToggleBinding = new EventBinding<OnInventoryInputEvent>(HandleInventoryToggle);
        EventBus<OnInventoryInputEvent>.Register(onInventoryToggleBinding);

        m_OnPlayerCollectComponent = new EventBinding<OnCollectComponent>(HandlePlayerCollectComponent);
        EventBus<OnCollectComponent>.Register(m_OnPlayerCollectComponent);



        PlayerInventory.OnPlayerGetWeapon += AddWeaponVisual;
    }



    void Initialize()
    {

        SpawnComponentsVisual();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            int index = 0;
            foreach (var component in componentsArray)
            {
                if (component.GetComponent<ComponentSlot>().currentComponentUI == null)
                {
                    GameObject comp = Instantiate(componentUIPrefab);
                    componentsArray[index].GetComponent<ComponentSlot>().OverrideComponent(comp.GetComponent<ComponentUI>());
                    return;
                }
                index++;
            }

        }
    }

    void SpawnComponentsVisual()
    {
        for (int i = 0; i < componentCount; i++)
        {
            GameObject component = Instantiate(componentSlotPrefab, componentsPanel);
            componentsArray[i] = component;
        }
    }

    private void HandlePlayerCollectComponent(OnCollectComponent arg0)
    {
        AddComponentVisual(arg0.data);
    }

    private void HandleInventoryToggle(OnInventoryInputEvent eventData)
    {
        // _canvas.enabled = eventData.isOpen;
    }
    public void AddWeaponVisual(AbstractWeapon weapon, int slot)
    {
        Debug.Log("Add Weapon Visual");
        GameObject visual = Instantiate(weaponVisualPrefab, weaponsPanel);

        visual.GetComponent<UIWeaponVisual>().UpdateVisual(weapon, componentUIPrefab);
    }

    void AddComponentVisual(ComponentSO data)
    {
        int index = 0;
        foreach (var component in componentsArray)
        {
            if (component.GetComponent<ComponentSlot>().currentComponentUI == null)
            {
                GameObject comp = Instantiate(componentUIPrefab);
                comp.GetComponent<ComponentUI>().SetComponentVisual(data);
                componentsArray[index].GetComponent<ComponentSlot>().OverrideComponent(comp.GetComponent<ComponentUI>());
                return;
            }
            index++;
        }
    }

    public void UpdateWeaponVisual(int slot, ComponentSO component)
    {
        OnUpdateWeaponVisual?.Invoke(slot, component);
    }


    void OnDestroy()
    {
        PlayerInventory.OnPlayerGetWeapon -= AddWeaponVisual;
        EventBus<OnInventoryInputEvent>.Unregister(onInventoryToggleBinding);
        EventBus<OnCollectComponent>.Unregister(m_OnPlayerCollectComponent);
    }
}
