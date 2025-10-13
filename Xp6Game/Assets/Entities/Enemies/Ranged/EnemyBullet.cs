using UnityEngine;

public class EnemyBullet : Bullet
{
    void Start()
    {
        Direction = transform.forward;
        Initialize();
    }
    public override void Initialize()
    {
        base.Initialize();
    }
    void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision.gameObject);       
    }
    public override void HandleCollision(GameObject target)
    {
        if (target.TryGetComponent<PlayerEntity>(out var entity))
        {

            entity.SendMessage("TakeDamage", GetBulletDamage());

        }
        Destroy(gameObject);

        if (bulletData.hitVFX)
        {
            GameObject vfx = Instantiate(bulletData.hitVFX, transform.position, transform.rotation);
            Destroy(vfx, 5f);
        }
    }

}
