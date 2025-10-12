using UnityEngine;

[CreateAssetMenu(fileName = "Decision", menuName = "FSM/Decision/HasTargetDecision")]
public class HasTargetDecision : Decision
{
    public override bool Decide(StateMachine stateMachine)
    {
        return stateMachine.HasTarget();
    }
    
}
