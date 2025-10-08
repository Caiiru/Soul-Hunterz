using UnityEngine;
using UnityEngine.VFX;

public class Idle_DummyEnemy : Enemy
{

    [Header("VFX")]
    public GameObject takeDamageVFXPrefab;
    protected override void Start()
    {

        base.Start();
        canBeDamaged = false;
    }

    protected override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        if (!takeDamageVFXPrefab) return;

        GameObject takeDamageVFX = Instantiate(takeDamageVFXPrefab);
        takeDamageVFX.transform.position = transform.position;
        // takeDamageVFX.GetComponentInChildren<VisualEffect>().
        Destroy(takeDamageVFX, 7f);

    }

}
