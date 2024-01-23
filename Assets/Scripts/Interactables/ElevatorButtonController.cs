using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;

[assembly: InternalsVisibleTo("PlayMode")]
[assembly: InternalsVisibleTo("EditMode")]

public class ElevatorButtonController : Interactable
{
    public enum ButtonRole { Top, Bottom, Inside }

    [SerializeField] internal ElevatorController controlledElevator;
    [SerializeField] internal ButtonRole buttonRole;

    private Collider2D _buttonCD;

    private void Awake()
    {
        _buttonCD = GetComponent<Collider2D>(); 
    }

    void Start()
    {

    }

    protected override void Update()
    {
        if (controlledElevator.CurrentState == ElevatorController.ElevatorState.Ready && !_buttonCD.enabled) 
        {
            _buttonCD.enabled = true;
        }

        base.Update();
    }

    public override void HandleInteract() 
    {
        switch (buttonRole) 
        {
            case ButtonRole.Top:
                if (controlledElevator.CurrentPos == ElevatorController.ElevatorIdlePos.Top) return;
                controlledElevator.CurrentState = ElevatorController.ElevatorState.GoingUp;
                _buttonCD.enabled = false;
                _isInteract = false;
                break;
            case ButtonRole.Bottom:
                if (controlledElevator.CurrentPos == ElevatorController.ElevatorIdlePos.Bottom) return;
                controlledElevator.CurrentState = ElevatorController.ElevatorState.GoingDown;
                _buttonCD.enabled = false;
                _isInteract = false;
                break;
            case ButtonRole.Inside:
                _SetStateInside();
                break;
        }

        AudioManager.Instance?.PlayOneShot(FMODEvents.Instance.SwitchPressed, transform.position);
    }

    private void _SetStateInside()
    {
        switch (controlledElevator.CurrentPos) 
        {
            case ElevatorController.ElevatorIdlePos.Top:
                controlledElevator.CurrentState = ElevatorController.ElevatorState.GoingDown;
                _buttonCD.enabled = false;
                _isInteract = false;
                break;
            case ElevatorController.ElevatorIdlePos.Bottom:
                controlledElevator.CurrentState = ElevatorController.ElevatorState.GoingUp;
                _buttonCD.enabled = false;
                _isInteract = false;
                break;
        }
    }
}
