using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestPlayElevator
{
    [UnityTest]
    public IEnumerator ElevatorStateGoingDown_ElevatorAtTop_ElevatorStopAtBottom()
    {
        GameObject elevatorGO = _GetElevator();
        ElevatorController elevatorScript = elevatorGO.GetComponent<ElevatorController>();
        yield return new WaitForSeconds(.1f);

        elevatorScript.CurrentPos = ElevatorController.ElevatorIdlePos.Top;

        elevatorScript.CurrentState = ElevatorController.ElevatorState.GoingDown;
        yield return new WaitForSeconds(.1f);
        Assert.IsTrue(elevatorScript.IsReachedBottom);
    }

    [UnityTest]
    public IEnumerator ElevatorStateGoingUp_ElevatorAtBottom_ElevatorStopAtTop()
    {
        GameObject elevatorGO = _GetElevator();
        ElevatorController elevatorScript = elevatorGO.GetComponent<ElevatorController>();
        yield return new WaitForSeconds(.1f);

        elevatorScript.CurrentPos = ElevatorController.ElevatorIdlePos.Bottom;

        elevatorScript.CurrentState = ElevatorController.ElevatorState.GoingUp;
        yield return new WaitForSeconds(.1f);
        Assert.IsTrue(elevatorScript.IsReachedTop);

    }

    [UnityTest]
    public IEnumerator ElevatorStateGoingUp_ElevatorAtTop_ElevatorStaysAtTop()
    {
        GameObject elevatorGO = _GetElevator();
        ElevatorController elevatorScript = elevatorGO.GetComponent<ElevatorController>();
        yield return new WaitForSeconds(.1f);

        elevatorScript.CurrentPos = ElevatorController.ElevatorIdlePos.Top;

        elevatorScript.CurrentState = ElevatorController.ElevatorState.GoingUp;
        yield return new WaitForSeconds(1f);
        Assert.AreEqual(ElevatorController.ElevatorState.Ready, elevatorScript.CurrentState);
        Assert.AreEqual(ElevatorController.ElevatorIdlePos.Top, elevatorScript.CurrentPos);
    }

    [UnityTest]
    public IEnumerator ElevatorStateGoingDown_ElevatorAtBottom_ElevatorStaysAtBottom()
    {
        GameObject elevatorGO = _GetElevator();
        ElevatorController elevatorScript = elevatorGO.GetComponent<ElevatorController>();
        yield return new WaitForSeconds(.1f);

        elevatorScript.CurrentPos = ElevatorController.ElevatorIdlePos.Bottom;

        elevatorScript.CurrentState = ElevatorController.ElevatorState.GoingDown;
        yield return new WaitForSeconds(.1f);
        Assert.IsTrue(elevatorScript.IsReachedBottom);
    }

    [UnityTest]
    public IEnumerator HandleInteract_SwitchTopPress_ElevatorGoingUp ()
    {
        GameObject elevator = _GetElevator(isFast: false);
        ElevatorController elevatorScript = elevator.GetComponent<ElevatorController>();
        
        GameObject btn = _GetButton(
                role: ElevatorButtonController.ButtonRole.Top,
                elevator: elevator.GetComponent<ElevatorController>()
            );
        ElevatorButtonController btnScript = btn.GetComponent<ElevatorButtonController>();
        
        yield return new WaitForSeconds(.1f);

        elevatorScript.CurrentPos = ElevatorController.ElevatorIdlePos.Bottom;
        
        Assert.AreEqual(ElevatorController.ElevatorIdlePos.Bottom, elevatorScript.CurrentPos);
        Assert.AreEqual(ElevatorController.ElevatorState.Ready, elevatorScript.CurrentState);
        
        btnScript.HandleInteract();

        yield return new WaitForSeconds(.1f);

        Assert.AreEqual(ElevatorController.ElevatorState.GoingUp, elevatorScript.CurrentState);

        yield return null;
    }

    [UnityTest]
    public IEnumerator HandleInteract_SwitchBottomPress_ElevatorGoingDown ()
    {
        GameObject elevator = _GetElevator(isFast: false);
        ElevatorController elevatorScript = elevator.GetComponent<ElevatorController>();
        
        GameObject btn = _GetButton(
                           role: ElevatorButtonController.ButtonRole.Bottom,
                           elevator: elevator.GetComponent<ElevatorController>()
                         );
        ElevatorButtonController btnScript = btn.GetComponent<ElevatorButtonController>();
        
        elevatorScript.CurrentPos = ElevatorController.ElevatorIdlePos.Top;

        yield return new WaitForSeconds(.1f);
        
        Assert.AreEqual(ElevatorController.ElevatorIdlePos.Top, elevatorScript.CurrentPos);
        Assert.AreEqual(ElevatorController.ElevatorState.Ready, elevatorScript.CurrentState);
        
        btnScript.HandleInteract();

        yield return new WaitForSeconds(.1f);

        Assert.AreEqual(ElevatorController.ElevatorState.GoingDown, elevatorScript.CurrentState);

        yield return null;
    }

    [UnityTest]
    public IEnumerator HandleInteract_SwitchInsidePress_ElevatorGoingOppose ()
    {
        GameObject elevator = _GetElevator(isFast: false);
        ElevatorController elevatorScript = elevator.GetComponent<ElevatorController>();
        
        GameObject btn = _GetButton(
                            role: ElevatorButtonController.ButtonRole.Inside,
                            elevator: elevator.GetComponent<ElevatorController>()
                         );
        ElevatorButtonController btnScript = btn.GetComponent<ElevatorButtonController>();
        
        elevatorScript.CurrentPos = ElevatorController.ElevatorIdlePos.Top;

        yield return new WaitForSeconds(.1f);

        Assert.AreEqual(ElevatorController.ElevatorIdlePos.Top, elevatorScript.CurrentPos);
        Assert.AreEqual(ElevatorController.ElevatorState.Ready, elevatorScript.CurrentState);
        
        btnScript.HandleInteract();

        yield return new WaitForSeconds(.1f);

        Assert.AreEqual(ElevatorController.ElevatorState.GoingDown, elevatorScript.CurrentState);

        yield return null;
    }

    GameObject _GetElevator(bool isFast = true)
    {
        GameObject elevator = new GameObject();
        elevator.AddComponent<ElevatorController>();
        ElevatorController elevatorScript = elevator.GetComponent<ElevatorController>();

        GameObject lowerPos = new GameObject();
        GameObject upperPos = new GameObject();

        lowerPos.transform.position += Vector3.zero;
        upperPos.transform.position += Vector3.up;

        elevatorScript.lowerPos = lowerPos.transform;
        elevatorScript.upperPos = upperPos.transform;
        elevatorScript.moveSpeed = isFast ? 100f : 0.1f ;

        return elevator;
    }

    GameObject _GetButton(ElevatorButtonController.ButtonRole role, ElevatorController elevator) 
    {
        GameObject btn = new GameObject();
        btn.AddComponent<BoxCollider2D>();
        btn.GetComponent<BoxCollider2D>().isTrigger = true;
        btn.AddComponent<ElevatorButtonController>();
        ElevatorButtonController btnScript = btn.GetComponent<ElevatorButtonController>();
        btnScript.controlledElevator = elevator;
        btnScript.buttonRole = role;

        return btn;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        foreach (GameObject obj in Object.FindObjectsOfType<GameObject>())
        {
            Object.Destroy(obj);
        }
        yield return null;
    }

}
