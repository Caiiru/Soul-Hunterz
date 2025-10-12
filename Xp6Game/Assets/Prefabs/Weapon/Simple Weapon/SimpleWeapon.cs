using UnityEngine;

public class SimpleWeapon : AbstractWeapon
{
    [Header("Bullet Settings")]
    private float nextFire = 0.0f;
    public override void Attack()
    {
        if (Time.time >= nextFire)
        {
            Shoot();
            nextFire = Time.time + AttackRate;
        }
    }

    public void Shoot()
    {
        GameObject _bulletGO = Instantiate(bulletPrefab, _firePoint.position, Quaternion.identity);
        var bullet = _bulletGO.GetComponent<Bullet>();
        bullet.Direction = transform.forward;

        foreach (var component in weaponComponents)
        {
            if (component == null) continue;

            // Debug.Log("Execute component: " + component.ComponentName);
            component.Execute(_bulletGO);
        }

        bullet.Initialize();


    }
}
