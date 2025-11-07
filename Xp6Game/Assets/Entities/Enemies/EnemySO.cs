using UnityEngine;
 
public class EnemySO : EntitySO
{
    [Header("Enemy Settings")]
    public float movementSpeed = 3.5f;
    public float attackRange = 1.5f;
    public Transform Target;

    [Header("VFX Settings")]
    public GameObject hitVFXPrefab;

    [Header("State Machine")]
    public State initialState;
    public State remainState;

    [HideInInspector]
    public LayerMask playerMask = 1 << 7;
}
