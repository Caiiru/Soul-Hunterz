using System.Collections;
using System.Collections.Generic;
using CMF;
using StarterAssets;
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
    private WeaponHolder _weaponHolder;
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

        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
    }
    private void Initialize()
    {
        _weaponHolder = GetComponent<WeaponHolder>();
        if (!_weaponHolder) Debug.LogWarning("Weapon Holder not find");
        weapons = new GameObject[weaponCount];
        components = new GameObject[componentCount];
        HandleEvents();
        StartCoroutine(AddDebugWeapon());



    }
    private void HandleEvents()
    {
        StarterAssetsInputs.OnChangeWeapon += ChangeWeapon;
    }



    void CheckInput()
    {
        if (Input.GetKeyDown(inventoryKey))
        {
            ToggleInventory();
        }
        InputDebugWeapon();
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

        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] == null)
            {
                OnPlayerGetWeapon?.Invoke(weapon, i);
                return;
            }
        }
    }

    public void ChangeWeapon(int slot)
    {
        Debug.Log($"Change Weapon on Inventory to Slot {slot}");


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
    public void InputDebugWeapon()
    {
        if (Input.GetKeyDown(debugWeapon))
        {
            AddDebugWeapon();
        }
    }

    public IEnumerator AddDebugWeapon()
    {
        if (!hasWeaponSlot())
            yield break;

        yield return new WaitForSeconds(1);
        GameObject weapon = Instantiate(simpleWeaponPrefab, this.transform);
        weapon.GetComponent<AbstractWeapon>().InitializeWeapon();
        AddWeapon(weapon.GetComponent<AbstractWeapon>());
        _weaponHolder.HoldWeapon(weapon);
        // weapon.transform.position = _weaponHolder.firePoint.position;
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


    void OnDisable()
    {
        StarterAssetsInputs.OnChangeWeapon -= ChangeWeapon;
    }

}
