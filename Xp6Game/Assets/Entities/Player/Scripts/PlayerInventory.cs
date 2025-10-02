using System.Collections.Generic;
using CMF;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] bool isInventoryOpen = false;

    [SerializeField] Transform inventoryTransform;

    // private CharacterInput _characterInput;
    [Header("Inventory KeyCodes")]
    public KeyCode inventoryKey = KeyCode.E;

    public KeyCode debugComponent = KeyCode.O;
    public KeyCode debugWeapon = KeyCode.P;


    [Header("Weapons and Components")]
    [SerializeField] private int weaponCount = 3;
    public GameObject[] weapons;

    [SerializeField] private int componentCount = 10;
    public GameObject[] components;

    [Space]
    [Header("Debug Prefabs")]
    public GameObject simpleComponentPrefab;
    public GameObject simpleWeaponPrefab;


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

        weapons = new GameObject[weaponCount];
        components = new GameObject[componentCount];

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
        DebugWeapon();
    }

    void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        OnPlayerInventoryToggle?.Invoke(isInventoryOpen);
    }

    public void AddWeapon(AbstractWeapon weapon)
    {
        if (!hasWeaponSlot())
            return;
        Debug.Log($"weapon list count: {weapon.ComponentList.Count}");
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] == null)
            {
                OnPlayerGetWeapon?.Invoke(weapon, i);
                return;
            }
        }
    }
    public void AddComponent(GameObject component)
    {

    }

    public void DebugComponent()
    {
        if (Input.GetKeyDown(debugWeapon))
        {
            GameObject comp = Instantiate(simpleComponentPrefab);
            AddComponent(comp);
        }
    }
    public void DebugWeapon()
    {
        if (Input.GetKeyDown(debugWeapon))
        {
            GameObject weapon = Instantiate(simpleWeaponPrefab, this.transform.parent);
            weapon.GetComponent<AbstractWeapon>().InitializeWeapon();
            AddWeapon(weapon.GetComponent<AbstractWeapon>());
        }
    }

    private bool hasWeaponSlot()
    {
        foreach (var weapon in weapons)
        {
            if (weapon != null)
                return false;
        }

        return true;
    }

    public Transform GetInventoryTransform()
    {
        return inventoryTransform;
    }

}
