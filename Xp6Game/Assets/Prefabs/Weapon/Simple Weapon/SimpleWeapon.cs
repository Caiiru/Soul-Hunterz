using UnityEngine;

public class SimpleWeapon : AbstractWeapon
{ 
    private float nextFire = 0.0f;
    public GameObject bulletPrefab;
    public override void Attack(Transform attackPoint, Vector3 direction)
    {
        if(Time.time >= nextFire)
        {
            Shoot(attackPoint, direction);
            nextFire = Time.time + AttackRate;
        }
    }

    public void Shoot(Transform attackPoint, Vector3 direction)
    {
        Instantiate(bulletPrefab, attackPoint.position, attackPoint.rotation);
    }
}
