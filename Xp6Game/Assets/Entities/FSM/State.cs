using UnityEngine;

[CreateAssetMenu(fileName = "New State", menuName = "FSM/NewState")]
public class State : ScriptableObject
{
    public Action[] actions;
    public Transition[] transitions;

    public void BeginState(StateMachine stateMachine)
    {
        SetupActions(stateMachine);
    }
    public void UpdateState(StateMachine stateMachine)
    {
        DoActions(stateMachine);
        CheckTransitions(stateMachine);
    }

    public void ExitState(StateMachine stateMachine)
    {
        ExitActions(stateMachine);
    }
    private void SetupActions(StateMachine stateMachine)
    {
        foreach (Action action in actions)
        {
            action.Setup(stateMachine);
        }
    }
    private void DoActions(StateMachine stateMachine)
    {
        foreach (Action action in actions)
        {
            action.Act(stateMachine);
        }
    }
    private void ExitActions(StateMachine stateMachine)
    {
        foreach (Action action in actions)
        {
            action.Exit(stateMachine);
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
