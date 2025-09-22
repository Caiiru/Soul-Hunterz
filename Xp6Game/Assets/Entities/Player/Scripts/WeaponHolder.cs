using System;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    public AbstractWeapon currentWeapon;
    public GameObject currentWeaponGO;
    public Transform firePoint; // Point from where the weapon fires

    private bool _canFire = true;
    void Start()
    {

    }

    private void HandleInventoryToggle(bool isOpen)
    {
        _canFire = !isOpen;
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
        currentWeapon.Attack(firePoint, firePoint.forward);
    }

    void OnEnable()
    {
        PlayerInventory.OnPlayerInventoryToggle += HandleInventoryToggle;   
    }

    void OnDisable()
    {
        PlayerInventory.OnPlayerInventoryToggle -= HandleInventoryToggle;   
    }
}
