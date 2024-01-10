using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleElevatorTrigger : MonoBehaviour
{
    public enum TriggerPos { Top, Bottom }

    [field: SerializeField] public TriggerPos TriggerAt { get; private set; } = TriggerPos.Bottom;

    public bool IsObstacles;
    private ElevatorController _elvt;

    private void Awake()
    {
        _elvt = GetComponentInParent<ElevatorController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (TriggerAt) 
        {
            case TriggerPos.Bottom:
                if (_elvt.CurrentState != ElevatorController.ElevatorState.GoingDown) return;
                break;
            case TriggerPos.Top:
                if (_elvt.CurrentState != ElevatorController.ElevatorState.GoingUp) return;
                break;
        }

        if (!collision.CompareTag("Moveable") && !collision.CompareTag("PlayerCat") && !collision.CompareTag("PlayerCompBot")) return;

        switch (TriggerAt) 
        {
            case TriggerPos.Bottom:
                _elvt.CurrentPos = ElevatorController.ElevatorIdlePos.Bottom;
                _elvt.CurrentState = ElevatorController.ElevatorState.GoingUp;
                break;
            case TriggerPos.Top:
                _elvt.CurrentPos = ElevatorController.ElevatorIdlePos.Top;
                _elvt.CurrentState = ElevatorController.ElevatorState.GoingDown;
                break;
        }
    }
}
