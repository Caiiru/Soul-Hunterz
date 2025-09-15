using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Enemy : MonoBehaviour
{
    [SerializeField]
    int maxHealth = 100;
    [SerializeField]
    int currentHealth = 100;

    [SerializeField]
    bool canBeDamaged = true;

    void Start()
    {
        currentHealth = maxHealth;

    }

    // Update is called once per frame
    void Update()
    {

    }

    protected virtual void TakeDamage(int damage)
    {
        if (!canBeDamaged)
            return;


        currentHealth -= damage;
        if (currentHealth <= 0)
            Die();
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
