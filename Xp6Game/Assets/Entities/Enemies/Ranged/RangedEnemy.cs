using UnityEngine;

public class RangedEnemy : Enemy
{
    [HideInInspector] public RangedEnemySO rangedEnemyData;
    [SerializeField] private Transform _firePoint;


    private float _shotCooldown;
    private float _timer;
    protected override void OnEnable()
    {
        // base.OnEnable();
    }
    public override void SetData(EnemySO newData)
    {
        rangedEnemyData = newData as RangedEnemySO; 
        base.SetData(rangedEnemyData);

        // Initialize();
    }

    public override void Initialize()
    {
        base.Initialize();

        _shotCooldown = rangedEnemyData.timeBetweenShots;
        _timer = _shotCooldown; // So it can shoot immediately on spawn 
        _firePoint = _visualTransform.Find("FirePoint");
    }
    public override bool CanAttack()
    {
        if (_timer < _shotCooldown)
        {
            _timer += Time.deltaTime;
            return false;
        }
        _timer = 0;

        return base.CanAttack();
    }

    public override void Attack()
    { 
        if (rangedEnemyData.bulletPrefab != null && _firePoint != null)
        {
            Instantiate(rangedEnemyData.bulletPrefab, _firePoint.position, _firePoint.rotation);

        }
    }

}
