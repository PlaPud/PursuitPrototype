using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtmElevatorTrigger : MonoBehaviour
{

    public bool IsObstacles;
    private ElevatorController _elvt;

    private void Awake()
    {
        _elvt = GetComponentInParent<ElevatorController>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_elvt.CurrentState != ElevatorController.ElevatorState.GoingDown) return;

        if (!collision.CompareTag("Moveable") && !collision.CompareTag("PlayerCat") && !collision.CompareTag("PlayerCompBot")) return;

        _elvt.currentPos = ElevatorController.ElevatorIdlePos.Bottom;
        _elvt.CurrentState = ElevatorController.ElevatorState.GoingUp;
    }
}
