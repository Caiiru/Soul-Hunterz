using System; 
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
        _rigidbody.useGravity = false;
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
            
            enemy.SendMessage("TakeDamage", GetBulletDamage());

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

    internal int GetBulletDamage()
    {
        if (bulletData == null)
        {
            Debug.LogError("No bullet data assigned to bullet");
            return Damage;
        }
        
        // damage threshold 

        Damage = bulletData.BaseDamage;

        System.Random rand = new System.Random();

        int roll = rand.Next(0, 100);
        Debug.Log($"Bullet roll: {roll} / CritChance: {bulletData.CritChance} / CritMultiplier: {bulletData.CritMultiplier}");
        if (roll < bulletData.CritChance)
        {
            Damage = Mathf.RoundToInt(Damage*bulletData.CritMultiplier);
            Debug.Log("CRIT!");
        }
        else
        {
            float damageReduction = UnityEngine.Random.Range(0.0f, 0.25f); 
            
            float threshold = (bulletData.BaseDamage * damageReduction);  
            Damage -= Mathf.RoundToInt(threshold);

            Debug.Log($"Normal hit! Threshold damage: {threshold}" );
            if (Damage < 1) Damage = 1;
            
        }
        
        return Damage;
    }
}
