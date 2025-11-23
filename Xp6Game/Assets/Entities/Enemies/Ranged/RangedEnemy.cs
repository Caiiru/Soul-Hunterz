using Cysharp.Threading.Tasks;
using UnityEngine;

public class RangedEnemy : Enemy<RangedEnemySO>
{
    [SerializeField] private Transform _firePoint;

    public float _rotationVelocity = 15f;
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

        if (m_targetTransform == null)
        {
            SetTarget(GameObject.FindGameObjectWithTag("Player").transform);
        }



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
    void RotateTowardsTarget()
    {
        float rot = Mathf.Atan2(m_targetTransform.position.x, m_targetTransform.position.z) * Mathf.Rad2Deg;
        float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, rot, ref _rotationVelocity,
                          0.15f);
        // // rotate to face input direction relative to camera position
        transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
    }
    public override void Update()
    {
        base.Update();

        if (m_targetTransform == null) return;

        // RotateTowardsTarget();
        Vector3 _targetPos = m_targetTransform.transform.position;
        _targetPos.y = transform.position.y;
        transform.LookAt(_targetPos);
    }


}
