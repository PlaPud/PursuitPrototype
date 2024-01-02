using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnWarning : MonoBehaviour
{
    private EnemySpawnPoint _enemySp;

    private Transform _spawnTransform;

    private void Awake()
    {
        _enemySp = GetComponent<EnemySpawnPoint>();
        _spawnTransform = _enemySp.gameObject.GetComponentsInChildren<Transform>()[1];
    }

    void Start()
    {
        _enemySp.OnBeforeSpawn += SetWarningEffect;
    }

    void Update()
    {
       
    }

    private void SetWarningEffect() 
    {
        GameObject warningEff = WarningPoolingManager.Instance.GetWarningFromPool();
        
        if (!warningEff) 
        {
            Debug.LogError("WarningPoolingManager.Instance.GetWarningFromPool() returns null");
            return;
        }

        warningEff.transform.position = _spawnTransform.position;
        warningEff.SetActive(true);
    }
}
