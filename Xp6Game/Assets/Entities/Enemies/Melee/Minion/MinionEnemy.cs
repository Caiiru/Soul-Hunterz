using UnityEngine;

public class MinionEnemy : Enemy<EnemySO>
{
    public bool m_IsMoving;


    private int m_PlayerLayerMask;

    Collider[] m_HitCollider;


    public void SetMove(bool isMoving)
    {
        m_IsMoving = isMoving;


    }

    public override void Initialize()
    {
        base.Initialize();
        m_PlayerLayerMask = 1 << 7;
        m_HitCollider = new Collider[5];

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

    public override void Attack()
    {
        if (CanAttack())
        {
            base.Attack();
            // Debug.Log("MINION ATTACK");
            m_animator.SetTrigger("Attack");

            // var m_hitPlayer = Physics.CheckBox(transform.position + Vector3.forward, Vector3.one, Quaternion.identity, m_PlayerLayerMask);
            // Debug.DrawLine(transform.position, transform.position + Vector3.forward, Color.green); 

            int m_hitCount = Physics.OverlapSphereNonAlloc(transform.position + Vector3.forward, m_entityData.m_AttackRange, m_HitCollider, m_PlayerLayerMask);

            if (m_hitCount == 0)
            {
                return;
            }

            foreach (var _hit in m_HitCollider)
            { 
                if(_hit.TryGetComponent<PlayerEntity>(out var _comp)){
                    _comp.TakeDamage(m_entityData.m_AttackMeleeDamage);
                    break;
                }
            }

 
        }
    }

    public bool GetMove()
    {
        return m_IsMoving;
    }



}
