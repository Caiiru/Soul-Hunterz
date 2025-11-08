using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DummyEnemy : Enemy<EnemySO>
{
    [Header("Dummy Debug")]
    public bool m_AlwaysShooting = true;
    public bool _canAttack = true;

    [SerializeField] private Transform _firePoint;
    [Space]
    [Header("Bullet & VFX")]

    public GameObject bulletPrefab;
    public GameObject fireVFXPrefab;

    //Animations

    protected override void OnEnable()
    {
        base.OnEnable();
        VFXDebugManager.OnInputPressed += OnInputPressed;


        if (m_AlwaysShooting)
            StartShooting();
    }


    private void OnInputPressed(int key)
    {
        if (key == 0)
            StartShooting();
        if (key == 1)
            StopShooting();
    }

    async void StartShooting()
    {
        _canAttack = true;
        await Aim();
    }


    private void StopShooting()
    {
        _canAttack = false;
    }


    async UniTask Aim()
    {
        await UniTask.Delay(10);
        m_animator.SetTrigger("Shoot");
        if (CanAttack())
            Attack();

        await UniTask.CompletedTask;
        await CastAim();
        // Attack();
    }
    public override void Attack()
    {
        base.Attack();

        GameObject bullet = Instantiate(bulletPrefab, _firePoint.transform.position, Quaternion.identity);
        // bullet.transform.position = _firePoint.transform.position;
        if (!fireVFXPrefab) return;
        GameObject fireVFX = Instantiate(fireVFXPrefab);
        fireVFX.transform.position = _firePoint.transform.position;
        Destroy(fireVFX, 2f);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

    }

    private async UniTask CastAim()
    {
        // await UniTask.Delay(1000);
        await UniTask.Delay((int)m_entityData.m_AttackCooldown * 1000);
        await Aim();


        return;
    }

}
