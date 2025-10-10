using DG.Tweening;
using UnityEngine;

public class UIWeaponVisual : MonoBehaviour
{
    [Header("Visual")]
    [Tooltip("Put here the component visual slot")]
    public GameObject SlotPrefab;

    public GameObject[] Slots;


    public Transform ComponentsPanel;
    public Transform BulletPanel;

    [HideInInspector]
    public AbstractWeapon weapon;

    /// <summary>
    /// Updates the visual representation of a weapon's components in the UI.
    /// </summary>
    /// <param name="weapon">The <see cref="AbstractWeapon"/> whose component list will be visualized. The method reads <c>weapon.ComponentList</c> to determine how many slots to create and which component visuals to instantiate.</param>
    /// <param name="componentUIPrefab">A UI prefab to instantiate for each non-null component in the weapon's component list. The prefab is expected to contain a <c>ComponentUI</c> component which will be initialized with the corresponding component data.</param>
    /// <remarks>
    /// Side effects:
    /// - Assigns <c>this.weapon</c>.
    /// - Resizes/overwrites the <c>Slots</c> array to <c>weapon.ComponentList.Count</c>.
    /// - Instantiates a slot GameObject from <c>SlotPrefab</c> for each component and parents it to <c>this.transform</c>.
    /// - For non-null components, instantiates <paramref name="componentUIPrefab"/> parented under the inventory transform obtained via <c>GetInventoryTransform()</c>, calls <c>ComponentUI.SetComponentVisual</c>, and calls <c>ComponentSlot.OverrideComponent</c> on the corresponding slot.
    /// - Operates on the Unity main thread and modifies the scene (allocates GameObjects).
    ///
    /// Notes and requirements:
    /// - <paramref name="componentUIPrefab"/> must contain a <c>ComponentUI</c> script.
    /// - <c>SlotPrefab</c> and the inventory transform returned by <c>GetInventoryTransform()</c> must be valid.
    /// - The method does not preserve or clean up any previously created slot or component GameObjects; callers should handle cleanup if needed.
    /// </remarks>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="weapon"/> or <paramref name="componentUIPrefab"/> is null.</exception>
    /// <exception cref="System.InvalidOperationException">May be thrown if required prefabs/components (for example <c>SlotPrefab</c> or a <c>ComponentUI</c> on the provided prefab) are missing or invalid.</exception>
    public void UpdateVisual(AbstractWeapon weapon, GameObject componentUIPrefab)
    {
        this.weapon = weapon;
        int componentsLength = weapon.ComponentList.Count;
        Slots = new GameObject[componentsLength];

        Transform _inventoryTransform = GetInventoryTransform();

        for (int i = 0; i < weapon.ComponentList.Count; i++)
        {
            GameObject slot = Instantiate(SlotPrefab, ComponentsPanel);
            Slots[i] = slot;
            if (weapon.ComponentList[i] != null)
            {
                GameObject componentGO = Instantiate(componentUIPrefab, _inventoryTransform);
                ComponentUI component = componentGO.GetComponent<ComponentUI>();
                component.SetComponentVisual(weapon.ComponentList[i]);


                slot.GetComponent<ComponentSlot>().OverrideComponent(component);

            }
        }

        // Bullet
        GameObject bulletGO = Instantiate(componentUIPrefab, BulletPanel);
        ComponentUI bullet = bulletGO.GetComponent<ComponentUI>();
        bullet.SetComponentSprite(weapon.GetBullet().Icon);
        bulletGO.transform.localPosition = Vector3.zero;
        bulletGO.transform.DOScale(Vector3.one, 0.1f);
        bullet.SetDraggable(false);
    }

    Transform GetInventoryTransform()
    {
        GameObject player = GameManager.Instance.GetPlayer();
        return player.GetComponentInChildren<PlayerInventory>().GetInventoryTransform();
    }
}
