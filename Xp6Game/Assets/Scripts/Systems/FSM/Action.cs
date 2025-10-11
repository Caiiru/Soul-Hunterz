using UnityEngine;

// [CreateAssetMenu(fileName = "Action", menuName = "FSM/Action/NewAction")]
public abstract class Action : ScriptableObject
{
    public abstract void Act(StateMachine stateMachine);
}
