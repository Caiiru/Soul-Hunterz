using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "New Chase Action", menuName = "FSM/Action/ChaseAction")]
public class ChaseAction : Action
{

    public override void Setup(StateMachine stateMachine)
    {
        stateMachine.SetMoving(true);

    }
    public override void Act(StateMachine stateMachine)
    { 

        float randomThresholdOffset = 1.5f;
        Vector3 offset = new Vector3(Random.Range(-randomThresholdOffset, randomThresholdOffset), 0, Random.Range(-randomThresholdOffset, randomThresholdOffset));

        if (stateMachine.GetTarget() == null) return;

        Vector3 targetPosition = stateMachine.GetTarget().transform.position + offset;
        stateMachine.SetDestination(targetPosition);

    }

    public override void Exit(StateMachine stateMachine)
    {

    }
}
