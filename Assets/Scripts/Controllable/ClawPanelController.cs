using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ControllingManager;

public class ClawPanelController : Interactable
{

    public bool IsControllingThis;

    void Start()
    {

    }

    protected override void Update()
    {
        base.Update();    
    }

    override public void HandleInteract()
    {
        switch (instance.CurrentControl)
        {
            case Control.ClawMachine:
                instance.ChangeControl(Control.PlayerMain);
                break;
            case Control.PlayerMain:
                instance.ChangeControl(Control.ClawMachine);
                break;
        }
    }

    
}

