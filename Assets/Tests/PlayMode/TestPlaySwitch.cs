using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestPlaySwitch
{
    [UnityTest]
    [TestCase(1, ExpectedResult = null)]
    [TestCase(3, ExpectedResult = null)]
    [TestCase(10, ExpectedResult = null)]
    public IEnumerator SwitchPressed_DoorsOpenSame_DoorsClosedSame(int doorsAmount)
    {
        GameObject testSwitch = _GetSwitch();
        List<GameObject> testDoors = _GetDoors(amount: doorsAmount);

        SwitchController switchScript = testSwitch.GetComponent<SwitchController>();
        List<DoorController> doorScripts = testDoors.ToList().ConvertAll(door => door.GetComponent<DoorController>());

        doorScripts.ForEach(door => door.IsOpen = true);

        switchScript.toggleDoorsTarget = doorScripts;

        yield return new WaitForSeconds(.25f);

        switchScript.HandleInteract();

        Assert.IsTrue(doorScripts.All(door => !door.IsOpen));
    }

    [UnityTest]
    [TestCase(1, ExpectedResult = null)]
    [TestCase(3, ExpectedResult = null)]
    [TestCase(10, ExpectedResult = null)]
    public IEnumerator SwitchPress_DoorsOpenDifferently_DoorsOpenCloseOppose(int doorsAmount)
    {
        List<bool> doorsStatus = new List<bool>();

        GameObject testSwitch = _GetSwitch();
        List<GameObject> testDoors = _GetDoors(amount: doorsAmount);

        SwitchController switchScript = testSwitch.GetComponent<SwitchController>();
        List<DoorController> doorScripts = testDoors.ToList().ConvertAll(door => door.GetComponent<DoorController>());

        for (int i = 0; i < doorsAmount; i++)
        {
            bool isOpen = Random.Range(0, 2) == 1;
            doorsStatus.Add(isOpen);
        }

        for (int i = 0; i < doorsAmount; i++)
        {
            doorScripts[i].IsOpen = doorsStatus[i];
        }

        switchScript.toggleDoorsTarget = doorScripts;

        yield return new WaitForSeconds(.25f);

        switchScript.HandleInteract();

        for (int i = 0; i < doorsAmount; i++)
        {
            Assert.IsTrue(doorScripts[i].IsOpen == !doorsStatus[i]);
        }

        Object.Destroy(testSwitch);
        testDoors.ForEach(door => Object.Destroy(door));
    }

    private GameObject _GetSwitch() 
    {
        GameObject testSwitch = new GameObject();

        testSwitch.AddComponent<BoxCollider2D>();
        testSwitch.GetComponent<BoxCollider2D>().isTrigger = true;
        testSwitch.AddComponent<SwitchController>();

        return testSwitch;
    }

    private List<GameObject> _GetDoors(int amount)
    {
        List<GameObject> testDoors = new List<GameObject>();
        for (int i = 0; i < amount; i++)
        {
            GameObject testDoor = _SetUpDoor();
            testDoors.Add(testDoor);
        }
        return testDoors;
    }

    private static GameObject _SetUpDoor()
    {
        GameObject testDoorObject = new GameObject();
        GameObject doorSprite = new GameObject();
        GameObject doorCollider = new GameObject();

        DoorController doorScript = testDoorObject.AddComponent<DoorController>();
        doorScript.DoorBody = doorSprite.transform;
        doorCollider.AddComponent<BoxCollider2D>();

        doorSprite.transform.parent = testDoorObject.transform;
        doorCollider.transform.parent = testDoorObject.transform;

        return testDoorObject;
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
