using UnityEngine;

public class UIWeaponVisual : MonoBehaviour
{
    [Header("Visual")]
    [Tooltip("Put here the component visual slot")]
    public GameObject SlotPrefab;

    public GameObject[] Slots;

    [HideInInspector]
    public AbstractWeapon weapon;

    public void UpdateVisual(AbstractWeapon weapon, GameObject componentUIPrefab)
    {
        this.weapon = weapon;
        int componentsLength = weapon.ComponentList.Count;
        Slots = new GameObject[componentsLength];

        Transform _inventoryTransform = GetInventoryTransform();

        for (int i = 0; i < weapon.ComponentList.Count; i++)
        {
            GameObject slot = Instantiate(SlotPrefab, this.transform);
            Slots[i] = slot;
            if (weapon.ComponentList[i] != null)
            {
                GameObject componentGO = Instantiate(componentUIPrefab, _inventoryTransform);
                ComponentUI component = componentGO.GetComponent<ComponentUI>();
                component.SetComponentVisual(weapon.ComponentList[i]);


                slot.GetComponent<ComponentSlot>().OverrideComponent(component);


                // RectTransform compTransform = component.GetComponent<RectTransform>();
                // compTransform.position = new Vector3(compTransform.rect.width / 2, compTransform.rect.height / 2, 0); 


            }
        }
    }

    Transform GetInventoryTransform()
    {
        GameObject player = GameManager.Instance.GetPlayer();
        return player.GetComponentInChildren<PlayerInventory>().GetInventoryTransform();
    }
}
