using UnityEngine;

[CreateAssetMenu(fileName = "IncreaseDamageSO", menuName = "Components/IncreaseDamageSO")]
public class IncreaseDamageComponentSO : ComponentSO
{
    [Header("Damage Params")]
    public int DamageIncreaseAmount = 5;

    public override void ComponentUpdate(Bullet bullet)
    {
        // throw new System.NotImplementedException();
    }

    public override BulletPayload Execute(BulletPayload payload, Transform firePoint, int slotIndex)
    {
        payload.BonusDamage += DamageIncreaseAmount;
        return payload;
    }
}
