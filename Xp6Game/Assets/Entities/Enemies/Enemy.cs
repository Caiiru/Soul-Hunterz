using System;
using UnityEngine;
using UnityEngine.AI;
using Unity.Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;


[RequireComponent(typeof(Collider))]
public abstract class Enemy<T> : Entity<T> where T : EnemySO
{
    [Space(1)]
    [Header("Enemy Stats")]
    protected float m_speed = 3.5f;
    protected float m_attackRange = 1.5f;
    protected float m_attackCooldown = 2f;

    [SerializeField]
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

    private int k_Milliseconds = 1000;

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

            m_navMesh.speed = m_speed;
            m_navMesh.stoppingDistance = m_attackRange;

        }

        if (TryGetComponent(out StateMachine comp))
        {
            m_stateMachine = comp;

            m_stateMachine.InitializeStateMachine(m_entityData as EnemySO);

            if (!m_hasNavMesh)
                m_stateMachine.SetActive(false);


        }
        m_impulseSource = GetComponent<CinemachineImpulseSource>();

        this.GetComponent<Collider>().enabled = true;


        BindEvents();
    }

    #endregion
    #region Bind Events
    void BindEvents()
    {
        //Local Events
        if (m_stateMachine != null)
        {
            m_stateMachine.m_OnAttack.AddListener(OnAttackEventListener);
            m_stateMachine.m_OnTakeDamage.AddListener(OnTakeDamageEventListener);
        }
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

    protected async override UniTask Die()
    {
        if (m_hasNavMesh)
        {
            m_navMesh.enabled = false;
        }

        if (m_stateMachine != null)
        {
            m_stateMachine.SetActive(false);
        }
        this.GetComponent<Collider>().enabled = false;

        m_animator.SetTrigger("isDead");


        await UniTask.Delay(2 * k_Milliseconds);
        var m_AnimationClipInfo = m_animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;


        await UniTask.Delay((int)m_AnimationClipInfo * k_Milliseconds);
        transform.DOMoveY(transform.position.y - 2f, 2).SetEase(Ease.Linear);

        await UniTask.Delay(3 * k_Milliseconds);

        EventBus<EnemyDiedEvent>.Raise(new EnemyDiedEvent());
        // Debug.Log("Enemy died");
        await base.Die();
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
        if (m_currentHealth <= 0) return false;

        if (m_attackTimer >= 0)
        {
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
