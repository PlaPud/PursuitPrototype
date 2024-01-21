using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorePoolingManager : MonoBehaviour
{
    public static CorePoolingManager Instance { get; private set; }

    [SerializeField] private GameObject corePrefab;
    [SerializeField] private int poolAmount = 20;

    private List<GameObject> _corePool = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null) 
        {
            Debug.LogError ("There is more than one CorePoolingManager in the scene!");
            return;
        }

        Instance = this;
    }

    void Start()
    {
        for (int i = 0; i < poolAmount; i++)
        {
            GameObject newObj = Instantiate(corePrefab, transform);
            newObj.SetActive(false);
            newObj.transform.position = new Vector3(
                newObj.transform.position.x, 
                newObj.transform.position.y, 
                0f
            );
            _corePool.Add(newObj);
        }
        
    }

    public GameObject GetCoreFromPool()
    {
        GameObject notActiveObj = _corePool.FirstOrDefault((obj) => !obj.activeSelf);
        return notActiveObj;
    }

}
