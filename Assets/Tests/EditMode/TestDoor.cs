using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestDoor
{
    [Test]
    public void SetOpenDoor_IsOpen() 
    {
        GameObject testDoorObject = GetTestDoor();
        DoorController doorScript = testDoorObject.GetComponent<DoorController>();
        doorScript.Start();
        doorScript.SetOpenDoor();
        Assert.IsTrue(doorScript.IsOpen);
    }

    [Test]
    public void SetCloseDoor_IsClose()
    {
        GameObject testDoorObject = GetTestDoor();
        DoorController doorScript = testDoorObject.GetComponent<DoorController>();
        doorScript.Start();
        doorScript.SetCloseDoor();
        Assert.IsFalse(doorScript.IsOpen);
    }

    private static GameObject GetTestDoor() 
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
}
