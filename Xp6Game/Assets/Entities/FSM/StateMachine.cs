using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Enemy))]
public class StateMachine : MonoBehaviour
{
    [Header("Data")]
    private EnemySO enemyData;
    private Enemy _enemyReference;
    private NavMeshAgent _navMeshAgent;
    [Header("State Machine States")]
    private State initialState;
    public State currentState;
    public State remainState;

    

    private bool isActive = false; 
 
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

    public GameObject GetTarget()
    {
        return _enemyReference.GetTarget() ? _enemyReference.GetTarget().gameObject : null;
    }
    public NavMeshAgent GetNavMeshAgent()
    {
        return _navMeshAgent;
    }

    public void SetTarget(GameObject gameObject)
    { 
        _enemyReference.SetTarget(gameObject.transform);
    }

    public EnemySO GetEnemyData()
    {
        return enemyData;
    }

    internal void SetMoving(bool newState)
    {
        _navMeshAgent.isStopped = !newState;
    }

    internal void SetDestination(Vector3 targetPosition)
    {
        _navMeshAgent.SetDestination(targetPosition);
    }

    internal Enemy GetEnemy()
    {
        return _enemyReference;
    }
}
