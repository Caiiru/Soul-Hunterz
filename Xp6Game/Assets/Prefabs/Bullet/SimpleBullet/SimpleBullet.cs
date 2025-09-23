using UnityEngine;

public class SimpleBullet : Bullet
{
    
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
            
            Destroy(gameObject);
        }
    }
}
