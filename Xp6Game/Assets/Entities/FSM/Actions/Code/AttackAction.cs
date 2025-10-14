using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "FSM/Action/AttackAction")]
public class AttackAction : Action
{
    public override void Act(StateMachine stateMachine)
    {
        Enemy _enemy = stateMachine.GetEnemy();
        if (_enemy.CanAttack())
        {
            stateMachine.SetMoving(false);
            stateMachine.GetEnemy().Attack();
        }
    }

    public override void Exit(StateMachine stateMachine)
    {

    }

    public override void Setup(StateMachine stateMachine)
    {

    }

}
