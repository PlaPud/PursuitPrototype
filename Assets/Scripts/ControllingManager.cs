using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllingManager : MonoBehaviour
{
    public enum Control {
        PlayerMain,
        CompBot,
        ClawMachine
    }

    public static ControllingManager instance;

    public Control CurrentControl { get; private set; } = Control.PlayerMain;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public void ChangeControl(Control switchControl) 
    {
        bool cannotSwitch =
            (CurrentControl == Control.CompBot && switchControl == Control.ClawMachine)
                ||
            (CurrentControl == Control.ClawMachine && switchControl == Control.CompBot);
        
        if (cannotSwitch) return;
        CurrentControl = switchControl;
    }

}
