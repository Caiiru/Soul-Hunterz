using UnityEngine;

[CreateAssetMenu(fileName = "Entity Data", menuName = "Entity/Entity Data")]
public abstract class EntitySO : ScriptableObject
{
    [Header("Base Entity Settings")]
    public int maxHealth = 100;
    public bool canBeDamaged = true;
    public GameObject visualPrefab;


}
