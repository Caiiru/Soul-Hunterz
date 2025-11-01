using UnityEngine;

public class PlayerEntity : Entity
{
    Animator m_Animator;

    //Anim id hash bind

    [SerializeField] int m_TakeDamageIDAnim;
    public override void Initialize()
    {
        base.Initialize();

        m_Animator = GetComponentInChildren<Animator>();

        BindAnim();
    }

    // Update is called once per frame
    void Update()
    { 
    }

    void BindAnim()
    {
        if (m_Animator == null) return;
        m_TakeDamageIDAnim = Animator.StringToHash("TakeDamage"); 

    }

    public override void TakeDamage(int damage)
    {

        base.TakeDamage(damage);
        EventBus<OnPlayerTakeDamage>.Raise(new OnPlayerTakeDamage());
        
        if (m_Animator == null) return;
        m_Animator.SetTrigger(m_TakeDamageIDAnim); 
        
    }

    protected override void Die()
    {
        base.Die();
        // GameManager.Instance.LoseGame();
        EventBus<GameOverEvent>.Raise(new GameOverEvent());

    }
}

