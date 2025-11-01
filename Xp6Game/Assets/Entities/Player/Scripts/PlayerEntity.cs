using System;
using UnityEngine;

public class PlayerEntity : Entity
{

    [SerializeField] PlayerStates m_PlayerState = PlayerStates.Exploring;
    Animator m_Animator;

    //Anim id hash bind

    int m_TakeDamageIDAnim;

    //COMBAT
    const float k_maxCombatTime = 10f;
    [SerializeField] float m_CombatTime;

    //POST COMBAT

    const float k_maxPostCombatTime = 10f;
    [SerializeField] float m_PostCombatTime;

    //Events

    EventBinding<OnPlayerAttack> m_OnPlayerAttackBinding;
    EventBinding<OnPlayerTakeDamage> m_OnPlayerTakeDamageBinding;

    void BindEvents()
    {
        m_OnPlayerAttackBinding = new EventBinding<OnPlayerAttack>(HandlePlayerAttack);
        EventBus<OnPlayerAttack>.Register(m_OnPlayerAttackBinding);

    }

    void UnbindEvents()
    {
        EventBus<OnPlayerAttack>.Unregister(m_OnPlayerAttackBinding);
        EventBus<OnPlayerTakeDamage>.Unregister(m_OnPlayerTakeDamageBinding);

    }



    void BindAnim()
    {
        if (m_Animator == null) return;
        m_TakeDamageIDAnim = Animator.StringToHash("TakeDamage");

    }
    public override void Initialize()
    {
        base.Initialize();

        m_Animator = GetComponentInChildren<Animator>();
        BindEvents();
        BindAnim();
    }

    // Update is called once per frame
    void Update()
    {
        HandlePlayerState();
    }



    public override void TakeDamage(int damage)
    {

        base.TakeDamage(damage);


        EventBus<OnPlayerTakeDamage>.Raise(new OnPlayerTakeDamage());
        SetPlayerState(PlayerStates.Combat);



        if (m_Animator == null) return;
        m_Animator.SetTrigger(m_TakeDamageIDAnim);

    }

    protected override void Die()
    {
        base.Die();
        // GameManager.Instance.LoseGame();
        EventBus<GameOverEvent>.Raise(new GameOverEvent());

    }

    private void HandlePlayerAttack(OnPlayerAttack arg0)
    {
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

