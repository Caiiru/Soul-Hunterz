using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Collider))]
public class SimpleEnemy : Enemy
{
    private NavMeshAgent _navMesh;
    protected override void OnEnable()
    {
        base.OnEnable();
        _navMesh = GetComponent<NavMeshAgent>();
        _navMesh.speed = speed;
        _navMesh.stoppingDistance = attackRange;
    }

    override protected void TakeDamage(int damage)
    {
        base.TakeDamage(damage); 
    }   
}
