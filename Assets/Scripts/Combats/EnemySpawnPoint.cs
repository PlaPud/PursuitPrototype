using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{

    [SerializeField] bool isSpawnTrigger;

    void Start()
    {

    }

    void Update()
    {
        HandleSpawn();
    }

    private void HandleSpawn() 
    {
        if (!isSpawnTrigger) return;
        isSpawnTrigger = false;
        GameObject newObj = EnemyPoolingManager.Instance.GetEnemyFromPool();
        newObj.SetActive(true);
    }
}
