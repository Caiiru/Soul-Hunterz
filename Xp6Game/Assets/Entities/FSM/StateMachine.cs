using System;
using Codice.Client.BaseCommands;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    [Header("Data")]
    public EnemySO enemyData;
    private Enemy _enemyReference;
    [Header("State Machine States")]
    private State initialState;
    public State currentState;
    public State remainState;

    private bool isActive = false;

    [Space]
    [Header("Target")]
    [SerializeField] private GameObject _target;

    void OnEnable()
    {
        _enemyReference = GetComponent<Enemy>();
        enemyData = _enemyReference.enemyData;

        enemyData.onDie += OnEnemyDie;


        if (enemyData == null) return;

        initialState = enemyData.initialState;
        currentState = initialState;

        isActive = true;

        remainState = Resources.Load<State>("FSM/RemainInState");

    }
    void Start()
    {

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
 
        currentState = trueState;
        ExitState();
    }
    private void ExitState()
    {

    }

    private void OnEnemyDie()
    {
        Debug.Log("From FSM: Enemy Died");
        isActive = false;

    }

    void OnDisable()
    {
        enemyData.onDie -= OnEnemyDie;
    }
    
    public bool HasTarget()
    {
        return _target != null;
    }
}
