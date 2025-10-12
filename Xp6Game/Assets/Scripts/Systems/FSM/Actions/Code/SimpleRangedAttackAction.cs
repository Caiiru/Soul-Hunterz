using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "FSM/Action/SimpleAttack")]
public class SimpleRangedAttackAction : Action
{
    
    private RangedEnemySO _enemyData;
    private GameObject _bulletPrefab;

    private float _timer;

    public override void Setup(StateMachine stateMachine)
    {
        _enemyData = stateMachine.GetEnemyData() as RangedEnemySO;
        _bulletPrefab = _enemyData.bulletPrefab;

    }
    public override void Act(StateMachine stateMachine)
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            Shoot(stateMachine);
        }
    }

    private void Shoot(StateMachine stateMachine)
    {
        GameObject _bulletGO = Instantiate(_bulletPrefab, stateMachine.transform.position, Quaternion.identity);
        var bullet = _bulletGO.GetComponent<Bullet>();
        bullet.Direction = stateMachine.transform.forward;
        // bullet.Initialize();
        Destroy(bullet, _enemyData.projectileLifeTime);
        _timer = _enemyData.timeBetweenShots;

    }

    public override void Exit(StateMachine stateMachine)
    {
        throw new System.NotImplementedException();
    }
 


}
