using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DummyEnemy : Enemy
{
    [Header("Debug")]
    public bool m_AlwaysShooting = true;

    private Animator _animator;

    public float attackCooldown = 5;
    private float _attackTimer;
    private bool _canAttack = false;

    [SerializeField] private Transform _firePoint;
    [Space]
    [Header("Bullet & VFX")]

    public GameObject bulletPrefab;
    public GameObject fireVFXPrefab;
    protected override void OnEnable()
    {
        base.OnEnable();
        VFXDebugManager.OnInputPressed += OnInputPressed;
        _animator = GetComponentInChildren<Animator>();
        _attackTimer = 0;

        if (m_AlwaysShooting)
            StartShooting();
    }
    async void Update()
    {
        await HandleTimer();
    }

    private async UniTask HandleTimer()
    {
        if (!_canAttack) return;
        _attackTimer += Time.deltaTime;
        if (_attackTimer >= attackCooldown)
        {
            // _attackTimer = 0;
            await Aim();
        }
    }


    private void OnInputPressed(int key)
    {
        if (key == 0)
            StartShooting();
        if (key == 1)
            StopShooting();
    }

    async void StartShooting()
    {
        _canAttack = true;
        await Aim();
    }


    private void StopShooting()
    {
        _canAttack = false;
    }


    async UniTask Aim()
    {
        await UniTask.Delay(1000);
        _animator.SetTrigger("aim");

        await UniTask.CompletedTask;
        // Attack();
    }
    public override void Attack()
    {
        base.Attack();
        _attackTimer = 0;

        GameObject bullet = Instantiate(bulletPrefab, _firePoint.transform.position, Quaternion.identity);
        // bullet.transform.position = _firePoint.transform.position;
        if (!fireVFXPrefab) return;
        GameObject fireVFX = Instantiate(fireVFXPrefab);
        fireVFX.transform.position = _firePoint.transform.position;
        Destroy(fireVFX, 2f);
    }

}
