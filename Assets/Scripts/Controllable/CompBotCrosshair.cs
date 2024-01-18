using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompBotCrosshair : MonoBehaviour
{
    [SerializeField] private CompBotController compBot;

    private SpriteRenderer _crosshairSR;

    private void Awake()
    {
        _crosshairSR = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        _crosshairSR.enabled = false;
    }

    void Update()
    {
        if (!ControllingManager.Instance.IsControllingCompBot || !ControllingManager.Instance.IsMatchedCompBot(compBot))
        {
            _crosshairSR.enabled = false;
            return;
        }

        UpdateCrosshair();
    }

    private void UpdateCrosshair()
    {
        if (!compBot.GrapplerHit)
        {
            _crosshairSR.enabled = false;   
            return;
        }

        _crosshairSR.enabled = true;

        if (compBot.IsShootHold) return;
        
        transform.position = compBot.GrapplerHit.point;
    }
}
