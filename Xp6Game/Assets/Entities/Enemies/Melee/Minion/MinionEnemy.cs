using UnityEngine;

public class MinionEnemy : Enemy<EnemySO>
{
    public bool m_IsMoving;




    public void SetMove(bool isMoving)
    {
        m_IsMoving = isMoving;


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
        base.Attack();

    }

    public bool GetMove()
    {
        return m_IsMoving;
    } 



}
