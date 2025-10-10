using UnityEngine;

public class DummyBullet : Bullet
{

    [Header("VFX")]
    public GameObject hitVFX;
    void Start()
    {
        Direction = transform.forward;
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnCollisionEnter(Collision collision)
    {
        if (!wasInstancied) return;
        Debug.Log($"Bullet colission with {collision.transform.name}");
        if (collision.gameObject.TryGetComponent<Enemy>(out var enemy))
        {
            enemy.SendMessage("TakeDamage", Damage);

        }
        if (hitVFX)
        {
            GameObject vfx = Instantiate(hitVFX, collision.GetContact(0).point, transform.rotation);
            Destroy(vfx, 5f);
        }

        Destroy(gameObject);
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Direction);
    }
}
