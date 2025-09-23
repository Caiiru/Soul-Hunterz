using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractWeapon : MonoBehaviour
{
    public WeaponSO WeaponData;
    public IComponent[] Components;

    public List<ModifierData> ModifierDatas = new List<ModifierData>();
    [Space]
    [Header("Stats")]

    public float AttackRange;
    public float AttackRate;
    public float AttackDamage;


    // Visual

    public Sprite Icon;
    public string WeaponName;
    public string Description;
    public int Rarity;
    public Color RarityColor;
    public GameObject meshPrefab;

    public virtual void Start()
    {
        InitializeWeapon();
    }
    public virtual void InitializeWeapon()
    {
        if (WeaponData != null)
        {
            AttackRange = WeaponData.AttackRange;
            AttackRate = WeaponData.AttackRate;
            AttackDamage = WeaponData.AttackDamage;

            Icon = WeaponData.Icon;
            WeaponName = WeaponData.WeaponName;
            Description = WeaponData.Description;
            Rarity = WeaponData.Rarity;
            RarityColor = WeaponData.RarityColor;

            Components = new IComponent[WeaponData.ModifierCount];
            meshPrefab = WeaponData.meshPrefab;
            if (meshPrefab != null)
            {
                GameObject mesh = Instantiate(WeaponData.meshPrefab, transform);
                Debug.Log("Spawn Absrtact Weapon mesh");
            }
        }
    }
    public virtual void Attack(Transform attackPoint, Vector3 direction) { }
}
