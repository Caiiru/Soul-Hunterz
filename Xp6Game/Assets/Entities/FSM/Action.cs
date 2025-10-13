using System;
using UnityEngine;

// [CreateAssetMenu(fileName = "Action", menuName = "FSM/Action/NewAction")]
public abstract class Action : ScriptableObject
{
    public abstract void Setup(StateMachine stateMachine);
    public abstract void Act(StateMachine stateMachine);

    public abstract void Exit(StateMachine stateMachine);
}
