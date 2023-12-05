using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompBotPanelController : Interactable
{
    public bool IsControllingThis;

    void Start()
    {
        
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void HandleInteract()
    {
        switch (ControllingManager.Instance.CurrentControl) 
        {
            case ControllingManager.Control.CompBot:
                ControllingManager.Instance.ChangeControl(
                    ControllingManager.Control.PlayerMain
                );
                break;
            case ControllingManager.Control.PlayerMain: 
                ControllingManager.Instance.ChangeControl( 
                    ControllingManager.Control.CompBot 
                ); 
                break;
        }
    }
}
