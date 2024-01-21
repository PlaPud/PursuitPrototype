using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraModeManager : MonoBehaviour
{
    public static ExtraModeManager Instance { get; private set; }

    public int CurrentLevel { get; private set; }

    private PlayerController _player;

    private ExtraLvlData _curXLvlControl;

    public int CoresRemain { get; private set; }
    public float Timer { get; private set; }

    public float ElapsedTime => _curXLvlControl.SecondsToComplete - Timer;

    public bool IsCollectedAllCores => CoresRemain <= 0;
    public bool IsTimeOver => Timer <= 0;

    public bool IsWin { get; private set; } = false;
    
    public bool IsEnd { get; private set; } = false;

    public Action OnLevelWin;
    public Action OnLevelFailed;

    void Awake()
    {
        if (Instance != null) 
        {
            Debug.LogError("ExtraModeManager already exists!");
            return;
        }

        Instance = this;
        
        
    }

    void Start()
    {
        _player = ControllingManager.Instance.CatController;
        CurrentLevel = ExtraLevelSelect.Instance.SelectedLevel;
       
        _curXLvlControl = ExtraLevelData.Instance.levelSpawn[CurrentLevel].GetComponent<ExtraLvlData>();
        _player.transform.position = _curXLvlControl.transform.position;

        _curXLvlControl.FinishPoint.gameObject.SetActive(false);
        CoresRemain = _curXLvlControl.AmountOfCores;
        Timer = _curXLvlControl.SecondsToComplete;

        _GenerateCores();
    }

    private IEnumerator _PlaySoundTest()
    {
        yield return new WaitForSeconds(5f);
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.SavedLevelComplete, transform.position);
    }

    void Update()
    {
        if (IsEnd) return;

        if (_curXLvlControl.StartPoint.transform.gameObject.activeSelf || IsWin) return;

        _CountDown();

        if (!IsCollectedAllCores) return;

        _DisplayFinishMark();
    }

    private void _GenerateCores() 
    {
        for (int i = 0; i < _curXLvlControl.AmountOfCores; i++)
        {
            GameObject core = CorePoolingManager.Instance.GetCoreFromPool();
            core.transform.position = _curXLvlControl.CoresPos[i].position;
            core.SetActive(true);
        }
    }

    private void _DisplayFinishMark()
    {
        _curXLvlControl.FinishPoint.gameObject.SetActive(!IsTimeOver);
    }

    private void _CountDown()
    {
        Timer = Timer >= Time.deltaTime ? Timer - Time.deltaTime : 0f;

        if (IsTimeOver) 
        {
            IsEnd = true;
            OnLevelFailed?.Invoke();
        } 
    }

    public void CollectCore()
    {
        CoresRemain -= CoresRemain > 0 ? 1 : 0;
    }

    public void SetWin() 
    {
        IsWin = true;
        OnLevelWin?.Invoke();   
    }
}
