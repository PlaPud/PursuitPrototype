using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    public static DataPersistenceManager Instance { get; private set; }

    [SerializeField] private bool turnOnDebug = false;

    [Header("File Storage")]
    [SerializeField] private string fileName;

    private GameData _gameData;
    private List<IDataPersistence> _dataPersistObjs;

    private FileDataHandler _fileHandler;

    public bool IsSaveDataExist => _gameData != null;

    public Action OnLoadedComplete;

    private void Awake()
    {
        if (Instance != null) 
        {
            Debug.Log("Destroy new DataPersistenceManager gameObject. (Old one already exists)");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        _fileHandler = new FileDataHandler(
            fileDir: Application.persistentDataPath, 
            fileName: fileName
        );
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
    {
        _dataPersistObjs = GetAllDataPersistObjects();
        LoadGameData();
    }

    private void Start()
    {
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

        SaveGameData();

        OnLoadedComplete?.Invoke();
    }

    public void LoadGameData()
    {
        _gameData = _fileHandler.LoadFromFile();

        if (_gameData == null)
        {
            Debug.Log("No save file found. Initialize new game data.");
            return;
        }

        _dataPersistObjs.ForEach((dataPersistObj) => {
            dataPersistObj.LoadData(_gameData);
        });

        OnLoadedComplete?.Invoke();

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
