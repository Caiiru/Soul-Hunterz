using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractWeapon : MonoBehaviour
{
    public WeaponSO m_WeaponData;

    public GameObject m_bulletPrefab;
    public int m_currentIndexSlot = 0;
    public ComponentSO[] m_weaponComponents;
    [Space]
    [Header("Stats")]

    public float m_AttackRange;
    public float m_AttackRate;
    public float m_AttackDamage;
    public bool m_CanAttack = true;

    [Header("Ammo")]
    public int m_CurrentAmmo;

    public float m_CurrentReloadTime;
    public float m_ReloadTime;

    [Header("FirePoint")]
    public Transform _firePoint;
    // Visual
    [Header("Visual")]
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
        if (m_WeaponData == null)
        {
            Debug.Log("Weapon Data Null");
            return;
        }
        m_AttackRange = m_WeaponData.AttackRange;
        m_AttackRate = m_WeaponData.AttackRate;
        m_AttackDamage = m_WeaponData.AttackDamage;


        Icon = m_WeaponData.Icon;
        WeaponName = m_WeaponData.WeaponName;
        Description = m_WeaponData.Description;
        Rarity = m_WeaponData.Rarity;
        RarityColor = m_WeaponData.RarityColor;

        m_weaponComponents = m_WeaponData.components.ToArray();
        meshPrefab = m_WeaponData.meshPrefab;

        //Ammo
        m_ReloadTime = m_WeaponData.ReloadTime;
        m_CurrentReloadTime = 0;
        m_CurrentAmmo = m_WeaponData.MaxAmmo;

        if (meshPrefab == null)
        {
            Debug.LogError("Weaponn without mesh");
            return;
        }
        GameObject mesh = Instantiate(m_WeaponData.meshPrefab, transform);
        _firePoint = mesh.transform.Find("FirePoint");

        // _firePoint.SetParent(transform.root);

        // m_bulletPrefab.GetComponent<Bullet>().SetBullet(m_WeaponData.bullet);
        m_bulletPrefab = m_WeaponData.BulletPrefab;
        m_CanAttack = true;

        Debug.Log("Weapon initialized");

    }

    public BulletSO GetBullet()
    {
        return m_WeaponData.bullet;
    }

    public virtual void Update()
    {
        if (m_CurrentReloadTime > 0)
        {
            m_CurrentReloadTime -= Time.deltaTime;
            if (m_CurrentReloadTime <= 0)
            {
                m_CurrentAmmo = m_WeaponData.MaxAmmo;
                m_CanAttack = true;
            }
        }
    }
    public virtual void Attack()
    {


    }
}
