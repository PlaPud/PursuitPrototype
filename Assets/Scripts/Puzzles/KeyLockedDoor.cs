using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyLockedDoor : DoorController
{
    [Header("Detector")]
    [SerializeField] ItemDetectorController detector;

    void Update()
    {
        if (detector.IsUnlocked && !IsOpen) 
        {
            SetOpenDoor();
            return;
        }

        base.Update();
    }
}
