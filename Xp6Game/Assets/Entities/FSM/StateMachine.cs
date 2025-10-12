using System;
using Codice.Client.BaseCommands;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Enemy))]
public class StateMachine : MonoBehaviour
{
    [Header("Data")]
    public EnemySO enemyData;
    private Enemy _enemyReference;
    private NavMeshAgent _navMeshAgent;
    [Header("State Machine States")]
    private State initialState;
    public State currentState;
    public State remainState;

    

    private bool isActive = false;

    [Space]
    [Header("Target")]
    [SerializeField] private GameObject _target;

    async void OnEnable()
    {
        await InitializeStateMachine();
    }
    public UniTask InitializeStateMachine()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _enemyReference = GetComponent<Enemy>();

        enemyData = _enemyReference.enemyData; 

        initialState = enemyData.initialState;
        currentState = Instantiate(initialState);

        isActive = true;

        remainState = Resources.Load<State>("FSM/RemainInState");

        return UniTask.CompletedTask;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;
        currentState.UpdateState(this);
    }
    public void TransitionToState(State trueState)
    {
        if (trueState == remainState)
            return;

        currentState.ExitState(this);
        Destroy(currentState);
        currentState = Instantiate(trueState); 
        currentState.BeginState(this);
    }

    private void OnEnemyDie()
    {
        Debug.Log("From FSM: Enemy Died");
        isActive = false;

    }

    void OnDisable()
    { 
    }

    public bool HasTarget()
    {
        return _target != null;
    }

    public GameObject GetTarget()
    {
        return _target;
    }
    public NavMeshAgent GetNavMeshAgent()
    {
        return _navMeshAgent;
    }

    public void SetTarget(GameObject gameObject)
    {
        _target = gameObject;
    }
}
