using Cysharp.Threading.Tasks;
using UnityEngine;

public class RangedEnemy : Enemy<RangedEnemySO>
{
    [SerializeField] private Transform _firePoint;


    private float _shotCooldown;
    private float _timer;

    // [Header("Debug")]
    // public bool m_Initialize = true;
    protected override void OnEnable()
    {
        base.OnEnable();
        // if (m_Initialize)
        // {
        //     SetData(entityData as EnemySO);
        //     Initialize();
        // }



    }

    public override void Initialize()
    {
        base.Initialize();

        m_attackRange = m_entityData.m_AttackRange;
        m_speed = m_entityData.m_MoveSpeed;


        _firePoint = transform.Find("FirePoint");
    }
    public override bool CanAttack()
    {
        return base.CanAttack();
    }

    public override void Attack()
    {
        if (!CanAttack()) return;


        base.Attack();
        
        if (m_animator)
        {
            m_animator.SetTrigger("Shoot");
        }
        if (m_entityData.bulletPrefab != null && _firePoint != null)
        {
            Instantiate(m_entityData.bulletPrefab, _firePoint.position, _firePoint.rotation);

        }
    }


}
