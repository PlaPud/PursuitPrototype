using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestPlayPressureDoor
{
    [UnityTest]
    public IEnumerator SetDoorPositionIdle_IsOpen_TransformPositionOpen()
    {
        GameObject testDoorObject = SetUpDoor();
        DoorController doorScript = testDoorObject.GetComponent<DoorController>();
        doorScript.Start();
        doorScript.SetOpenDoor();
        yield return new WaitForSeconds(1f);
        Assert.AreEqual((Vector2) doorScript.DoorBody.transform.position, doorScript.DoorOpenPos);
    }

    [UnityTest]
    public IEnumerator SetDoorPositionIdle_IsNotOpen_TransformPositionClose()
    {
        GameObject testDoorObject = SetUpDoor();
        DoorController doorScript = testDoorObject.GetComponent<DoorController>();
        doorScript.Start();
        doorScript.SetCloseDoor();
        yield return new WaitForSeconds(1f);
        Assert.AreEqual((Vector2)doorScript.DoorBody.transform.position, doorScript.DoorClosePos);
    }

    private static GameObject SetUpDoor()
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
