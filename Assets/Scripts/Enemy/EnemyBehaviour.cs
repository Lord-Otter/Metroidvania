using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    #region Declarations
    [Header("References")]
    private TimeManager timeManager;
    
    private enum BehaviourState
    {
        Idle,
        Patrolling,
        Engaged
    }
    private BehaviourState currentState = BehaviourState.Idle;
    #endregion



    #region Unity Functions
    void Awake()
    {
        timeManager = GameObject.Find("Time_Manager").GetComponent<TimeManager>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
    #endregion



    #region State Machine
    private void ChangeState(BehaviourState newState)
    {
        //StopAllCoroutines();
        currentState = newState;
        switch (currentState)
        {
            case BehaviourState.Idle:
            // Code
                break;

            case BehaviourState.Patrolling:
            // Code
                break;

            case BehaviourState.Engaged:
            // Code
                break;
        }
    }
    #endregion
}
