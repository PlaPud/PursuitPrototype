using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageClearCheckpoint : MonoBehaviour, IDataPersistence
{
    [Range(1, 3)]
    [SerializeField] private int stageNumber = 1;
    public bool IsCompleted { get; private set; } = false;

    private CheckPoint _checkPoint;
    private int _clearedStage = 0;

    private void Awake()
    {
        _checkPoint = GetComponent<CheckPoint>();
    }

    void Start()
    {
        _checkPoint.OnBeforeSave += ChangeClearedStage;
    }

    void Update()
    {
        
    }

    private void ChangeClearedStage() 
    {
        if (IsCompleted) return;
        IsCompleted = true;
        if (stageNumber < 3) _clearedStage++;
    }
    public void SaveData(GameData data)
    {
        data.SavedStagesCompleted = _clearedStage;
    }

    public void LoadData(GameData data)
    {
        _clearedStage = data.SavedStagesCompleted;
        if (_clearedStage >= stageNumber) IsCompleted = true;
    }

}
