using UnityEngine;

[CreateAssetMenu(fileName = "Entity Data", menuName = "Entity/Entity Data")]
public abstract class EntitySO : ScriptableObject
{
    public int maxHealth = 100;
    public bool canBeDamaged = true;


}
