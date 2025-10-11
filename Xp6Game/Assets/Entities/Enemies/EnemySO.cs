using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Data", menuName = "Entity/Enemies/Enemy Data")]
public class EnemySO : EntitySO
{
    public float movementSpeed = 3.5f;
    public float attackRange = 1.5f;

    [Header("Visual Settings")]
    public GameObject hitVFXPrefab;

    [Header("State Machine")]
    public State initialState;
    public State remainState;
    
}
