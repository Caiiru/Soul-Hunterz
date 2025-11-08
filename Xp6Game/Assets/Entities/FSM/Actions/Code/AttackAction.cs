using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "FSM/Action/AttackAction")]
public class AttackAction : Action
{
    public override void Act(StateMachine stateMachine)
    {

        stateMachine.Attack();
    }

    public override void Exit(StateMachine stateMachine)
    {

    }

    public override void Setup(StateMachine stateMachine)
    {
        stateMachine.SetMoving(false);

    }

}
