using System;
using UnityEngine;
using UnityEngine.AI;
using Unity.Cinemachine;
using Unity.VisualScripting;


[RequireComponent(typeof(Collider))]
public abstract class Enemy<T> : Entity<T> where T : EnemySO
{
    [Space(1)]
    [Header("Enemy Stats")]
    protected float m_speed = 3.5f;
    protected float m_attackRange = 1.5f;

    [Tooltip("Does this enemy use a NavMeshAgent for movement?")]
    [SerializeField] bool m_hasNavMesh;
    protected NavMeshAgent m_navMesh;
    protected StateMachine m_stateMachine;


    [SerializeField] private Transform _targetTransform;



    #region Visual 
    [SerializeField] private GameObject _hitVFXInstance;
    public CinemachineImpulseSource impulseSource;


    #endregion

    //Events
    EventBinding<GameWinEvent> m_OnGameWinEventBinding;
    EventBinding<GameOverEvent> m_OnGameOverBinding;



    [Header("Debug Mode", order = 8)]

    [SerializeField] private bool m_DebugMode = true;
    public Animator _animator;


    override protected void OnEnable()
    {

        // entityData = enemyData;
        // base.OnEnable();
        if (m_DebugMode)
        {
            // SetData(entityData as EnemySO);
            Initialize();
        }
        _animator = GetComponentInChildren<Animator>();



    }

    public override void Initialize()
    {
        base.Initialize();

        // Debug.Log($"Enemy Enabled: {gameObject.name} with Speed: {speed} and Attack Range: {attackRange}");

        m_attackRange = m_entityData.attackRange;
        m_speed = m_entityData.movementSpeed;


        if (m_hasNavMesh)
        {
            if (TryGetComponent<NavMeshAgent>(out m_navMesh))
            {
                // Debug.Log("Im have navmesh");
                m_navMesh.enabled = true;
            }
            else
            {
                m_navMesh = transform.AddComponent<NavMeshAgent>();
            }

            m_navMesh.speed = m_speed;
            m_navMesh.stoppingDistance = m_attackRange;

        }

        if (TryGetComponent(out StateMachine comp))
        {
            m_stateMachine = comp;

            m_stateMachine.InitializeStateMachine(m_entityData as EnemySO);
        }
        impulseSource = GetComponent<CinemachineImpulseSource>();

        

        BindEvents();
    }

    void BindEvents()
    {   
        //Local Events
        m_stateMachine.m_OnAttack.AddListener(OnAttackEventListener);
        m_stateMachine.m_OnTakeDamage.AddListener(OnTakeDamageEventListener);

        //Event Bus
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

    private void OnTakeDamageEventListener(int v)
    {
        TakeDamage(v);
    }

    private void OnAttackEventListener()
    {
        Attack();
    }

    void UnbindEvents()
    {
        EventBus<GameOverEvent>.Unregister(m_OnGameOverBinding);
        EventBus<GameWinEvent>.Unregister(m_OnGameWinEventBinding);

        
        m_stateMachine.m_OnAttack.RemoveListener(OnAttackEventListener);
        m_stateMachine.m_OnTakeDamage.RemoveListener(OnTakeDamageEventListener);
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

        if (_animator)
            _animator.SetTrigger("TakeDamage");



        if (cameraShakeManager.instance != null && impulseSource != null)
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
        if (m_entityData.hitVFXPrefab != null)
        {
            if (_hitVFXInstance != null)
            {
                Destroy(_hitVFXInstance);
            }
            _hitVFXInstance = Instantiate(m_entityData.hitVFXPrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);
            Destroy(_hitVFXInstance, 2f);
        }
    }

    #endregion

    #region Movement
    public void MoveTowards(Vector3 targetPosition)
    {
        if (!m_hasNavMesh || m_navMesh == null) return;

        m_navMesh.SetDestination(targetPosition);

    }

    #endregion

    #region Attack 

    public virtual void Attack()
    {
        // PlayOneShotAtPosition
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
        return distanceToTarget <= m_attackRange;
    }

    public float GetAttackRange()
    {
        return m_attackRange;
    }

    #endregion
}
