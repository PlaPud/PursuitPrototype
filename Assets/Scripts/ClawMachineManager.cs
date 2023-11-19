using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawMachineManager : MonoBehaviour
{
    public static ClawMachineManager instance;  

    private ClawPanelController[] _clawPanels;

    public bool IsControlClaw { get; private set; }

    private void Awake()
    {
        instance = this;    
        _clawPanels = FindObjectsOfType<ClawPanelController>();
    }
    void Start()
    {

    }

    void Update()
    {
        IsControlClaw = ControllingManager.instance.CurrentControl == ControllingManager.Control.ClawMachine;
        AssignMachine();
    }

    private void AssignMachine()
    {
        foreach (ClawPanelController panel in _clawPanels)
        {
            panel.IsControllingThis = (panel.PlayerCD != null);
        }
    }
}
