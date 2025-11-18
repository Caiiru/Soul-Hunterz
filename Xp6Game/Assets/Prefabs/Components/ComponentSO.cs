using UnityEngine;
using UnityEngine.UI;

// [CreateAssetMenu(fileName = "New Component", menuName = "Components/New Component")]
public abstract class ComponentSO : ScriptableObject
{
    public string ComponentName;
    public string Description;
    public int Rarity;

    [PreviewSprite]
    public Sprite Icon;

    public WeaponStats[] weaponStats;

    public virtual BulletPayload InitializeOnWeapon(BulletPayload payload)
    {
        if (weaponStats == null)
            return payload;

        foreach (WeaponStats stats in weaponStats)
        {
            // Debug.Log(stats);
            payload = InitializeFromEnum(payload, stats);
        }
        return payload;
    }

    /// <summary>
    /// Executa a lógica do componente, modificando o payload da bala.
    /// </summary>
    /// <param name="payload">O payload a ser modificado.</param>
    /// <param name="firePoint">O ponto de origem do disparo.</param>
    /// <param name="slotIndex">O índice do slot do componente na arma.</param>
    /// <returns>O payload modificado.</returns>
    public virtual BulletPayload Execute(BulletPayload payload, Transform firePoint, int slotIndex)
    {
        foreach (WeaponStats stats in weaponStats)
        {
            payload = ExecuteAttributesFromEnum(payload, stats);
        }
        return payload;
    }
    public abstract void ComponentUpdate(Bullet bullet);

    public BulletPayload ExecuteAttributesFromEnum(BulletPayload p, WeaponStats stats)
    {
        // Debug.Log($"LoadAttributeFromEnum case{stats.m_WeaponAttribute}");

        //Only Bullet Settings
        switch (stats.m_WeaponAttribute)
        {
            case WeaponAttribute.Damage:
                p.BonusDamage += stats.m_Value;
                return p;

            case WeaponAttribute.SpeedMultiplier:
                p.SpeedMultiplier += stats.m_Value;
                return p;
            case WeaponAttribute.SpeedFlat:
                p.SpeedFlat += stats.m_Value;
                return p;

            case WeaponAttribute.LifetimeMultiplier:
                p.LifetimeMultiplier += stats.m_Value;
                return p;

            case WeaponAttribute.FlatLifeTime:
                p.FlatLifeTime += stats.m_Value;
                return p;


            default:
                break;
        }
        return p;
    }
    public BulletPayload InitializeFromEnum(BulletPayload p, WeaponStats stats)
    {
        //Weapon Settings
        switch (stats.m_WeaponAttribute)
        {
            case WeaponAttribute.Damage:
                p.BonusDamage += stats.m_Value;
                return p;

            case WeaponAttribute.SpeedMultiplier:
                p.SpeedMultiplier += stats.m_Value;
                return p;
            case WeaponAttribute.SpeedFlat:
                p.SpeedFlat += stats.m_Value;
                return p;

            case WeaponAttribute.LifetimeMultiplier:
                p.LifetimeMultiplier += stats.m_Value;
                return p;

            case WeaponAttribute.FlatLifeTime:
                p.FlatLifeTime += stats.m_Value;
                return p;

            case WeaponAttribute.FireDelay:
                p.AttackDelay += stats.m_Value;
                return p;

            case WeaponAttribute.RechargeTime:
                p.RechargeTime += stats.m_Value;
                return p;

            case WeaponAttribute.MaxAmmo:
                p.MaxAmmo += (int)stats.m_Value;
                return p;

            default:
                break;
        }
        return p;
    }

}

public enum WeaponAttribute
{
    Damage,
    SpeedFlat,
    SpeedMultiplier,
    FlatLifeTime,
    LifetimeMultiplier,
    FireDelay,
    RechargeTime,
    MaxAmmo
}

[System.Serializable]
public struct WeaponStats
{
    public string name;
    public WeaponAttribute m_WeaponAttribute;
    public float m_Value;
    public string m_Description;
}
