using UnityEngine;

public class SimpleBullet : Bullet
{
    [Header("VFX")]
    public GameObject hitVFX;

    
    void Start()
    {

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
            Instantiate(hitVFX, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position+ Direction);
    }
}
