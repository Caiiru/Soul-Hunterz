using UnityEngine;

// [CreateAssetMenu(fileName = "Decision", menuName = "FSM/Decision/NewDecision")]
public abstract class Decision : ScriptableObject
{
    public abstract bool Decide(StateMachine stateMachine);   
}
