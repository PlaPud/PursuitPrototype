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

    private List<GameObject> _allClawMachineGO = new();
    private List<GameObject> _allCompBotGO = new();
    private List<ClawMachineController> _allClawMachine = new();
    private List<CompBotController> _allCompBot = new();

    [SerializeField] private float normalOrthSize = 9;
    [SerializeField] private float zoomedOrthSize = 6;

    private void Awake()
    {
        _playerCam = GetComponent<CinemachineVirtualCamera>();

        _player = GameObject
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
        switch (ControllingManager.Instance.CurrentControl) 
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
        _playerCam.m_Lens.OrthographicSize = normalOrthSize;
    }

    private void SetCamClawMachine() 
    {
        if (_allClawMachine.Count >= 0) 
        {
            _allClawMachine = _allClawMachineGO.Select(
                cb => cb.GetComponent<ClawMachineController>()
                ).ToList();
        }
        ClawMachineController controlledClaw = _allClawMachine.Find((cm) => cm.IsControlling);
        if (!controlledClaw) return;
        _playerCam.Follow = controlledClaw.transform;
        _playerCam.m_Lens.OrthographicSize = normalOrthSize;
    }

    private void SetCamCompBot() 
    {
        if (_allCompBot.Count >= 0)
        {
            _allCompBot = _allCompBotGO.Select(
                cb => cb.GetComponent<CompBotController>()
                ).ToList();
        }
        CompBotController compBot = _allCompBot.Find((cb) => cb.IsControlling);
        if (!compBot) return;
        _playerCam.Follow = compBot.transform;
        _playerCam.m_Lens.OrthographicSize = zoomedOrthSize;
    }
}
