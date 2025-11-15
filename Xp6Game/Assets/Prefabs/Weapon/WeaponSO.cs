using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSO", menuName = "Weapons/New WeaponSO", order = 1)]
public class WeaponSO : ScriptableObject
{
    [Header("Base Params")]
    public string WeaponName;
    public string Description;
    public Sprite Icon;
    public int Rarity;
    public Color RarityColor;
    public GameObject meshPrefab;


    [Header("Attack Params")]
    public GameObject BulletPrefab;
    public BulletSO bullet;
    public int MaxAmmo;
    public float ReloadTime;
    public float AttackRange;
    public float AttackRate;
    public float AttackDamage;

    [Header("Components Params")]
    public List<ComponentSO> components;


}
