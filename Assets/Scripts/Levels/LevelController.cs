using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class LevelController : MonoBehaviour, IDataPersistence
{
    [Header("Level ID")]
    [SerializeField] private string levelId;

    [Header("Level Config")]
    public bool IsLevelComplete;
    [SerializeField] LayerMask playerMask;
    [SerializeField] EnemyAreaController enemyAreaCtrl;
    [SerializeField] LevelCompleteTrigger puzzleCompleteTrigger;

    void Start()
    {
       
    }

    void Update()
    {
        if (IsLevelComplete)
        {
            if (enemyAreaCtrl) 
            enemyAreaCtrl.IsEnemyAllCleared = true;

            if (puzzleCompleteTrigger) 
            puzzleCompleteTrigger.IsPlayerReached = true;

            return;
        }

        CheckLevelElementComplete();
    }

    private void CheckLevelElementComplete()
    {

        bool isPuzzleDone = _IsPuzzleDoneOrNull();
        bool isEnemyAreaCleared = _IsEnemyAreaClearedOrNull();

        IsLevelComplete = isPuzzleDone && isEnemyAreaCleared;
    }

    private bool _IsPuzzleDoneOrNull() 
    {
        if (!puzzleCompleteTrigger) return true;

        if (puzzleCompleteTrigger.IsPlayerReached) return true;

        return false;
    }

    private bool _IsEnemyAreaClearedOrNull() 
    {
        if (!enemyAreaCtrl) return true;

        if (enemyAreaCtrl.IsEnemyAllCleared) return true;  

        return false;
    }

    public void LoadData(GameData data)
    {
        if (!data.SavedLevelComplete.ContainsKey(levelId)) return;
        IsLevelComplete = data.SavedLevelComplete[levelId];
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
    private void _GenerateItemGuid() => levelId = System.Guid.NewGuid().ToString();

}
