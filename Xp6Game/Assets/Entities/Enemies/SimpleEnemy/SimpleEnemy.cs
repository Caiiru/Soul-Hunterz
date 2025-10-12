using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Collider))]
public class SimpleEnemy : Enemy
{
    private Transform _playerTransform;
    // private NavMeshAgent _navMesh;
    protected override void OnEnable()
    {
        base.OnEnable();

    }

    public override void Initialize()
    {
        base.Initialize();

        _playerTransform = GameManager.Instance.GetPlayer().transform;
    } 
    void Update()
    {
        // MoveTowards(_playerTransform.position);

    }
    override protected void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }
}
