using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BombPoolingManager : MonoBehaviour
{
    public static BombPoolingManager Instance;

    [SerializeField] private GameObject bombPrefab;

    private List<GameObject> _bombPool = new List<GameObject>();
    private int _poolAmount = 5;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
    }
    void Start()
    {
        for (int i = 0; i < _poolAmount; i++)
        {
            GameObject newObj = Instantiate(bombPrefab, transform);
            newObj.SetActive(false);
            _bombPool.Add(newObj);
        }
    }

    void Update()
    {

    }

    public GameObject GetBombFromPool()
    {
        GameObject notActiveObj = _bombPool.FirstOrDefault((obj) => !obj.activeSelf);
        return notActiveObj;
    }
}
