using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Collider))]
public class Enemy : Entity
{
    [HideInInspector] public EnemySO enemyData;
    protected float speed = 3.5f;
    protected float attackRange = 1.5f;

    [Tooltip("Does this enemy use a NavMeshAgent for movement?")]
    [SerializeField] bool hasNavMesh;
    protected NavMeshAgent _navMesh;


    #region Visual 
    private GameObject _hitVFXInstance;

    #endregion


    override protected void OnEnable()
    {
        enemyData = (EnemySO)entityData;

        base.OnEnable();

        speed = enemyData.movementSpeed;
        attackRange = enemyData.attackRange;

        Debug.Log($"Enemy Enabled: {gameObject.name} with Speed: {speed} and Attack Range: {attackRange}");

        if (hasNavMesh)
        {
            _navMesh = GetComponent<NavMeshAgent>();
            _navMesh.speed = speed;
            _navMesh.stoppingDistance = attackRange;
        }

    }


    #region Take Damage & Death
    protected override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);


    }

    protected override void Die()
    {
        base.Die();
        GameManager.Instance.EnemyDefeated();
    }

    protected virtual void SpawnHitVFX()
    {
        if (enemyData.hitVFXPrefab != null)
        {
            if (_hitVFXInstance != null)
            {
                Destroy(_hitVFXInstance);
            }
            _hitVFXInstance = Instantiate(enemyData.hitVFXPrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);
            Destroy(_hitVFXInstance, 2f);
        }
    }

    #endregion

    #region Movement
    public void MoveTowards(Vector3 targetPosition)
    {
        if (!hasNavMesh || _navMesh == null) return;

        _navMesh.SetDestination(targetPosition);
        // }
        // else
        // {
        //     // Simple movement without NavMesh
        //     Vector3 direction = (targetPosition - transform.position).normalized;
        //     transform.position += direction * speed * Time.deltaTime;
        //     // Optional: Rotate to face the target
        //     if (direction != Vector3.zero)
        //     {
        //         Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
        //         transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * 5f);
        //     }
        // }
    }

    #endregion
    
    #region Attack 
    protected virtual void Attack()
    {
        Debug.Log("Enemy Attacking");
    }

    #endregion
}
