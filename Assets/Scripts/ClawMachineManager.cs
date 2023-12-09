using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawMachineManager : MonoBehaviour
{
    public static ClawMachineManager Instance;  

    private ClawPanelController[] _clawPanels;

    public bool IsControlClaw { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More Than One Instance of ClawMachineManager Exist");
        }

        Instance = this;

        _clawPanels = FindObjectsOfType<ClawPanelController>();
    }
    void Start()
    {

    }

    void Update()
    {
        IsControlClaw = ControllingManager.Instance.CurrentControl == ControllingManager.Control.ClawMachine;
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
