using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CompBotManager : MonoBehaviour
{

    public static CompBotManager instance;

    private const float HOLD_DOWN_TIME = 1f;

    // Temporary SerializeField (Need Level Manager To Manage Area)
    //[Header("TEMP FIELD (WILL IMPLEMENT LATER VIA LEVEL MANAGER)")]
    
    private List<ActiveAreaCompBot> _activeAreas;

    public bool IsInActiveArea = false;

    private bool _isHoldCompBotKey = false;
    private bool _isReadyToSwitch = false;
    private bool _isKeyLock = false;

    private float _holdTimer = HOLD_DOWN_TIME;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        _activeAreas = FindObjectsOfType<ActiveAreaCompBot>().ToList<ActiveAreaCompBot>();
        //Debug.Log(_activeAreas.Count);
    }

    void Update()
    {
        OnSwitchCompBot();

        HandleCountDown();
        HandleSwitchCompBot();
    }

    private void OnSwitchCompBot()
    {
        if (Input.GetKeyUp(KeyCode.R)) _isKeyLock = false;

        if (_isKeyLock) return;
        _isHoldCompBotKey = Input.GetKey(KeyCode.R);
    }

    private void HandleCountDown()
    {
        _holdTimer = _isHoldCompBotKey ? _holdTimer - Time.deltaTime : HOLD_DOWN_TIME;
        if (_holdTimer <= 0)
        {
            _isReadyToSwitch = true;
        }
    }

    private void HandleSwitchCompBot()
    {
        if (IsInActiveArea && _isReadyToSwitch)
        {
            _isReadyToSwitch = false;
            _LockInteractKey();

            switch (ControllingManager.instance.CurrentControl) 
            {
                case ControllingManager.Control.CompBot:
                    HandleBackgroundToPlayer();
                    SwitchControl(ControllingManager.Control.PlayerMain);
                    break;
                case ControllingManager.Control.PlayerMain:
                    HandleBackgroundToCompBot();
                    SwitchControl(ControllingManager.Control.CompBot);
                    break;
            }
        }
    }

    private void SwitchControl(ControllingManager.Control characterToSwitch) 
    {
        ControllingManager.instance.ChangeControl(characterToSwitch);
    }

    private void HandleBackgroundToCompBot() 
    {
        
    }

    private void HandleBackgroundToPlayer() 
    {
        
    }

    private void _LockInteractKey()
    {
        _isHoldCompBotKey = false;
        _isKeyLock = true;
    }

}
