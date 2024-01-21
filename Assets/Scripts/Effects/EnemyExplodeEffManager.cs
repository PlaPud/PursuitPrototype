using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyExplodeEffManager : MonoBehaviour
{

    public static EnemyExplodeEffManager Instance;

    [SerializeField] private GameObject enemyExplodeEffPrefab;
    [SerializeField] private int poolAmount = 20;

    private List<GameObject> _enemyExplodeEffPool = new List<GameObject>();


    private void Awake()
    {
        if (Instance == null) { Instance = this; }
    }

    private void Start()
    {
        for (int i = 0; i < poolAmount; i++)
        {
            GameObject newObj = Instantiate(enemyExplodeEffPrefab, transform);
            newObj.SetActive(false);
            newObj.transform.position = new Vector3(
                newObj.transform.position.x, 
                newObj.transform.position.y, 
                0f
            );
            _enemyExplodeEffPool.Add(newObj);
        }
    }

    public GameObject GetExplodeEffectFromPool()
    {
        GameObject notActiveObj = _enemyExplodeEffPool.FirstOrDefault((obj) => !obj.activeSelf);
        return notActiveObj;
    }
}
