using Cysharp.Threading.Tasks;
using FMODUnity;
using UnityEngine;

public abstract class Entity<T> : MonoBehaviour where T : EntitySO
{
    [Header("Life Settings")]
    public T m_Data;
    [HideInInspector] public T m_entityData;

    public int m_MaxHealth;
    [SerializeField] protected int m_currentHealth = 30;
    [SerializeField] public bool canBeDamaged = true;



    protected virtual void OnEnable()
    {
        Initialize();
    }
    public virtual void Initialize()
    {
        if (m_Data == null)
        {
            Debug.LogError($"Entity Data not assigned in: {gameObject.name}");
        }
        m_entityData = Instantiate(m_Data);

        m_MaxHealth = m_entityData.m_MaxHealth;
        m_currentHealth = this.m_MaxHealth;
        canBeDamaged = m_entityData.m_CanBeDamaged;

        transform.name = m_entityData.name;
    }

    public virtual async void TakeDamage(int damage)
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


        PlayOneShotAtPosition(EntitySoundType.TakeDamage);

        if (m_currentHealth <= 0)
            await Die();


    }

    protected async virtual UniTask Die()
    {
        //PlayOneShotAtPosition(EntitySoundType.Die);
        canBeDamaged = false;
        gameObject.SetActive(false);

        await UniTask.CompletedTask;

    }

    protected int GetSoulValue()
    {
        int _min = m_entityData.m_minSoulAmount;
        int _max = m_entityData.m_maxSoulAmount;
        return Random.Range(_min, _max + 1);
    }

    #region Sounds 
    public void PlayOneShotAtPosition(EntitySoundType audioType)
    {
        EventReference? audioEvent = GetAudioFromString(audioType);

        if (audioEvent == null) return;

        if (AudioManager.Instance == null)
            return;


        AudioManager.Instance.PlayOneShotAtPosition((EventReference)audioEvent, transform.position);

    }

    private EventReference? GetAudioFromString(EntitySoundType audioName)
    {
        for (int i = 0; i < m_entityData.m_SoundsList.Length; i++)
        {
            // Debug.Log($"Comparing {audioName} with {m_entityData.m_SoundsList[i].m_SoundType}");
            if (m_entityData.m_SoundsList[i].m_SoundType == audioName)
            { 
                return m_entityData.m_SoundsList[i].m_SoundReference;
            }
        }

        return null;
    }

    #endregion
}
