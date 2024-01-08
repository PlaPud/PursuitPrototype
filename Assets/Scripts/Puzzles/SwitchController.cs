using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("EditMode")]
[assembly: InternalsVisibleTo("PlayMode")]

public class SwitchController : Interactable
{

    public bool IsPuzzleComplete;

    [SerializeField] internal List<DoorController> toggleDoorsTarget = new List<DoorController>();

    void Start()
    {
        
    }

    protected override void Update()
    {
        if (IsPuzzleComplete) 
        {
            foreach (DoorController target in toggleDoorsTarget) 
            {
                target.SetOpenDoor();
            }
            return;
        };

        base.Update();
    }

    public override void HandleInteract()
    {
        foreach (DoorController target in toggleDoorsTarget)
        {
            _ToggleDoor(target);
        }
    }

    private void _ToggleDoor(DoorController targetDoor)
    {
        if (targetDoor.IsOpen)
        {
            targetDoor.SetCloseDoor();
        }
        else
        {
            targetDoor.SetOpenDoor();
        }
    }
}
