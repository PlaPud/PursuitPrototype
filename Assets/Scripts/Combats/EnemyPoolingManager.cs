using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyPoolingManager : MonoBehaviour
{

    public static EnemyPoolingManager Instance;

    [SerializeField] private GameObject enemyBotPrefab;

    private List<GameObject> _enemyPool = new List<GameObject>();
    private int _poolAmount = 20;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
    }
    void Start()
    {
        for (int i = 0; i < _poolAmount; i++) 
        {
            GameObject newObj = Instantiate(enemyBotPrefab, transform);
            newObj.SetActive(false);
            _enemyPool.Add(newObj);
        }
    }

    void Update()
    {
        
    }

    public GameObject GetEnemyFromPool() 
    {
        GameObject notActiveObj = _enemyPool.FirstOrDefault((obj) => !obj.activeSelf);
        return notActiveObj;
    }
}
