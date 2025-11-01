using FMODUnity;
using UnityEngine;

public class Entity : MonoBehaviour
{

    [SerializeField] protected EntitySO entityData;
    [SerializeField] protected int currentHealth = 30;
    [SerializeField] public bool canBeDamaged = true;

    protected Transform _visualTransform;


    protected virtual void OnEnable()
    {
        Initialize();
    }
    public virtual void Initialize()
    {
        if (entityData == null)
        {
            Debug.LogError("EntitySO is not assigned in " + gameObject.name);
            return;
        }
        currentHealth = entityData.maxHealth;
        canBeDamaged = entityData.canBeDamaged;
        if (_visualTransform) { Destroy(_visualTransform.gameObject); }
        if (entityData.visualPrefab != null)
        {

            _visualTransform = Instantiate(entityData.visualPrefab, transform).transform;
        }



        transform.name = entityData.name;
    }

    public virtual void TakeDamage(int damage)
    {
        if (PopupTextManager.instance != null)
        {
            PopupTextManager.instance.ShowPopupText(
                damage.ToString(),
                new Vector3(transform.position.x, transform.position.y + transform.localScale.y + 1, transform.position.z),
                Color.red);

        }
        if (!canBeDamaged)
            return;

        currentHealth -= damage;
        PlayOneShotAtPosition(entityData.takeDamageSound);

        if (currentHealth <= 0)
            Die();


    }

    protected virtual void Die()
    {
        PlayOneShotAtPosition(entityData.dieSound);
        canBeDamaged = false;
        gameObject.SetActive(false);

    }

    #region Sounds 
    protected void PlayOneShotAtPosition(EventReference audioEvent)
    {
        if (audioEvent.IsNull)
        {
            // Debug.LogWarning("No Audio Event");
            return;
        }
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.PlayOneShotAtPosition(audioEvent, transform.position);

    }

    #endregion
}
