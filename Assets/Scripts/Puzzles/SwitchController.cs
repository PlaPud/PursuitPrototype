using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : Interactable
{

    public bool IsPuzzleComplete;

    private const int PLAYER_LAYER = 8;

    [SerializeField] List<DoorController> toggleDoorsTarget = new List<DoorController>();

    public bool IsPressed { get; private set; } = false;
    private bool _canPress;
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
