using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestPlayPlate
{
    [OneTimeSetUp]
    public void Setup()
    {
        GameObject testCamera = new GameObject();
        testCamera.AddComponent<Camera>();
        testCamera.transform.position = new Vector3(0, 0, -10);
    }

    [UnityTest]
    public IEnumerator EnterTrigger_PlateEmpty_DoorsClosed()
    {
        GameObject testPlate = _SetUpPlate();
        GameObject testDoors = _SetUpDoor();

        PlateController plateScript = testPlate.GetComponent<PlateController>();
        DoorController doorScript = testDoors.GetComponent<DoorController>();

        plateScript.targetDoor = doorScript;

        yield return new WaitForSeconds(.1f);

        Assert.IsEmpty(plateScript.OnPlateObjs);
        Assert.IsFalse(doorScript.IsOpen);
    }

    [UnityTest]
    public IEnumerator EnterTrigger_ValidObjEntered_DoorOpen()
    {
        GameObject testPlate = _SetUpPlate();
        GameObject testDoors = _SetUpDoor();
        GameObject testValidObject = _SetUpPlayerObject();

        PlateController plateScript = testPlate.GetComponent<PlateController>();
        DoorController doorScript = testDoors.GetComponent<DoorController>();

        plateScript.targetDoor = doorScript;

        testValidObject.transform.position = testPlate.transform.position;

        yield return new WaitForSeconds(.1f);

        Assert.AreEqual(1, plateScript.OnPlateObjs.Count);
        Assert.IsTrue(doorScript.IsOpen);
    }

    [UnityTest]
    public IEnumerator EnterTrigger_InvalidObjEntered_DoorRemainClosed() 
    {
        GameObject testPlate = _SetUpPlate();
        GameObject testDoors = _SetUpDoor();
        GameObject testInvalidObject = new GameObject();

        PlateController plateScript = testPlate.GetComponent<PlateController>();
        DoorController doorScript = testDoors.GetComponent<DoorController>();

        plateScript.targetDoor = doorScript;
        
        testInvalidObject.transform.position = testPlate.transform.position;

        yield return new WaitForSeconds(.1f);

        Assert.IsEmpty(plateScript.OnPlateObjs);
        Assert.IsFalse(doorScript.IsOpen);
    }

    [UnityTest]
    public IEnumerator ExitTrigger_ObjExit_DoorClosed() 
    {
        GameObject testPlate = _SetUpPlate();
        GameObject testDoors = _SetUpDoor();
        GameObject testValidObject = _SetUpPlayerObject();

        PlateController plateScript = testPlate.GetComponent<PlateController>();
        DoorController doorScript = testDoors.GetComponent<DoorController>();

        plateScript.targetDoor = doorScript;
        testValidObject.transform.position = testPlate.transform.position;
        
        yield return new WaitForSeconds(.1f);

        Assert.IsNotEmpty(plateScript.OnPlateObjs);
        Assert.IsTrue(doorScript.IsOpen);

        testValidObject.transform.position += new Vector3(3, 0, 0);

        yield return new WaitForSeconds(.1f);

        Assert.IsEmpty(plateScript.OnPlateObjs);
        Assert.IsFalse(doorScript.IsOpen);
    }

    [UnityTest]

    public IEnumerator EnterTrigger_OneFromTwoObjExit_DoorRemainOpen()
    {
        GameObject testPlate = _SetUpPlate();
        GameObject testDoors = _SetUpDoor();
        GameObject testValidObject = _SetUpPlayerObject();
        GameObject testValidObject2 = _SetUpPlayerObject();

        PlateController plateScript = testPlate.GetComponent<PlateController>();
        DoorController doorScript = testDoors.GetComponent<DoorController>();

        plateScript.targetDoor = doorScript;
        testValidObject.transform.position = testPlate.transform.position;
        testValidObject2.transform.position = testPlate.transform.position;

        yield return new WaitForSeconds(.1f);

        Assert.AreEqual(2, plateScript.OnPlateObjs.Count);
        Assert.IsTrue(doorScript.IsOpen);

        testValidObject.transform.position += new Vector3(3, 0, 0);

        yield return new WaitForSeconds(.1f);

        Assert.AreEqual(1, plateScript.OnPlateObjs.Count);
        Assert.IsTrue(doorScript.IsOpen);
    }

    private GameObject _SetUpPlayerObject() 
    {
        GameObject testObject = new GameObject();
        
        testObject.AddComponent<BoxCollider2D>();
        testObject.AddComponent<Rigidbody2D>();
        testObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

        testObject.layer = 8;

        return testObject;
    }

    private GameObject _SetUpDoor()
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

    private GameObject _SetUpPlate()
    {
        GameObject testPlate = new GameObject();

        testPlate.AddComponent<BoxCollider2D>();
        testPlate.GetComponent<BoxCollider2D>().isTrigger = true;   

        testPlate.AddComponent<PlateController>();

        return testPlate;
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
