using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSO", menuName = "Weapons/New WeaponSO", order = 1)]
public class WeaponSO : ScriptableObject
{
    public string WeaponName;
    public string Description;
    public Sprite Icon;
    public int Rarity;
    public Color RarityColor;
    public GameObject meshPrefab;

    public float AttackRange;
    public float AttackRate;
    public float AttackDamage;

    public int ModifierCount;
    // Visual


}
