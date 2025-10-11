using UnityEngine;

[CreateAssetMenu(fileName = "IncreaseDamageSO", menuName = "Components/IncreaseDamageSO")]
public class IncreaseDamageSO : ComponentSO
{
    [Header("Damage Params")]
    public int DamageIncreaseAmount = 5; 
    public override void Execute(GameObject target)
    {
        if (target.TryGetComponent<Bullet>(out var bullet))
        {
            bullet.Damage += DamageIncreaseAmount;
        }
    }

}
