using UnityEngine;
using UnityEngine.VFX;

public class Idle_DummyEnemy : Enemy
{
    protected override void OnEnable()
    {

        base.OnEnable();
        canBeDamaged = false;
    }

    protected override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

    }

}
