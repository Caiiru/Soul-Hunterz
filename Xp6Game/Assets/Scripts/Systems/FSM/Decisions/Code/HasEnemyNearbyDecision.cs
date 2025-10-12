
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "HasEnemyNearbyDecision", menuName = "FSM/Decision/HasEnemyNearbyDecision")]
public class HasEnemyNearbyDecision : Decision
{
    public float detectionRadius = 5f;
    public float timeBetweenChecks = 2.5f;
    [SerializeField] private float _timer; 

    //Debug
 

    public override bool Decide(StateMachine stateMachine)
    { 
        if (canCheck())
        {
            return isEnemyNearby(stateMachine);
        }
        return false;

        
    }

    private bool canCheck()
    {
        _timer = _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            _timer = timeBetweenChecks;
            return true;
        }
        return false;
    }

    private bool isEnemyNearby(StateMachine stateMachine)
    {
        // Debug.Log("Checking for Player Nearby");
        // CreateDebugSphere(stateMachine);
        var numColliders = Physics.OverlapSphere(stateMachine.transform.position, detectionRadius, stateMachine.enemyData.playerMask);
        if (numColliders.Length > 0)
        {
            stateMachine.SetTarget(numColliders[0].gameObject);
            return true;
        } 
        
        return false;
    }

    private void CreateDebugSphere(StateMachine stateMachine)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = stateMachine.transform.position;
        sphere.transform.localScale = new Vector3(detectionRadius * 2, detectionRadius * 2, detectionRadius * 2);
        sphere.GetComponent<Collider>().enabled = false;
        sphere.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.1f);
        Destroy(sphere, 2f);
    }

}
