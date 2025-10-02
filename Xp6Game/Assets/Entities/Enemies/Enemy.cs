using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Enemy : Entity
{

    protected override void Start()
    {
        base.Start();

    }

    // Update is called once per frame
    protected override void Update()
    {

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
        // Notify Game Manager
        GameManager.Instance.EnemyDefeated();

        gameObject.SetActive(false);
    }
}
