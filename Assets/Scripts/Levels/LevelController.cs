using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class LevelController : MonoBehaviour
{

    public bool IsLevelComplete;
    [SerializeField] LayerMask playerMask;
    [SerializeField] PuzzleController puzzle;
    [SerializeField] CombatController combat;
    [SerializeField] LevelCompleteTrigger trigger;

    void Start()
    {
        
    }

    void Update()
    {
        if (!trigger.IsLevelComplete) return;
        IsLevelComplete = true;
        puzzle.IsLevelComplete = true;
    }
}
