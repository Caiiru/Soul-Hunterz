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


        _shotCooldown = m_entityData.timeBetweenShots;
        _timer = _shotCooldown; // So it can shoot immediately on spawn 

        m_attackRange = m_entityData.attackRange;
        m_speed = m_entityData.movementSpeed;


        _firePoint = transform.Find("FirePoint");
    }
    public override bool CanAttack()
    {
        if (_timer < _shotCooldown)
        {
            _timer += Time.deltaTime;
            return false;
        }
        _timer = 0;

        return true;
    }

    public override void Attack()
    {
        // Debug.Log("ranged atk");
        if (!CanAttack()) return;
        if (_animator)
        {
            _animator.SetTrigger("Shoot");
        }
        if (m_entityData.bulletPrefab != null && _firePoint != null)
        {
            Instantiate(m_entityData.bulletPrefab, _firePoint.position, _firePoint.rotation);

        }
    }
    protected override void Die()
    {
        base.Die();
    }
 

}
