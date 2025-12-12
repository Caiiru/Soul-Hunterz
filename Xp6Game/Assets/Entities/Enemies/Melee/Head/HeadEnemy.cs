using System.Buffers.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class HeadEnemy : Enemy<EnemySO>
{

    Collider[] results = new Collider[10];

    bool m_canAttack;

    void Start()
    {
        m_canAttack = true;
    }

    public override void Update()
    {
        base.Update();
        if (m_navMesh.hasPath)
        {
            m_animator.SetFloat("Speed", m_navMesh.speed);
            // Debug.Log("Minion has path");
        }
    }
    public async override void Attack()
    {
        if (CanAttack() & m_canAttack)
        {

            await HandleAttack();

        }
    }
    private async UniTask HandleAttack()
    {
        m_canAttack = false;
        Physics.OverlapSphereNonAlloc(transform.position, m_entityData.m_AttackRange, results, m_entityData.playerMask);
        if (results.Length > 0)
        {
            if (results[0].TryGetComponent<PlayerEntity>(out PlayerEntity _playerEntity))
            {
                _playerEntity.TakeDamage(m_entityData.m_AttackMeleeDamage);
            }
        }
        await UniTask.Delay(1000 * (int)m_entityData.m_AttackCooldown);
        m_canAttack = true;

    }
}
