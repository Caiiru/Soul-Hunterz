using UnityEngine;

public class MinionEnemy : Enemy
{
    public bool m_IsMoving;

    Animator m_Animator;

    public override void Initialize()
    {
        base.Initialize();


        //Try cast all types of get component
        m_Animator = GetComponentInChildren<Animator>();
        if (m_Animator == null)
        {
            m_Animator = GetComponent<Animator>();
        }
    }


    public void SetMove(bool isMoving)
    {
        m_IsMoving = isMoving;


    }

    void Update()
    {
        if (_navMesh.hasPath)
        {
            m_Animator.SetFloat("Speed", _navMesh.speed);
            // Debug.Log("Minion has path");
        }
    }
    public bool GetMove()
    {
        return m_IsMoving;
    }
}
