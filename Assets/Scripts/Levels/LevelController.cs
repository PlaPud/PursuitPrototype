using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class LevelController : MonoBehaviour
{

    public bool IsLevelComplete;
    [SerializeField] LayerMask playerMask;
    [SerializeField] PuzzleController puzzle;
    [SerializeField] EnemyAreaController enemyAreaCtrl;
    [SerializeField] LevelCompleteTrigger levelCompleteTrigger;

    void Start()
    {
        
    }

    void Update()
    {
        if (!levelCompleteTrigger.IsLevelComplete) return;
        IsLevelComplete = true;
        puzzle.IsLevelComplete = true;
    }
}
