using UnityEngine;

public class DummyBullet : Bullet
{

    [Header("VFX")]
    public GameObject hitVFX;
    void Start()
    {
        Direction = transform.forward;
        this.Initialize(Direction, new BulletPayload());
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerEntity>(out var player))
        {
            Debug.Log("Casting take damage on player");
            player.TakeDamage(GetBulletDamage());
            SpawnVFX();
        }
        Destroy(this.gameObject);
        // HandleCollision(collision.gameObject);
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Direction);
    }
}
