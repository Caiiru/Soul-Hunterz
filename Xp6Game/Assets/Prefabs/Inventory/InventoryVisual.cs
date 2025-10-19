using UnityEngine;

public class InventoryVisual : MonoBehaviour
{
    public Transform weaponsPanel;
    public Transform componentsPanel;

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

    void Start()
    {
        _canvas = GetComponent<Canvas>();
        PlayerInventory.OnPlayerInventoryToggle += HandleInventoryToggle;
        PlayerInventory.OnPlayerGetWeapon += AddWeaponVisual;
        HandleInventoryToggle(false);


        componentsArray = new GameObject[componentCount];
        SpawnComponentsVisual();
    }

    void SpawnComponentsVisual()
    {
        for (int i = 0; i < componentCount; i++)
        {
            GameObject component = Instantiate(componentSlotPrefab, componentsPanel);
            componentsArray[i] = component;
        }
    }
    void OnDisable()
    {

        PlayerInventory.OnPlayerInventoryToggle -= HandleInventoryToggle;
        PlayerInventory.OnPlayerGetWeapon -= AddWeaponVisual;
    }
    private void HandleInventoryToggle(bool isOpen)
    {
        _canvas.enabled = isOpen;
    }
    public void AddWeaponVisual(AbstractWeapon weapon, int slot)
    {
        GameObject visual = Instantiate(weaponVisualPrefab, weaponsPanel);

        visual.GetComponent<UIWeaponVisual>().UpdateVisual(weapon, componentUIPrefab);
    }
    
    public void UpdateWeaponVisual(int slot, ComponentSO component)
    {
        OnUpdateWeaponVisual?.Invoke(slot,component);
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
}
