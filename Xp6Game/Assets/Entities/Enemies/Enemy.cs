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
    protected float m_attackCooldown = 2f;

    private float m_attackTimer = 0;

    [Tooltip("Does this enemy use a NavMeshAgent for movement?")]
    [SerializeField] bool m_hasNavMesh;
    protected NavMeshAgent m_navMesh;
    protected StateMachine m_stateMachine;


    [SerializeField] private Transform m_targetTransform;



    #region Visual 
    [SerializeField] private GameObject m_hitVFXInstance;
    public CinemachineImpulseSource m_impulseSource;


    #endregion

    //Events
    EventBinding<GameWinEvent> m_OnGameWinEventBinding;
    EventBinding<GameOverEvent> m_OnGameOverBinding;



    [Header("Debug Mode", order = 8)]

    [SerializeField] private bool m_DebugMode = true;
    public Animator m_animator;

    #region Initialize

    override protected void OnEnable()
    {

        // entityData = enemyData;
        // base.OnEnable();
        if (m_DebugMode)
        {
            // SetData(entityData as EnemySO);
            Initialize();
        }
        m_animator = GetComponentInChildren<Animator>();



    }

    public override void Initialize()
    {
        base.Initialize();

        // Debug.Log($"Enemy Enabled: {gameObject.name} with Speed: {speed} and Attack Range: {attackRange}");

        m_attackRange = m_entityData.m_AttackRange;
        m_speed = m_entityData.m_MoveSpeed;
        m_attackCooldown = m_entityData.m_AttackCooldown;


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
        m_impulseSource = GetComponent<CinemachineImpulseSource>();



        BindEvents();
    }

    #endregion
    #region Bind Events
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
    #endregion
    #region End Game
    void EndGame()
    {
        UnbindEvents();
        Destroy(this.gameObject);
    }

    #endregion
    #region Update

    public virtual void Update()
    {
        HandleAttackTimer();
    }

    #endregion


    #region Take Damage & Death
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        if (m_animator)
            m_animator.SetTrigger("TakeDamage");



        if (cameraShakeManager.instance != null && m_impulseSource != null)
            cameraShakeManager.instance.CameraShake(m_impulseSource);


    }

    protected override void Die()
    {
        EventBus<EnemyDiedEvent>.Raise(new EnemyDiedEvent());
        // Debug.Log("Enemy died");
        base.Die();
    }

    protected virtual void SpawnHitVFX()
    {
        if (m_entityData.m_takeDamageVFX != null)
        {
            if (m_hitVFXInstance != null)
            {
                Destroy(m_hitVFXInstance);
            }
            m_hitVFXInstance = Instantiate(m_entityData.m_takeDamageVFX, transform.position + Vector3.up * 1.5f, Quaternion.identity);
            Destroy(m_hitVFXInstance, 2f);
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
        m_attackTimer = m_attackCooldown;

    }
    public virtual void SetTarget(Transform targetTransform)
    {
        this.m_targetTransform = targetTransform;

    }
    public virtual bool HasTarget()
    {
        return m_targetTransform != null;
    }
    public virtual Transform GetTarget()
    {
        return m_targetTransform;
    }

    public virtual bool CanAttack()
    {
        if (m_attackTimer >= 0)
        {
            m_attackTimer -= Time.deltaTime;
            return false;
        }

        return true;
    }

    public virtual void HandleAttackTimer()
    {
        if (m_attackTimer >= 0)
        {
            m_attackTimer -= Time.deltaTime;
        }
    }

    public float GetAttackRange()
    {
        return m_attackRange;
    }

    #endregion
}
