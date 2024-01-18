using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("EditMode")]
[assembly: InternalsVisibleTo("PlayMode")]
public class LevelController : MonoBehaviour, IDataPersistence
{
    [Header("Level ID")]
    [SerializeField] private string levelId;

    [Header("Level Config")]
    public bool IsLevelComplete;
    [SerializeField] internal EnemyAreaController enemyAreaCtrl;
    [SerializeField] internal LevelCompleteTrigger puzzleCompleteTrigger;

    public Action<bool> OnLevelStatusLoaded;

    void Start()
    {

    }

    void Update()
    {
        if (IsLevelComplete)
        {
            if (enemyAreaCtrl) enemyAreaCtrl.IsAllSpawnsCleared = true;

            if (puzzleCompleteTrigger) puzzleCompleteTrigger.IsPlayerReached = true;

            return;
        }

        CheckLevelElementComplete();
    }

    public void CheckLevelElementComplete()
    {
        bool oldStatus = IsLevelComplete;
        bool isPuzzleDone = IsPuzzleDoneOrNull();
        bool isEnemyAreaCleared = IsEnemyAreaClearedOrNull();

        IsLevelComplete = isPuzzleDone && isEnemyAreaCleared;
    }

    public bool IsPuzzleDoneOrNull() 
    {
        if (!puzzleCompleteTrigger) return true;

        if (puzzleCompleteTrigger.IsPlayerReached) return true;

        return false;
    }

    public bool IsEnemyAreaClearedOrNull() 
    {
        if (!enemyAreaCtrl) return true;

        if (enemyAreaCtrl.IsAllSpawnsCleared) return true;  

        return false;
    }

    public void LoadData(GameData data)
    {
        if (!data.SavedLevelComplete.ContainsKey(levelId)) return;
        bool oldStatus = IsLevelComplete;
        IsLevelComplete = data.SavedLevelComplete[levelId];
        OnLevelStatusLoaded?.Invoke(IsLevelComplete);
    }

    public void SaveData(GameData data)
    {
        if (levelId == "") return;

        if (data.SavedLevelComplete.ContainsKey(levelId)) 
        {
            data.SavedLevelComplete[levelId] = IsLevelComplete;
            return;
        }

        data.SavedLevelComplete.Add(levelId, IsLevelComplete);
    }

    [ContextMenu("Generate GUID for This Key Item")]
    private void _GenerateItemGuid() => levelId = "L-" + System.Guid.NewGuid().ToString();

}
