using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorBottonController : MonoBehaviour
{
    enum ButtonRole { Top, Bottom, Inside }

    [SerializeField] ElevatorController controlledElevator;
    [SerializeField] ButtonRole buttonRole;


    void Start()
    {

    }

    void Update()
    {
        
    }
}
