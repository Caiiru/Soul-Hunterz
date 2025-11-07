
using UnityEngine;

[CreateAssetMenu(fileName = "Decision", menuName = "FSM/Decision/OnAttackRangeDecision")]
public class OnAttackRangeDecision : Decision
{
    public override bool Decide(StateMachine stateMachine)
    { 
        Transform targetTranform = stateMachine.GetTarget().transform;

        if (!targetTranform) return false;
        float distance = Vector3.Distance(stateMachine.transform.position, targetTranform.position);
        return distance <= stateMachine.data.attackRange;
    }

}
