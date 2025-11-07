using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Collider))]
public class SimpleEnemy : Enemy<EnemySO>
{ 
    // private NavMeshAgent _navMesh;
    protected override void OnEnable()
    {
        base.OnEnable();

    }
 
    void Update()
    {
        // MoveTowards(_playerTransform.position);

    }
    override public void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }
}
