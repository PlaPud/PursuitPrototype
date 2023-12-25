using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VCamPlayer : MonoBehaviour
{
    private CinemachineVirtualCamera _playerCam;

    [SerializeField] private float normalOrthSize = 9;
    [SerializeField] private float zoomedOrthSize = 6;

    private void Awake()
    {
        _playerCam = GetComponent<CinemachineVirtualCamera>();
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
        _playerCam.Follow = ControllingManager.Instance.CatController.transform;
        _playerCam.m_Lens.OrthographicSize = normalOrthSize;
    }

    private void SetCamClawMachine() 
    {
        ClawMachineController controlledClaw = ControllingManager.Instance.ClawMachineControlled;
        if (!controlledClaw) return;
        _playerCam.Follow = controlledClaw.GetComponentsInChildren<Transform>()[2].transform;
        _playerCam.m_Lens.OrthographicSize = normalOrthSize;
    }

    private void SetCamCompBot() 
    {
        CompBotController compBot = ControllingManager.Instance.CompBotControlled;
        if (!compBot) return;
        _playerCam.Follow = compBot.transform;
        _playerCam.m_Lens.OrthographicSize = zoomedOrthSize;
    }
}
