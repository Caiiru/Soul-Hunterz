using System;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    public AbstractWeapon currentWeapon;
    public GameObject currentWeaponGO;
    public Transform firePoint; // Point from where the weapon fires

    private bool _canFire = true;
    
    //Events
    EventBinding<OnInventoryInputEvent> onInventoryToggleBinding;

    void Start()
    {

    }

    

    // Update is called once per frame
    void Update()
    {
        if (CanFire())
        {
            FireWeapon();
        }

    }

    bool CanFire()
    {
        return Input.GetButton("Fire1") && currentWeapon != null && _canFire;
    }

    public void FireWeapon()
    {
        currentWeapon.Attack();
    }

    void OnEnable()
    {

        // PlayerInventory.OnPlayerInventoryToggle += HandleInventoryToggle;
        onInventoryToggleBinding = new EventBinding<OnInventoryInputEvent>(HandleInventoryToggle);
        EventBus<OnInventoryInputEvent>.Register(onInventoryToggleBinding);
    }

    void OnDisable()
    {
        // PlayerInventory.OnPlayerInventoryToggle -= HandleInventoryToggle;   
        EventBus<OnInventoryInputEvent>.Unregister(onInventoryToggleBinding);
    }

    internal void HoldWeapon(GameObject weapon)
    {
        currentWeaponGO = weapon;
        currentWeapon = weapon.GetComponent<AbstractWeapon>();
        currentWeaponGO.transform.SetParent(firePoint.transform);
        currentWeaponGO.transform.localPosition = Vector3.zero;
        currentWeaponGO.transform.localRotation = Quaternion.identity;

    }
    private void HandleInventoryToggle(OnInventoryInputEvent eventdata)
    {
        _canFire = !eventdata.isOpen;
    }
}
