using UnityEngine;

[CreateAssetMenu(fileName = "Entity Data", menuName = "Entity/Entity Data")]
public abstract class EntitySO : ScriptableObject
{
    public int maxHealth = 100;
    public bool canBeDamaged = true;

    public delegate void TakeDamage(int damage);
    public delegate void Die();
    public TakeDamage onTakeDamage;
    public Die onDie;


}
