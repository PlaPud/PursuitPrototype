using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ControllingManager : MonoBehaviour
{
    public enum Control {
        PlayerMain,
        CompBot,
        ClawMachine
    }

    public static ControllingManager Instance;

    public PlayerController CatController { get; private set; }
    public CompBotController CompBotControlled { get; private set; }
    public ClawMachineController ClawMachineControlled { get; private set; }

    private List<GameObject> _allClawMachineGO = new();
    private List<GameObject> _allCompBotGO = new();
    private List<ClawMachineController> _allClawMachine = new();
    private List<CompBotController> _allCompBot = new();

    public Control CurrentControl { get; private set; } = Control.PlayerMain;

    public bool IsControllingCat => CurrentControl == Control.PlayerMain;
    public bool IsControllingCompBot => CurrentControl == Control.CompBot;
    public bool IsControllingClawMachine => CurrentControl == Control.ClawMachine;

    private void Awake()
    {
        Instance = this;

        CatController = GameObject
            .FindGameObjectWithTag("PlayerCat")
            .GetComponent<PlayerController>();

        _allCompBotGO = GameObject
            .FindGameObjectsWithTag("PlayerCompBot")
            .ToList();

        _allClawMachineGO = GameObject
            .FindGameObjectsWithTag("ClawMachine")
            .ToList();

    }

    void Start()
    {

    }

    void Update()
    {
        
    }

    public bool IsMatchedCompBot(CompBotController compBot) 
    {
        if (!IsControllingCompBot || compBot == null) return false;

        if (compBot.Equals(CompBotControlled)) return true;

        return false;
    }

    public bool IsMatchedClawMachine(ClawMachineController clawMachine)
    {
        if (!IsControllingClawMachine || clawMachine == null) return false;

        if (clawMachine.Equals(CompBotControlled)) return true;

        return false;
    }

    public void ChangeControl(Control switchControl)
    {
        bool cannotSwitch =
            (IsControllingCompBot && switchControl == Control.ClawMachine)
                ||
            (IsControllingClawMachine && switchControl == Control.CompBot);

        if (cannotSwitch) return;

        _InitControllerRef(switchControl);

        CurrentControl = switchControl;
    }

    private void _InitControllerRef(Control switchControl)
    {
        switch (switchControl)
        {
            case Control.PlayerMain:
                CompBotControlled = null;
                ClawMachineControlled = null;
                break;
            case Control.CompBot:
                CompBotControlled = _GetControlledCompBot();
                Cursor.visible = true;
                break;
            case Control.ClawMachine:
                ClawMachineControlled = _GetControlledClawMachine();
                break;
        }
    }

    private CompBotController _GetControlledCompBot() 
    {
        if (_allCompBot.Count >= 0)
        {
            _allCompBot = _allCompBotGO.Select(
                cb => cb.GetComponent<CompBotController>()
                ).ToList();
        }
        CompBotController compBot = _allCompBot.Find((cb) => cb.IsControlling);
        return compBot;
    }

    private ClawMachineController _GetControlledClawMachine() 
    {
        if (_allClawMachine.Count >= 0)
        {
            _allClawMachine = _allClawMachineGO.Select(
                cb => cb.GetComponent<ClawMachineController>()
                ).ToList();
        }

        ClawMachineController controlledClaw = _allClawMachine.Find((cm) => cm.IsControlling);
        return controlledClaw;
    }

}
