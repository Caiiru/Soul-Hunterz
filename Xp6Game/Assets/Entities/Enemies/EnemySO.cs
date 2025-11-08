using UnityEngine;
 
public class EnemySO : EntitySO
{
    [Header("Enemy Settings")]
    public float m_MoveSpeed = 3.5f;
    public float m_AttackRange = 1.5f;
    public float m_AttackCooldown = 2f;
    public int m_AttackMeleeDamage = 0;
    public Transform Target;

    [Header("VFX Settings")]
    public GameObject m_takeDamageVFX; 

    [Header("State Machine")]
    public State m_InitialState; 

    [HideInInspector]
    public LayerMask playerMask = 1 << 7;
}
