using UnityEngine;

[CreateAssetMenu(fileName = "New Multiple Bullet", menuName = "Components/MultipleBullet")]
public class MultipleBulletComponentSO : ComponentSO
{
    [SerializeField] SpawnBulletBehaviour behaviour;
    [SerializeField] int bulletCount = 2;
    [SerializeField] float distanceOffset = 1f;

    public override void ComponentUpdate(Bullet bullet)
    {

    }

    public override BulletPayload Execute(BulletPayload payload, Transform firePoint, int slotIndex)
    {
        // Este componente agora apenas informa o payload sobre o comportamento de múltiplos projéteis.
        base.Execute(payload, firePoint, slotIndex);
        
        payload.BulletCount += bulletCount;
        payload.SpreadDistance = distanceOffset;
        return payload;
    }
}


enum SpawnBulletBehaviour
{
    Parallel,
    WithAngle,

}
