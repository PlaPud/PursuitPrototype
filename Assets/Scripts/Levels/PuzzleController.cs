using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleController : MonoBehaviour
{

    [SerializeField] private List<PlateController> plates;
    [SerializeField] private List<SwitchController> switches;

    private LevelCompleteTrigger _puzzleDest;

    private void Awake()
    {
        _puzzleDest = GetComponentInChildren<LevelCompleteTrigger>();
    }
    void Start()
    {
        
    }

    void Update()
    {
        foreach (PlateController plateScript in plates) 
        {
            plateScript.IsPuzzleComplete = _puzzleDest.IsPlayerReached;
        }
        foreach (SwitchController switchScript in switches) 
        {
            switchScript.IsPuzzleComplete = _puzzleDest.IsPlayerReached;
        }
    }
}
