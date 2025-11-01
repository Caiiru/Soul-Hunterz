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
        // 1. Cria um payload inicial.
        var payload = new BulletPayload();

        // 2. Passa o payload por todos os componentes para modificá-lo.
        foreach (var component in weaponComponents)
        {
            if (component == null) continue;
            payload = component.Execute(payload, _firePoint, currentIndexSlot);
        }

        // 3. Dispara as balas com base no payload final.
        FireBullets(payload);
    }

    private void FireBullets(BulletPayload payload)
    {
        if (payload.BulletCount <= 0) return;

        if (payload.BulletCount == 1)
        {
            // Disparo simples
            var bulletGO = Instantiate(bulletPrefab, _firePoint.position, _firePoint.rotation);
            var bullet = bulletGO.GetComponent<Bullet>();
            bullet.Initialize(transform.forward, payload);
            return;
        } 
        // Lógica para múltiplos disparos (movida de MultipleBulletComponentSO)
        float totalWidth = (payload.BulletCount - 1) * payload.SpreadDistance;
        float initialOffset = -totalWidth / 2f;
        Vector3 spreadDirection = _firePoint.right; // Assumindo spread horizontal

        for (int i = 0; i < payload.BulletCount; i++)
        {
            float currentOffset = initialOffset + (i * payload.SpreadDistance);
            Vector3 finalPosition = _firePoint.position + (spreadDirection * currentOffset);
            
            var bulletGO = Instantiate(bulletPrefab, finalPosition, _firePoint.rotation);
            var bullet = bulletGO.GetComponent<Bullet>();

            bullet.Initialize(new Vector3(bullet.transform.forward.x,0,bullet.transform.forward.z), payload);
        }
    }
}
