using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class TutorialMinionEnemy : Enemy<EnemySO>
{
    public bool m_IsMoving;


    private int m_PlayerLayerMask;

    Collider[] m_HitCollider;

    const int k_Milliseconds = 1000;
    public GameObject m_DropMap;

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
                if (_hit.TryGetComponent<PlayerEntity>(out var _comp))
                {
                    _comp.TakeDamage(m_entityData.m_AttackMeleeDamage);
                    break;
                }
            }


        }
    }
    protected override async UniTask Die()
    {
        // return base.Die();

        if (m_hasNavMesh)
        {
            m_navMesh.enabled = false;
        }

        if (m_stateMachine != null)
        {
            m_stateMachine.SetActive(false);
        }
        this.GetComponent<Collider>().enabled = false;

        m_animator.SetTrigger("isDead");


        await UniTask.Delay(1 * k_Milliseconds);

        var m_AnimationClipInfo = m_animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;


        await UniTask.Delay((int)m_AnimationClipInfo * k_Milliseconds);


        transform.DOMoveY(transform.position.y - 2f, 2).SetEase(Ease.Linear);


        await UniTask.Delay(2 * k_Milliseconds);
        Instantiate(m_DropMap, transform.position, Quaternion.identity);



        await UniTask.Delay(1 * k_Milliseconds);
        Destroy(this.gameObject);
    }

    public bool GetMove()
    {
        return m_IsMoving;
    }



}
