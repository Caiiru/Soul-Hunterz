using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    public AbstractWeapon currentWeapon;
    public GameObject currentWeaponGO;

    public Transform firePoint; // Point from where the weapon fires
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1") && currentWeapon != null)
        {
            FireWeapon();
        }
    }

    public void FireWeapon()
    {
        currentWeapon.Attack(firePoint, firePoint.forward);
    }
}
