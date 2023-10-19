using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    public enum ElevatorState { Ready, GoingUp, GoingDown }
    
    public ElevatorState CurrentState = ElevatorState.Ready;

    private string currentAnimationState = ELEVATOR_IDLE;

    const string ELEVATOR_IDLE = "ElevatorIdle";
    const string ELEVATOR_UP = "ElevatorUp";
    const string ELEVATOR_DOWN = "ElevatorDown";

    void Start()
    {
        
    }

    void Update()
    {
        switch (CurrentState) 
        {
            case ElevatorState.Ready:
                HandleReady();
                break;
            case ElevatorState.GoingUp:
                HandleGoingDown();
                break;
            case ElevatorState.GoingDown:
                HandleGoingDown();
                break;
        }
    }

    private void HandleReady() 
    {
        
    }

    private void HandleGoingDown() 
    {
    
    }

    private void HandleGoingUp() 
    {
    
    }

    private void ChangeAnimationState(string newAnimState) 
    {
        
    }

}
