using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using StarterAssets;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerEntity : Entity<PlayerEntitySO>
{

    [Header("Player State")]
    [SerializeField] PlayerStates m_PlayerState = PlayerStates.Exploring;

    private Vector3 m_StartPosition;
    private Quaternion m_StartRotation;

    //Anim id hash bind
    int m_TakeDamageIDAnim;

    //COMBAT
    const float k_maxCombatTime = 10f;
    [SerializeField] float m_CombatTime;

    [SerializeField] int m_invencibilityTime;

    [SerializeField] float m_TakeDamageShakeImpulseForce;

    [Header("Visual")]
    public Transform m_DirectionIndicator;

    const int k_milliseconds = 1000;

    [SerializeField] ThirdPersonController m_Controller;

    //Events
    EventBinding<OnGameStart> m_OnGameReadyToStartBinding;
    EventBinding<OnPlayerAttack> m_OnPlayerAttackBinding;
    EventBinding<OnPlayerTakeDamage> m_OnPlayerTakeDamageBinding;

    EventBinding<OnGameWin> m_OnGameWinBinding;





    Animator m_Animator;


    [Header("Debug")]

    [SerializeField] bool m_die = false;
    [SerializeField] bool m_win = false;
    [SerializeField] bool m_takeDamage = false;
    [SerializeField] int m_damage = 10;

    public bool m_debug = false;
    #region Bind 

    void Start()
    {
        // Initialize();
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;

    }
    void BindEvents()
    {

        m_OnGameReadyToStartBinding = new EventBinding<OnGameStart>(HandleGameStart);
        EventBus<OnGameStart>.Register(m_OnGameReadyToStartBinding);



        m_OnPlayerAttackBinding = new EventBinding<OnPlayerAttack>(HandlePlayerAttack);
        EventBus<OnPlayerAttack>.Register(m_OnPlayerAttackBinding);

        ThirdPersonController.onPlayerDash += HandleDashEvent;

        m_OnGameWinBinding = new EventBinding<OnGameWin>(() =>
        {
            ResetPlayer();
        });
        EventBus<OnGameWin>.Register(m_OnGameWinBinding);




    }
    void UnbindEvents()
    {
        Debug.Log("Player Unbind");
        EventBus<OnPlayerAttack>.Unregister(m_OnPlayerAttackBinding);
        EventBus<OnPlayerTakeDamage>.Unregister(m_OnPlayerTakeDamageBinding);
        ThirdPersonController.onPlayerDash -= HandleDashEvent;

    }

    void BindObjects()
    {
        m_Controller = GetComponent<ThirdPersonController>();
    }
    public override void Initialize()
    {
        base.Initialize();
        m_Animator = GetComponentInChildren<Animator>();
        BindEvents();
        BindObjects();
        BindAnim();

        m_DirectionIndicator.gameObject.SetActive(false);
        m_Controller.enabled = false;


        m_invencibilityTime = (int)m_entityData.InvencibilityTime;


        EventBus<OnSetPlayerHealthEvent>.Raise(new OnSetPlayerHealthEvent { maxHealth = m_entityData.m_MaxHealth, currentHealth = m_entityData.m_MaxHealth });

        if (m_debug)
        {
            EventBus<OnGameStart>.Raise(new OnGameStart());
        }


    }

    void BindAnim()
    {
        if (m_Animator == null) return;
        m_TakeDamageIDAnim = Animator.StringToHash("TakeDamage");

    }
    #endregion
    #region Update

    // Update is called once per frame
    void Update()
    {
        HandlePlayerState();
        DebugUpdateHandler();

    }

    #endregion
    #region Events Handlers

    private void HandleGameStart()
    {

        //Reset player position or any other stuff
        Debug.Log("Start Game Player entity");
        m_Controller.enabled = true;
        m_DirectionIndicator.gameObject.SetActive(true);
        m_Animator.SetTrigger("StartGame");
        SetPlayerState(PlayerStates.Exploring);

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
    #endregion






    #region Take Damage
    public override void TakeDamage(int damage)
    {


        if (!canBeDamaged)
        {
            return;
        }

        EventBus<OnPlayerTakeDamage>.Raise(new OnPlayerTakeDamage { value = damage });

        SetPlayerState(PlayerStates.Combat);

        if (m_Animator == null) return;
        m_Animator.SetTrigger(m_TakeDamageIDAnim);

        base.TakeDamage(damage);
    }
    #endregion

    #region Die
    protected async override UniTask Die()
    {
        m_Controller.enabled = false;
        PlayOneShotAtPosition(EntitySoundType.Die);
        canBeDamaged = false;
        m_Animator.SetTrigger("isDead");
        await UniTask.Delay(k_milliseconds * 1);
        var m_AnimationClipInfo = m_Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;

        transform.DOMoveY(transform.position.y - 2f, 2).SetEase(Ease.Linear);

        await UniTask.Delay((int)m_AnimationClipInfo * 1000);

        // UnbindEvents();
        gameObject.SetActive(false);

        ResetPlayer();
        // EventBus<OnGameOver>.Raise(new OnGameOver());
        EventBus<OnPlayerDied>.Raise(new OnPlayerDied());

    }

    void ResetPlayer()
    {
        Debug.Log("Reset Player");
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        m_Controller.enabled = false;
        SetPlayerState(PlayerStates.Exploring);
        gameObject.SetActive(true);
        GetComponentInChildren<Animator>().SetTrigger("ResetGame");
        canBeDamaged = true;



    }

    #endregion
    #region Player States
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


        // if (PopupTextManager.instance != null && m_PlayerState != newState)
        // {
        //     PopupTextManager.instance.ShowPopupText(
        //         $"New State: {newState.ToString()}",
        //         new Vector3(transform.position.x, transform.position.y + transform.localScale.y + 1, transform.position.z),
        //         Color.white,
        //         new Vector3(0.1f, 0.1f, 0.1f));

        // }

        m_PlayerState = newState;
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
    #endregion
    #region Debug
    async void DebugUpdateHandler()
    {
        if (m_die)
        {
            m_die = false;
            await Die();
        }

        if (m_win)
        {
            m_win = false;
            GameManager.Instance.WinGame();
            // ResetPlayer();
        }

        if (m_takeDamage)
        {
            m_takeDamage = false;
            TakeDamage(m_damage);
        }
    }
    #endregion
    #region Unbind

    void OnDestroy()
    {
        UnbindEvents();
    }
    #endregion
}

public enum PlayerStates
{
    Exploring,
    PreCombat,
    Combat,

}
