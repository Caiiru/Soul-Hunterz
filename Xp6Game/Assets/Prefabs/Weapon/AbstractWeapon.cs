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
    public float m_AttackDamage;
    public bool m_CanAttack = true;

    [Header("Ammo")]
    public int m_CurrentAmmo;
    public int m_maxAmmo;
    [Header("Fire Delay")]
    public float m_CurrentFireDelay;
    public float m_FireDelay;

    [Header("Reload Time")]

    public float m_CurrentRechargeTime;
    public float m_RechargeTime;

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
        m_FireDelay = m_WeaponData.AttackDelay;
        m_AttackDamage = m_WeaponData.AttackDamage;


        Icon = m_WeaponData.Icon;
        WeaponName = m_WeaponData.WeaponName;
        Description = m_WeaponData.Description;
        Rarity = m_WeaponData.Rarity;
        RarityColor = m_WeaponData.RarityColor;

        m_weaponComponents = m_WeaponData.components.ToArray();
        meshPrefab = m_WeaponData.meshPrefab;

        //Ammo
        m_RechargeTime = m_WeaponData.ReloadTime;
        m_CurrentRechargeTime = 0;
        m_CurrentAmmo = m_WeaponData.MaxAmmo;
        m_maxAmmo = m_WeaponData.MaxAmmo;

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
        HandleFireRateTimer();
        HandleRechargeTimer();
    }

    public virtual void HandleFireRateTimer()
    {
        if (m_CurrentFireDelay < m_FireDelay)
        {
            m_CurrentFireDelay += Time.deltaTime;




            if (m_CurrentFireDelay > m_FireDelay && m_CurrentAmmo > 0)
            {

                m_CanAttack = true;

            }
        }

    }
    public virtual void HandleRechargeTimer()
    {
        if (m_CurrentRechargeTime < m_RechargeTime)
        {
            m_CurrentRechargeTime += Time.deltaTime;

            EventBus<OnUpdatedRechargeTime>.Raise(new OnUpdatedRechargeTime
            {
                time = m_CurrentRechargeTime,
                maxTime = m_RechargeTime
            });

            if (m_CurrentRechargeTime >= m_RechargeTime)
            {
                m_CurrentAmmo = m_WeaponData.MaxAmmo;


                EventBus<OnAmmoChanged>.Raise(new OnAmmoChanged
                {
                    currentAmmo = m_CurrentAmmo,
                    maxAmmo = m_maxAmmo
                });

                EventBus<OnEndedRechargeTime>.Raise(new OnEndedRechargeTime());
                m_CanAttack = true;
            }
        }
    }
    public virtual void Attack()
    {


    }
}
