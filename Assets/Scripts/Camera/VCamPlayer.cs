using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VCamPlayer : MonoBehaviour
{
    private CinemachineVirtualCamera _playerCam;

    private PlayerController _player;
    private CompBotController _botController;
    private ClawMachineController _clawMachineController;

    private List<GameObject> _allClawMachineGO;
    private List<ClawMachineController> _allClawMachine;

    private void Awake()
    {
        _playerCam = GetComponent<CinemachineVirtualCamera>();

        _player = GameObject
            .FindGameObjectWithTag("PlayerCat")
            .GetComponent<PlayerController>();

        _botController = GameObject
            .FindGameObjectWithTag("PlayerCompBot")
            .GetComponent<CompBotController>();

        _allClawMachineGO = GameObject
            .FindGameObjectsWithTag("ClawMachine")
            .ToList();

        _allClawMachineGO.ForEach((clawGO) => {
            _allClawMachine.Add(clawGO.GetComponent<ClawMachineController>());
        });
    }

    void Start()
    {
        
    }

    void Update()
    {
        switch (ControllingManager.instance.CurrentControl) 
        {
            case ControllingManager.Control.PlayerMain:
                SetCamCat();
                break;
            case ControllingManager.Control.ClawMachine:
                SetCamClawMachine();
                break;
            case ControllingManager.Control.CompBot:
                SetCamCompBot();
                break;
        }   
    }

    private void SetCamCat() 
    {
        _playerCam.Follow = _player.transform;
    }

    private void SetCamClawMachine() 
    {
        ClawMachineController controlledClaw = _allClawMachine.Find((cm) => cm.IsControlling);
        if (!controlledClaw) return;
        _playerCam.Follow = controlledClaw.transform;   
    }

    private void SetCamCompBot() 
    {
        Debug.Log("set");
        _playerCam.Follow = _botController.transform;
    }
}
