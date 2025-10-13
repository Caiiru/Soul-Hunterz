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
        if (entityData.visualPrefab != null)
        {
            _visualTransform = Instantiate(entityData.visualPrefab, transform).transform;
        }

        
        transform.name = entityData.name; 
    }

    protected virtual void TakeDamage(int damage)
    {
        PopupTextManager.instance.ShowPopupText(
            damage.ToString(),
            new Vector3(transform.position.x, transform.position.y + transform.localScale.y + 1, transform.position.z),
            Color.red);
        if (!canBeDamaged)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0)
            Die();


    }

    protected virtual void Die()
    {
        canBeDamaged = false;
        gameObject.SetActive(false);
 
    }
}
