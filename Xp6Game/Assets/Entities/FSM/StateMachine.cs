using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
public class StateMachine : MonoBehaviour
{
    [Header("Data")]
    public EnemySO data;
    private NavMeshAgent _navMeshAgent;
    [Header("State Machine States")]
    private State initialState;
    public State currentState;
    public State remainState;



    private bool isActive = false;


    //Events
    [Space]
    public UnityEvent m_OnAttack;
    public UnityEvent<int> m_OnTakeDamage;

    public UniTask InitializeStateMachine(EnemySO enemyData)
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();

        data = enemyData;

        initialState = data.m_InitialState;
        currentState = Instantiate(initialState);

        isActive = true;

        remainState = Resources.Load<State>("FSM/RemainInState");

        m_OnAttack = new UnityEvent();
        m_OnTakeDamage = new UnityEvent<int>();

        // Debug.Log("StateMachine Initialized");

        currentState.BeginState(this);



        return UniTask.CompletedTask;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;

        // Debug.Log("Update State Machine");
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
    void OnDisable()
    {

    }

#nullable enable
    public GameObject? GetTarget()
    {

        if (data.Target != null)
            return data.Target.gameObject;

        return null;
    }
    public NavMeshAgent GetNavMeshAgent()
    {
        return _navMeshAgent;
    }

    public void SetTarget(GameObject gameObject)
    {
        data.Target = gameObject.transform;
    }

    public EnemySO GetEnemyData()
    {
        return data;
    }

    internal void SetMoving(bool newState)
    {
        if (_navMeshAgent.isStopped != newState) return;
        _navMeshAgent.isStopped = !newState;
    }

    internal void SetDestination(Vector3 targetPosition)
    {
        _navMeshAgent.SetDestination(targetPosition);
    }

    internal void Attack()
    {
        m_OnAttack?.Invoke();
    }

    internal void TakeDamage(int v)
    {
        m_OnTakeDamage?.Invoke(v);
    }
    public void SetActive(bool isActive)
    {
        this.isActive = isActive;

    }
}
