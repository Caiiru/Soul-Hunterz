using UnityEngine;

[CreateAssetMenu(fileName = "State", menuName = "FSM/State/NewState")]
public class State : ScriptableObject
{
    public Action[] actions;
    public Transition[] transitions; 
    public void UpdateState(StateMachine stateMachine)
    {
        DoActions(stateMachine);
        CheckTransitions(stateMachine);
    }
    private void DoActions(StateMachine stateMachine)
    {
        foreach (Action action in actions)
        {
            action.Act(stateMachine);
        }
    }
    private void CheckTransitions(StateMachine stateMachine)
    {
        foreach (Transition transition in transitions)
        {
            bool decisionSucceeded = transition.decision.Decide(stateMachine);
            if (decisionSucceeded)
            {
                stateMachine.TransitionToState(transition.trueState);
            }
            else
            {
                stateMachine.TransitionToState(transition.falseState);
            }
        }
    } 
}
