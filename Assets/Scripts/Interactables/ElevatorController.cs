using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    [Header("Elevator Control")]
    [SerializeField] private Transform lowerPos;
    [SerializeField] private Transform upperPos;
    [SerializeField] private float moveSpeed;

    public enum ElevatorState { Ready, GoingUp, GoingDown }
    public enum ElevatorIdlePos { Top, Bottom }

    public ElevatorState CurrentState = ElevatorState.Ready;
    public ElevatorIdlePos CurrentPos = ElevatorIdlePos.Bottom;

    public bool IsReachedTop => Vector2.Distance(transform.position, upperPos.position) < 0.05f;
    public bool IsReachedBottom => Vector2.Distance(transform.position, lowerPos.position) < 0.05f;


    private string currentAnimationState = ELEVATOR_IDLE;

    const string ELEVATOR_IDLE = "ElevatorIdle";
    const string ELEVATOR_UP = "ElevatorUp";
    const string ELEVATOR_DOWN = "ElevatorDown";

    private Animator _anim;

    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    void Update()
    {

        switch (CurrentState) 
        {
            case ElevatorState.Ready:
                HandleReady();
                break;
            case ElevatorState.GoingUp:
                HandleGoingUp();
                break;
            case ElevatorState.GoingDown:
                HandleGoingDown();
                break;
        }
    }

    private void HandleReady() 
    {
        _ChangeAnimationState(ELEVATOR_IDLE);
        _RepositionElevator();
    }

    private void HandleGoingDown() 
    {
        if (CurrentPos == ElevatorIdlePos.Bottom) return;

        _ChangeAnimationState(ELEVATOR_DOWN);
        _MoveElevator(to: lowerPos, isReach: IsReachedBottom, pos: ElevatorIdlePos.Bottom);
    }

    private void HandleGoingUp() 
    {
        if (CurrentPos == ElevatorIdlePos.Top) return;
        _ChangeAnimationState(ELEVATOR_UP);
        _MoveElevator(to: upperPos, isReach: IsReachedTop, pos: ElevatorIdlePos.Top);
    }

    private void _MoveElevator(Transform to, bool isReach, ElevatorIdlePos pos) 
    {
        transform.position = Vector2.MoveTowards(
                transform.position,
                to.position,
                moveSpeed * Time.deltaTime
            );

        if (!isReach) return;

        CurrentState = ElevatorState.Ready;
        CurrentPos = pos;
    }

    private void _RepositionElevator() 
    {
        if (CurrentState != ElevatorState.Ready) return;
        switch (CurrentPos) 
        {
            case ElevatorIdlePos.Top:
                transform.position = upperPos.position;
                break;
            case ElevatorIdlePos.Bottom:
                transform.position = lowerPos.position;
                break;
        }
    }

    private void _ChangeAnimationState(string newAnimState) 
    {
        if (newAnimState == currentAnimationState) return;
        currentAnimationState = newAnimState;
        _anim.Play(currentAnimationState);
    }

}
