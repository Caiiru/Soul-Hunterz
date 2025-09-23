using System.Collections.Generic;
using CMF;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] bool isInventoryOpen = false;

    [SerializeField] Canvas inventoryCanvas;

    // private CharacterInput _characterInput;

    KeyCode inventoryKey = KeyCode.E;

    public AbstractWeapon[] weapons = new AbstractWeapon[3];

    #region Events

    public delegate void PlayerInventoryHandler(bool isOpen);
    public static event PlayerInventoryHandler OnPlayerInventoryToggle;

    public delegate void PlayerGetWeapon(AbstractWeapon weapon, int slot);
    public static event PlayerGetWeapon OnPlayerGetWeapon;

    #endregion
    void Start()
    {
        // isInventoryOpen = true;
        // ToggleInventory();

    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
    }

    void CheckInput()
    {
        if (Input.GetKeyDown(inventoryKey))
        {
            ToggleInventory();
        }
    }

    void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        OnPlayerInventoryToggle?.Invoke(isInventoryOpen);
    }

    public void ChangeWeapon(AbstractWeapon weapon, int slot)
    {
        if (slot > weapons.Length)
        {
            return;
        }

        weapons[slot] = weapon;
        OnPlayerGetWeapon?.Invoke(weapon,slot);
    }

    public void DebugWeapon()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            SimpleWeapon weapon = new SimpleWeapon();
            weapon.Components = new IComponent[5];
            ChangeWeapon(weapon, 0);


        }
    }

}
