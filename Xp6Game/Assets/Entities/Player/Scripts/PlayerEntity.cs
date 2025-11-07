using System;
using Cysharp.Threading.Tasks;
using StarterAssets;
using UnityEngine;

public class PlayerEntity : Entity<PlayerEntitySO>
{

    [Header("Player State")]
    [SerializeField] PlayerStates m_PlayerState = PlayerStates.Exploring;

    //Anim id hash bind
    int m_TakeDamageIDAnim;

    //COMBAT
    const float k_maxCombatTime = 10f;
    [SerializeField] float m_CombatTime;

    [SerializeField] int m_invencibilityTime;

    //POST COMBAT

    const float k_maxPostCombatTime = 10f;
    [SerializeField] float m_PostCombatTime;

    const int k_milliseconds = 1000;

    //Events

    EventBinding<OnPlayerAttack> m_OnPlayerAttackBinding;
    EventBinding<OnPlayerTakeDamage> m_OnPlayerTakeDamageBinding;



    Animator m_Animator;


    [Header("Debug")]

    [SerializeField] bool m_die = false;
    void BindEvents()
    {
        m_OnPlayerAttackBinding = new EventBinding<OnPlayerAttack>(HandlePlayerAttack);
        EventBus<OnPlayerAttack>.Register(m_OnPlayerAttackBinding);

        ThirdPersonController.onPlayerDash += HandleDashEvent;

    }
    void UnbindEvents()
    {
        EventBus<OnPlayerAttack>.Unregister(m_OnPlayerAttackBinding);
        EventBus<OnPlayerTakeDamage>.Unregister(m_OnPlayerTakeDamageBinding);
        ThirdPersonController.onPlayerDash -= HandleDashEvent;

    }

    void BindObjects()
    {

    }
    public override void Initialize()
    {
        base.Initialize();
        m_Animator = GetComponentInChildren<Animator>();
        BindEvents();
        BindObjects();
        BindAnim();


        m_invencibilityTime = (int)m_entityData.InvencibilityTime;


        EventBus<OnSetPlayerHealthEvent>.Raise(new OnSetPlayerHealthEvent { maxHealth = m_entityData.maxHealth, currentHealth = m_entityData.maxHealth });


    }

    // Update is called once per frame
    void Update()
    {
        HandlePlayerState();
        DebugUpdateHandler();

    }

    private void HandleDashEvent()
    {
        var _ = HandleInvencibility();
    }

    private async UniTask HandleInvencibility()
    {

        canBeDamaged = false;
        await UniTask.Delay(m_invencibilityTime * k_milliseconds);
        canBeDamaged = true;

    }




    void BindAnim()
    {
        if (m_Animator == null) return;
        m_TakeDamageIDAnim = Animator.StringToHash("TakeDamage");

    }




    public override void TakeDamage(int damage)
    {



        EventBus<OnPlayerTakeDamage>.Raise(new OnPlayerTakeDamage { value = damage });
        SetPlayerState(PlayerStates.Combat);



        if (m_Animator == null) return;
        m_Animator.SetTrigger(m_TakeDamageIDAnim);

        base.TakeDamage(damage);
    }

    protected override void Die()
    {
        EventBus<GameOverEvent>.Raise(new GameOverEvent());
        // base.Die();
        // GameManager.Instance.LoseGame();

    }

    private void HandlePlayerAttack(OnPlayerAttack arg0)
    {
        m_CombatTime = k_maxCombatTime;
        if (m_PlayerState == PlayerStates.Exploring || m_PlayerState == PlayerStates.PreCombat)
        {
            SetPlayerState(PlayerStates.Combat);
        }
    }
    public void SetPlayerState(PlayerStates newState)
    {
        EventBus<OnPlayerChangeState>.Raise(new OnPlayerChangeState { newState = newState });
        m_PlayerState = newState;

        if (PopupTextManager.instance != null)
        {
            PopupTextManager.instance.ShowPopupText(
                $"New State: {newState.ToString()}",
                new Vector3(transform.position.x, transform.position.y + transform.localScale.y + 1, transform.position.z),
                Color.white,
                new Vector3(0.1f, 0.1f, 0.1f));

        }

        switch (newState)
        {
            case PlayerStates.Exploring:
                break;
            case PlayerStates.PreCombat:
                break;
            case PlayerStates.Combat:
                m_CombatTime = k_maxCombatTime;
                break;

            default:
                break;
        }
    }
    void HandlePlayerState()
    {

        switch (m_PlayerState)
        {
            case PlayerStates.Exploring:
                break;
            case PlayerStates.PreCombat:
                break;
            case PlayerStates.Combat:

                m_CombatTime -= Time.deltaTime;
                if (m_CombatTime <= 0)
                {
                    // m_PostCombatTime = k_maxPostCombatTime;
                    SetPlayerState(PlayerStates.Exploring);
                }
                break;

            default:
                break;
        }

    }
    void DebugUpdateHandler()
    {
        if (m_die)
        {
            m_die = false;
            Die();
        }
    }

    void OnDestroy()
    {
        UnbindEvents();
    }
}

public enum PlayerStates
{
    Exploring,
    PreCombat,
    Combat,

}
