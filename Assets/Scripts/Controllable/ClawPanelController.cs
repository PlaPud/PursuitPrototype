using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
        switch (ControllingManager.Instance.CurrentControl)
        {
            case ControllingManager.Control.ClawMachine:
                ControllingManager.Instance.ChangeControl(
                    ControllingManager.Control.PlayerMain
                );
                break;
            case ControllingManager.Control.PlayerMain:
                ControllingManager.Instance.ChangeControl(
                    ControllingManager.Control.ClawMachine
                );
                break;
        }
    }

    
}

