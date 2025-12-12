using UnityEngine;
using UnityEngine.VFX;

public class Idle_DummyEnemy : Enemy<EnemySO>
{
    protected override void OnEnable()
    {

        base.OnEnable();
        canBeDamaged = false;
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

    }

}
