using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractWeapon : MonoBehaviour
{
    public WeaponSO WeaponData;

    public GameObject bulletPrefab;
    public List<ComponentSO> ComponentList = new List<ComponentSO>();
    [Space]
    [Header("Stats")]

    public float AttackRange;
    public float AttackRate;
    public float AttackDamage;

    public Transform _firePoint;
    // Visual

    public Sprite Icon;
    public string WeaponName;
    public string Description;
    public int Rarity;
    public Color RarityColor;
    public GameObject meshPrefab;

    public virtual void Start()
    {
    }
    void OnEnable()
    {
        // InitializeWeapon();
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

            ComponentList = WeaponData.components;
            meshPrefab = WeaponData.meshPrefab;
            if (meshPrefab == null)
            {
                Debug.LogError("Weaponn without mesh");
                return; 
            }
            GameObject mesh = Instantiate(WeaponData.meshPrefab, transform);
            _firePoint = mesh.transform.GetChild(0).transform;

            bulletPrefab = WeaponData.bullet;

        }
    }
    public virtual void Attack() { }
}
