using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataPersistenceManager : MonoBehaviour
{
    public static DataPersistenceManager Instance { get; private set; }

    [SerializeField] private bool turnOnDebug = false;

    [Header("File Storage")]
    [SerializeField] private string fileName;

    private GameData _gameData;
    private List<IDataPersistence> _dataPersistObjs;

    private FileDataHandler _fileHandler;

    private void Awake()
    {
        if (Instance != null) 
        {
            Debug.LogError("More Than One Instance of DataPersistenceManager Exist");
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        _fileHandler = new FileDataHandler(
            fileDir: Application.persistentDataPath, 
            fileName: fileName
        );
        _dataPersistObjs = GetAllDataPersistObjects();
        LoadGameData();
    }


    private void Update()
    {
        if (turnOnDebug && Input.GetKeyDown(KeyCode.P)) 
        {
            Debug.Log("[DEBUG] Forced Game Save");
            SaveGameData();
        }
    }

    public void NewGameData() 
    {
        _gameData = new GameData();
    }
    public void LoadGameData()
    {
        _gameData = _fileHandler.LoadFromFile();

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

        _fileHandler.SaveToFile(_gameData);
    }

    private List<IDataPersistence> GetAllDataPersistObjects() 
    {
        IEnumerable<IDataPersistence> data = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(data);
    }
}
