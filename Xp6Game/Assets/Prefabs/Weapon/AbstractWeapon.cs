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

    [Header("VFX")]
    public GameObject m_MuzzleGO;


    //Events

    EventBinding<OnComponentUpdate> m_OnComponentUpdateBinding;

    public virtual void Start()
    {
        BindEvents();
    }
    void OnEnable()
    {
        // InitializeWeapon();
    }

    protected void BindEvents()
    {
        m_OnComponentUpdateBinding = new EventBinding<OnComponentUpdate>(ReadComponents);
        EventBus<OnComponentUpdate>.Register(m_OnComponentUpdateBinding);

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

        //Ammo
        m_RechargeTime = m_WeaponData.ReloadTime;
        m_CurrentRechargeTime = m_RechargeTime;

        GameObject mesh = Instantiate(m_WeaponData.meshPrefab, transform);
        _firePoint = mesh.transform.Find("FirePoint");

        // _firePoint.SetParent(transform.root);

        // m_bulletPrefab.GetComponent<Bullet>().SetBullet(m_WeaponData.bullet);
        m_bulletPrefab = m_WeaponData.BulletPrefab;
        m_CanAttack = true;

        m_MuzzleGO = m_WeaponData.m_MuzzleVFX;

        var payload = new BulletPayload();
        //Load weapon payload

        ReadComponents();

        // m_maxAmmo = m_WeaponData.MaxAmmo;
        // m_maxAmmo += payload.MaxAmmo;


        m_CurrentAmmo = m_WeaponData.MaxAmmo;


        UpdateAmmoVisual();


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
                m_CurrentAmmo = m_maxAmmo;

                UpdateAmmoVisual();

                EventBus<OnEndedRechargeTime>.Raise(new OnEndedRechargeTime());
                m_CanAttack = true;
            }
        }
    }

    public BulletPayload EncodeWeaponStatsOnPayload()
    {
        var payload = new BulletPayload();
        //Load weapon payload
        BulletSO bulletData = GetBullet();

        payload.AttackDelay = m_FireDelay;
        payload.SpeedFlat = bulletData.Speed;
        payload.SpeedMultiplier = 1;
        payload.FlatLifeTime = bulletData.LifeTime;
        payload.LifetimeMultiplier = 1;
        payload.BonusDamage = 0;
        payload.RechargeTime = m_RechargeTime;
        payload.MaxAmmo = m_WeaponData.MaxAmmo;

        return payload;
    }
    /// <summary>
    /// when player put a new component, update the weapon payload & stats
    /// </summary>
    public void ReadComponents()
    {

        var payload = EncodeWeaponStatsOnPayload();
        foreach (var component in m_weaponComponents)
        {
            if (component == null) continue;
            payload = component.InitializeOnWeapon(payload);
        }

        //Weapon Stats
        m_maxAmmo = payload.MaxAmmo;
        m_CurrentAmmo = m_CurrentAmmo > m_maxAmmo ? m_maxAmmo : m_CurrentAmmo;

        m_FireDelay = payload.AttackDelay;
        m_RechargeTime = payload.RechargeTime;

        UpdateAmmoVisual();


    }

    private void UpdateAmmoVisual()
    {
        EventBus<OnAmmoChanged>.Raise(new OnAmmoChanged
        {
            currentAmmo = m_CurrentAmmo,
            maxAmmo = m_maxAmmo
        });
    }

    public virtual void Attack()
    {
        if (!m_CanAttack) return;
        GameObject _muzzle = Instantiate(m_MuzzleGO, _firePoint.position, _firePoint.rotation);
        Destroy(_muzzle, 5f);

    }
}
