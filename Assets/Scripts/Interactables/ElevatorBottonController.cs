using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class ElevatorBottonController : MonoBehaviour
{
    enum ButtonRole { Top, Bottom, Inside }

    [SerializeField] ElevatorController controlledElevator;
    [SerializeField] ButtonRole buttonRole;

    private bool _isButtonPress;
    private bool _canPress;

    void Start()
    {

    }

    void Update()
    {
        OnPressButton();
        HandlePress();
    }

    private void OnPressButton() 
    {

        _isButtonPress = Input.GetKeyDown(KeyCode.E) && _canPress;
    }

    private void HandlePress() 
    {
        if (!_isButtonPress) return;

        switch (buttonRole) 
        {
            case ButtonRole.Top:
                controlledElevator.CurrentState = ElevatorController.ElevatorState.GoingUp;
                break;
            case ButtonRole.Bottom:
                controlledElevator.CurrentState = ElevatorController.ElevatorState.GoingDown;
                break;
            case ButtonRole.Inside:
                _SetStateInside();
                break;
        }
    }

    private void _SetStateInside()
    {
        switch (controlledElevator.currentPos) 
        {
            case ElevatorController.ElevatorIdlePos.Top:
                controlledElevator.CurrentState = ElevatorController.ElevatorState.GoingDown;
                break;
            case ElevatorController.ElevatorIdlePos.Bottom:
                controlledElevator.CurrentState = ElevatorController.ElevatorState.GoingUp;
                break;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("PlayerCat")) return;
        _canPress = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _canPress = false;
    }
}
