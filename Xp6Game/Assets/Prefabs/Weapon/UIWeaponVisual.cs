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
        Debug.Log(weapon.ComponentList.Count);
        this.weapon = weapon;
        int componentsLength = weapon.ComponentList.Count;
        Slots = new GameObject[componentsLength];
        for (int i = 0; i < weapon.ComponentList.Count; i++)
        { 
            GameObject slot = Instantiate(SlotPrefab, this.transform);
            Slots[i] = slot;
            if (weapon.ComponentList[i] != null)
            {
                GameObject componentGO = Instantiate(componentUIPrefab);
                ComponentUI component = componentGO.GetComponent<ComponentUI>();
                component.SetComponentVisual(weapon.ComponentList[i]);

                slot.GetComponent<ComponentSlot>().OverrideComponent(component);
            } 
        }
    }
}
