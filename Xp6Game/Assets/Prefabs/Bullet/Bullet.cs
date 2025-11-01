using System;
using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    [Header("Data")]
    public BulletSO bulletData;
    public Vector3 Direction;
    public float Speed = 10f;
    public float LifeTime = 2f;

    public int Damage = 1;
    public int BonusDamage = 0;

    [SerializeField] protected bool wasInstancied = false;
    protected virtual void OnEnable()
    {
    }

    public virtual void FixedUpdate()
    {
        if (!wasInstancied) return;
        transform.position += Direction.normalized * Speed * Time.fixedDeltaTime;
    }

    /// <summary>
    /// Inicializa a bala com base nos dados do payload.
    /// </summary>
    /// <param name="direction">A direção inicial da bala.</param>
    /// <param name="payload">O payload contendo modificadores.</param>
    public virtual void Initialize(Vector3 direction, BulletPayload payload)
    {
        wasInstancied = true;
        LoadData();

        // Aplica modificadores do payload
        this.Direction = direction;
        this.BonusDamage = (int)payload.BonusDamage;
        this.Speed *= payload.SpeedMultiplier;
        this.LifeTime *= payload.LifetimeMultiplier;

        Destroy(gameObject, LifeTime);
    }
    void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision.gameObject);
    }
    public virtual void HandleCollision(GameObject target)
    {
        if (target.TryGetComponent<Enemy>(out var enemy))
        {

            enemy.TakeDamage(GetBulletDamage());

        }
        SpawnVFX();
        
        Destroy(gameObject);




    }

    public virtual void SpawnVFX()
    {
        if (bulletData.hitVFX)
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
        if (roll < bulletData.CritChance)
        {
            Damage = Mathf.RoundToInt(Damage * bulletData.CritMultiplier);
        }
        else
        {
            float damageReduction = UnityEngine.Random.Range(0.0f, 0.25f);

            float threshold = (bulletData.BaseDamage * damageReduction);
            Damage -= Mathf.RoundToInt(threshold);

            if (Damage < 1) Damage = 1;

        }

        return Damage + BonusDamage;
    }
}
