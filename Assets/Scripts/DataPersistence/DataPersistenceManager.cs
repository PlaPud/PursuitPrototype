using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataPersistenceManager : MonoBehaviour
{
    public static DataPersistenceManager Instance { get; private set; }

    private GameData _gameData;
    private List<IDataPersistence> _dataPersistObjs;

    private void Start()
    {
        
    }

    private void Awake()
    {
        if (Instance != null) 
        {
            Debug.LogError("More than one instance initialized in scene.");
            return;
        }
        Instance = this;
    }

    public void NewGameData() 
    {
        _gameData = new GameData();
    }
    public void LoadGameData()
    {
        if (_gameData == null)
        {
            Debug.Log("No save file found. Initialize new game data.");
            NewGameData();
            return;
        }

        _dataPersistObjs.ForEach((dataPersistObj) => {
            dataPersistObj.LoadData(_gameData);
        });

    }
    public void SaveGameData() 
    {
        _dataPersistObjs.ForEach((dataPersistObj) => {
            dataPersistObj.SaveData(_gameData);
        });
    }

    private List<IDataPersistence> GetAllDataPersistObjects() 
    {
        IEnumerable<IDataPersistence> data = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(data);
    }
}
