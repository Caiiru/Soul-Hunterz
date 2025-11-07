using FMODUnity;
using UnityEngine;

public abstract class Entity<T> : MonoBehaviour where T:EntitySO
{
    [Header("Life Settings")]
    public T m_Data;
    [HideInInspector]public T m_entityData;

    public int m_MaxHealth;
    [SerializeField] protected int m_currentHealth = 30;
    [SerializeField] public bool canBeDamaged = true;
 


    protected virtual void OnEnable()
    {
        Initialize();
    }
    public virtual void Initialize()
    {
        if(m_Data == null)
        {
            Debug.LogError($"Entity Data not assigned in: {gameObject.name}");
        }
        m_entityData = Instantiate(m_Data);
        
        m_MaxHealth = m_entityData.maxHealth;
        m_currentHealth = this.m_MaxHealth;
        canBeDamaged = m_entityData.canBeDamaged; 

        transform.name = m_entityData.name;
    }

    public virtual void TakeDamage(int damage)
    {

        if (!canBeDamaged)
            return;

        if (PopupTextManager.instance != null)
        {
            PopupTextManager.instance.ShowPopupText(
                damage.ToString(),
                new Vector3(transform.position.x, transform.position.y + transform.localScale.y + 1, transform.position.z),
                Color.red,
                new Vector3(0.5f, 0.5f, 0.5f));

        }
        m_currentHealth -= damage;


        PlayOneShotAtPosition(m_entityData.takeDamageSound);

        if (m_currentHealth <= 0)
            Die();


    }

    protected virtual void Die()
    {
        PlayOneShotAtPosition(m_entityData.dieSound);
        canBeDamaged = false;
        gameObject.SetActive(false);

    }

    #region Sounds 
    public void PlayOneShotAtPosition(EventReference audioEvent)
    {
        if (audioEvent.IsNull)
            return;

        if (AudioManager.Instance == null)
            return;


        AudioManager.Instance.PlayOneShotAtPosition(audioEvent, transform.position);

    }

    #endregion
}
