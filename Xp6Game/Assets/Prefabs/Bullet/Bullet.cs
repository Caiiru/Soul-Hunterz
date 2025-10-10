using System;
using PlasticGui.Configuration.CloudEdition.Welcome;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Bullet : MonoBehaviour
{
    Rigidbody _rigidbody;
    Collider _collider;
    [Header("Data")]
    public BulletSO bulletData;
    public Vector3 Direction;
    public float Speed = 10f;
    public float LifeTime = 2f;

    public int Damage = 10;

    protected bool wasInstancied = false;
    protected virtual void OnEnable()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        if (!_rigidbody)
        {
            Debug.LogError("No rigidbody attached to the bullet");
        }
    }
 
    public virtual void FixedUpdate()
    {
        if(!wasInstancied) return;
        transform.position+= Direction.normalized * Speed * Time.fixedDeltaTime;
    }

    public virtual void Initialize()
    {
        wasInstancied = true;
        // _rigidbody.linearVelocity = Direction.normalized * Speed;
        LoadData();

        Destroy(gameObject, LifeTime);
    }
    void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision.gameObject);
    }
    public virtual void HandleCollision(GameObject target)
    {
        Debug.Log($"Bullet colission with {target.name}");
        if (target.TryGetComponent<Enemy>(out var enemy))
        {
            enemy.SendMessage("TakeDamage", Damage);

        }
        Destroy(gameObject);

        if(bulletData.hitVFX)
        {
            GameObject vfx = Instantiate(bulletData.hitVFX, transform.position, transform.rotation);
            Destroy(vfx, 5f);
        }
    }
    
    private void LoadData()
    {
        if (bulletData)
        {
            Speed = bulletData.Speed;
            LifeTime = bulletData.LifeTime;
            Damage = bulletData.BaseDamage;
        }
    }
    public BulletSO GetBulletData()
    {
        return bulletData;
    }

    internal void SetBullet(BulletSO bullet)
    {
        bulletData = bullet;
        LoadData();
    }
}
