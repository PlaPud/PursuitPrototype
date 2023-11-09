using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleController : MonoBehaviour
{
    public bool IsLevelComplete;
    public bool IsPuzzleComplete;

    [SerializeField] List<PlateController> plates;
    [SerializeField] List<SwitchController> switches;

    void Start()
    {
        
    }

    void Update()
    {
        IsPuzzleComplete = IsLevelComplete;
        foreach (PlateController plateScript in plates) 
        {
            plateScript.IsPuzzleComplete = IsPuzzleComplete;
        }
        foreach (SwitchController switchScript in switches) 
        {
            switchScript.IsPuzzleComplete = IsPuzzleComplete;
        }
    }
}
