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

    void Start()
    {
        _canvas = GetComponent<Canvas>();
        PlayerInventory.OnPlayerInventoryToggle += HandleInventoryToggle;
        PlayerInventory.OnPlayerGetWeapon += UpdateWeaponVisual;
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
        PlayerInventory.OnPlayerGetWeapon -= UpdateWeaponVisual;
    }
    private void HandleInventoryToggle(bool isOpen)
    {
        _canvas.enabled = isOpen;
    }
    public void UpdateWeaponVisual(AbstractWeapon weapon, int slot)
    {
        GameObject visual = Instantiate(weaponVisualPrefab, weaponsPanel);


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
