using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("EditMode")]
[assembly: InternalsVisibleTo("PlayMode")]

public class PuzzleController : MonoBehaviour
{

    [SerializeField] internal List<PlateController> plates = new();
    [SerializeField] internal List<SwitchController> switches = new();

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

        if (plates.Count > 0) 
        {
            plates.ForEach(
                (plate) => plate.IsPuzzleComplete = _puzzleDest.IsPlayerReached
            );
        }

        if (switches.Count > 0)
        {
            switches.ForEach(
               (switchScript) => switchScript.IsPuzzleComplete = _puzzleDest.IsPlayerReached
            );
        }
        
    }
}
