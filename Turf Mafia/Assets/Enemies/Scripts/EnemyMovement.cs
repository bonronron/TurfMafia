using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [SerializeReference] EnemyBehaviour behaviour;
    NavMeshAgent agentNavigation;
    [SerializeField] private EnemyMovementState _state;
    public EnemyMovementState state
    {
        get => _state;
        set {_state = value; OnMovementStateChanged(value); }
    }
    [SerializeField] internal Transform targetDestination;

    private void OnEnable()
    {
        behaviour = GetComponent<EnemyBehaviour>();
        agentNavigation = GetComponent<NavMeshAgent>();
        var data = behaviour.enemyData;
        agentNavigation.speed = data.speed;
        state = EnemyMovementState.Idle;
    }

    private void FixedUpdate()
    {
        if (!agentNavigation.pathPending && state == EnemyMovementState.ComputingTarget) 
            state = EnemyMovementState.Moving;

        if (agentNavigation.remainingDistance <= agentNavigation.stoppingDistance && state == EnemyMovementState.Moving) 
            state = EnemyMovementState.Reached;
    }
    void OnMovementStateChanged(EnemyMovementState value)
    {
        switch (value)
        {
            case EnemyMovementState.Reached:
                agentNavigation.ResetPath();

                if (behaviour.behaviourState == EnemyState.MovingToHomeTurf) 
                    behaviour.behaviourState = EnemyState.ShootingHomeTurf; 

                if (behaviour.behaviourState == EnemyState.MovingToPlayer) 
                    behaviour.behaviourState = EnemyState.ShootingEnemySpawners;

                break;
            default:
                break;
        }
    }
    internal void WalkToLocation(Transform obj)
    {
        try
        {
            if (agentNavigation.isOnNavMesh)
            {
                
                var location = new Vector3(obj.position.x, transform.position.y, obj.position.z);
                agentNavigation.SetDestination(location);
                state = EnemyMovementState.ComputingTarget;
                targetDestination = obj;
            }
            else
            {
                Debug.LogWarning("Agent was not on Navmesh, destroyed");
                Destroy(gameObject);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Error while Navigating : " + e);
        }
    }   
}
public enum EnemyMovementState
{
    Idle,
    ComputingTarget,
    Moving,
    Reached
}
