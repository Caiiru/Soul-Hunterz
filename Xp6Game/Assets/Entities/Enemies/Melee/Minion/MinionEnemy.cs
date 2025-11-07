using UnityEngine;

public class MinionEnemy : Enemy<EnemySO>
{
    public bool m_IsMoving;




    public void SetMove(bool isMoving)
    {
        m_IsMoving = isMoving;


    }

    void Update()
    {
        if (m_navMesh.hasPath)
        {
            _animator.SetFloat("Speed", m_navMesh.speed);
            // Debug.Log("Minion has path");
        }
    }
    public bool GetMove()
    {
        return m_IsMoving;
    }
    public void PlayWalkSound()
    {
        PlayOneShotAtPosition(m_entityData.walkSound);
    }



}
