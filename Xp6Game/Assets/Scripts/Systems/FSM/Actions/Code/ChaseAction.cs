using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "New Chase Action", menuName = "FSM/Action/ChaseAction")]
public class ChaseAction : Action
{
    public float randomThresholdOffset = 0.5f;
    private Vector3 offset;
    private NavMeshAgent _agent;

    public override void Setup(StateMachine stateMachine)
    {
        _agent = stateMachine.GetNavMeshAgent();
        if (_agent.isStopped)
        {
            _agent.isStopped = false;
        }
        offset = new Vector3(Random.Range(-randomThresholdOffset, randomThresholdOffset), 0, Random.Range(-randomThresholdOffset, randomThresholdOffset));
    }
    public override void Act(StateMachine stateMachine)
    {
        if (stateMachine.GetTarget() == null) return;

        Vector3 targetPosition = stateMachine.GetTarget().transform.position + offset;
        stateMachine.GetNavMeshAgent().SetDestination(targetPosition);

    }

    public override void Exit(StateMachine stateMachine)
    {

    }
}
