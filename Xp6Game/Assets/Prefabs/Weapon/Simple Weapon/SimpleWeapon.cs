using UnityEngine;

public class SimpleWeapon : AbstractWeapon
{
    public override void Attack()
    {
        base.Attack();

        if (!m_CanAttack) return;
        m_CanAttack = false;

        Shoot();


        m_CurrentFireDelay = 0;
        m_CurrentAmmo--;


        EventBus<OnPlayerAttack>.Raise(new OnPlayerAttack());
        EventBus<OnAmmoChanged>.Raise(new OnAmmoChanged
        {
            currentAmmo = m_CurrentAmmo,
            maxAmmo = m_maxAmmo
        });

        if (m_CurrentAmmo <= 0)
        {
            m_CurrentRechargeTime = 0;
        }
    }


    public void Shoot()
    {
        // 1. Cria um payload inicial.
        var payload = new BulletPayload();

        // 2. Passa o payload por todos os componentes para modificá-lo.
        foreach (var component in m_weaponComponents)
        {
            if (component == null) continue;
            payload = component.Execute(payload, _firePoint, m_currentIndexSlot);
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
            var bulletGO = Instantiate(m_bulletPrefab, _firePoint.position, _firePoint.rotation);
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

            var bulletGO = Instantiate(m_bulletPrefab, finalPosition, _firePoint.rotation);
            var bullet = bulletGO.GetComponent<Bullet>();

            bullet.Initialize(new Vector3(bullet.transform.forward.x, 0, bullet.transform.forward.z), payload);
        }
    }
}
