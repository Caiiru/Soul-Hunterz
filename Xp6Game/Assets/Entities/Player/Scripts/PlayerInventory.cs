using System;
using System.Collections;
using System.Collections.Generic;
using CMF;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] bool isInventoryOpen = false;
    // private CharacterInput _characterInput;
    InputAction _inventoryInput;

    [Header("Inventory KeyCodes")]
    public KeyCode debugWeapon = KeyCode.P;


    [Header("Weapons and Components")]
    private WeaponHolder _weaponHolder;
    [SerializeField] private int weaponCount = 3;
    public GameObject[] weapons;

    [SerializeField] private int componentCount = 10;
    public List<ComponentSO> components;

    [Space]
    [Header("Debug Prefabs")]
    public GameObject simpleComponentPrefab;
    public GameObject simpleWeaponPrefab;

    [Header("Currency")]
    public int m_currency = 0;


    #region Events
 
    EventBinding<OnCollectSouls> m_OnCollectSouls;


    public delegate void PlayerGetWeapon(AbstractWeapon weapon, int slot);
    public static event PlayerGetWeapon OnPlayerGetWeapon;

    #endregion
    void Start()
    { 
        BindEvents();
        Initialize();


    }
 
    private void Initialize()
    {
        //Input
        StarterAssetsInputs.OnPlayerInventoryToggle += ToggleInventory;

        _weaponHolder = GetComponent<WeaponHolder>();
        if (!_weaponHolder) Debug.LogWarning("Weapon Holder not find");
        weapons = new GameObject[weaponCount];
        components = new List<ComponentSO>();
        StartCoroutine(AddDebugWeapon());

        // EventBus<OnInventoryInputEvent>.Raise(new OnInventoryInputEvent{isOpen = false});



    }
    #region BindEvents
    private void BindEvents()
    {
        StarterAssetsInputs.OnChangeWeapon += ChangeWeapon;

        m_OnCollectSouls = new EventBinding<OnCollectSouls>(HandleCollectSoulsEvent);
        EventBus<OnCollectSouls>.Register(m_OnCollectSouls);

    }
    #endregion
    #region Events Handlers
    private void HandleCollectSoulsEvent(OnCollectSouls eventData)
    {
        m_currency += eventData.amount;
        EventBus<OnUpdateSouls>.Raise(new OnUpdateSouls { amount = m_currency });
    }

    private void HandlePlayerCollectComponent(OnCollectComponent arg0)
    {
        if (components.Capacity >= componentCount)
        {
            return;
        }
        components.Add(arg0.data);
    }
    #endregion
 

    void ToggleInventory(bool newState)
    {
        isInventoryOpen = newState;
        // OnPlayerInventoryToggle?.Invoke(isInventoryOpen);
        EventBus<OnInventoryInputEvent>.Raise(new OnInventoryInputEvent { isOpen = newState });
    }
    #region Weapons
    public void AddWeapon(AbstractWeapon weapon)
    {
        if (!hasWeaponSlot())
            return;

        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] == null)
            {
                weapons[i] = weapon.gameObject; 
                OnPlayerGetWeapon?.Invoke(weapon, i);
                return;
            }
        }
    }

    public void ChangeWeapon(int slot)
    {
        ClearWeapons();
        if (weapons[slot] == null) return;
        weapons[slot].SetActive(true);
        _weaponHolder.HoldWeapon(weapons[slot]);

    }

    private void ClearWeapons()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] == null) continue;
            weapons[i].gameObject.SetActive(false);

        }
        _weaponHolder.currentWeapon = null;
        _weaponHolder.currentWeaponGO = null;
    }
    #endregion 
  

    public IEnumerator AddDebugWeapon()
    {
        if (!hasWeaponSlot())
            yield break;

        yield return new WaitForSeconds(0.1f);
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
    #region Getters
    public bool IsInventoryOpen()
    {
        return isInventoryOpen;
    }
    public int GetCurrency()
    {
        return m_currency;
    }
    #endregion

    #region Unbind methods

    void OnDestroy()
    {
        UnbindEvents();
    }

    void UnbindEvents()
    {
        StarterAssetsInputs.OnChangeWeapon -= ChangeWeapon;
        EventBus<OnCollectSouls>.Unregister(m_OnCollectSouls); 
    }

    internal void RemoveCurrency(int m_SoulsPerInteraction)
    {
        m_currency -= m_SoulsPerInteraction;
        EventBus<OnUpdateSouls>.Raise(new OnUpdateSouls { amount = m_currency });
    }
    #endregion
}

public class OnInventoryInputEvent : IEvent
{
    public bool isOpen;
}
