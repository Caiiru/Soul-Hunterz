using System;
using UnityEngine;
using UnityEngine.AI;
using Unity.Cinemachine;


[RequireComponent(typeof(Collider))]
public class Enemy : Entity
{
    [HideInInspector] public EnemySO enemyData;
    [Space(1)]
    [Header("Enemy Stats")]
    protected float speed = 3.5f;
    protected float attackRange = 1.5f;

    [Tooltip("Does this enemy use a NavMeshAgent for movement?")]
    [SerializeField] bool hasNavMesh;
    protected NavMeshAgent _navMesh;
    StateMachine _stateMachine;


    [SerializeField] private Transform _targetTransform;

    [Header("Debug Mode")]

    [SerializeField] private bool m_DebugMode = true;


    #region Visual 
    [SerializeField] private GameObject _hitVFXInstance;
    public CinemachineImpulseSource impulseSource;


    #endregion

    //Events
    EventBinding<GameWinEvent> m_OnGameWinEventBinding;
    EventBinding<GameOverEvent> m_OnGameOverBinding;


    public virtual void SetData(EnemySO newData)
    {
        enemyData = newData;
        entityData = enemyData;
    }

    override protected void OnEnable()
    {

        // entityData = enemyData;
        // base.OnEnable();
        if (m_DebugMode)
        {
            SetData(entityData as EnemySO);
            Initialize();
        }


    }

    public override void Initialize()
    {
        base.Initialize();
        attackRange = enemyData.attackRange;
        speed = enemyData.movementSpeed;

        // Debug.Log($"Enemy Enabled: {gameObject.name} with Speed: {speed} and Attack Range: {attackRange}");

        if (hasNavMesh)
        {
            _navMesh = GetComponent<NavMeshAgent>();
            _navMesh.speed = speed;
            _navMesh.stoppingDistance = attackRange;
        }
        if (TryGetComponent(out StateMachine comp))
        {
            _stateMachine = comp;

            _stateMachine.InitializeStateMachine();
        }
        impulseSource = GetComponent<CinemachineImpulseSource>();

        BindEvents();
    }

    void BindEvents()
    {
        m_OnGameOverBinding = new EventBinding<GameOverEvent>(() =>
        {
            EndGame();
        });
        EventBus<GameOverEvent>.Register(m_OnGameOverBinding);

        m_OnGameWinEventBinding = new EventBinding<GameWinEvent>(() =>
        {
            EndGame();
        });
        EventBus<GameWinEvent>.Register(m_OnGameWinEventBinding);
    }
    void UnbindEvents()
    {
        EventBus<GameOverEvent>.Unregister(m_OnGameOverBinding);
        EventBus<GameWinEvent>.Unregister(m_OnGameWinEventBinding);
    }

    void EndGame()
    {
        UnbindEvents();
        Destroy(this.gameObject);
    }


    #region Take Damage & Death
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);


        if (cameraShakeManager.instance != null)
            cameraShakeManager.instance.CameraShake(impulseSource);


    }

    protected override void Die()
    {
        EventBus<EnemyDiedEvent>.Raise(new EnemyDiedEvent());
        // Debug.Log("Enemy died");
        base.Die();
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

    public virtual void Attack()
    {

    }
    public virtual void SetTarget(Transform targetTransform)
    {
        this._targetTransform = targetTransform;

    }
    public virtual bool HasTarget()
    {
        return _targetTransform != null;
    }
    public virtual Transform GetTarget()
    {
        return _targetTransform;
    }

    public virtual bool CanAttack()
    {
        if (!HasTarget()) return false;

        float distanceToTarget = Vector3.Distance(transform.position, GetTarget().transform.position);
        return distanceToTarget <= attackRange;
    }

    public float GetAttackRange()
    {
        return attackRange;
    }

    #endregion
}
