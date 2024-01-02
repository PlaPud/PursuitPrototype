using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WarningPoolingManager : MonoBehaviour
{
    public static WarningPoolingManager Instance;
    
    [SerializeField] private GameObject warningPrefab;

    private List<GameObject> _warningPool = new List<GameObject>();
    private int _poolAmount = 20;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
    }

    void Start()
    {
        for (int i = 0; i < _poolAmount; i++)
        {
            GameObject newObj = Instantiate(warningPrefab, transform);
            newObj.SetActive(false);
            _warningPool.Add(newObj);
        }
    }

    void Update()
    {

    }

    public GameObject GetWarningFromPool()
    {
        GameObject notActiveObj = _warningPool.FirstOrDefault((obj) => !obj.activeSelf);
        return notActiveObj;
    }

}
