using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private PlayerController _playerCat;

    private void Awake()
    {
        if (Instance != null) 
        {
            Debug.LogError("[ERROR] More than one GameManager in Scene.");
            return;
        } 
        
        Instance = this;
    }

    void Start()
    {
        _playerCat = ControllingManager.Instance.CatController;
        PlayerHealth.Instance.OnPlayerDied += HandlePlayerDeath;
    }

    void Update()
    {

    }

    private void HandlePlayerDeath() 
    {
        // Reload Scene
        DataPersistenceManager.Instance.LoadGameData();
    }

    private void HandleOnPause() 
    {
        
    }
}
