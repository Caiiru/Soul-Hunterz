using UnityEngine;

public class SimpleBullet : Bullet
{
    [Header("VFX")]
    public GameObject hitVFX;
    protected override void Start()
    {
        Direction = transform.forward;
        base.Start();

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Enemy>(out var enemy))
        {
            enemy.SendMessage("TakeDamage", Damage);

        }
        if (hitVFX)
        {
            Instantiate(hitVFX, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }
}
