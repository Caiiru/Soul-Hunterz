using UnityEngine;

[CreateAssetMenu(fileName = "IncreaseDamageSO", menuName = "Components/IncreaseDamageSO")]
public class IncreaseDamageComponentSO : ComponentSO
{
    [Header("Damage Params")]
    public int DamageIncreaseAmount = 5;

    public override void Execute(GameObject target, Transform firePoint, int slotindex)
    {
        if (target.TryGetComponent<Bullet>(out var bullet))
        {
            bullet.BonusDamage += DamageIncreaseAmount;
        }
    }
}
