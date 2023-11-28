using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{

    [SerializeField] bool isSpawnTrigger;
    [field: SerializeField] public int enemyAmount { get; private set; } = 1;
    [SerializeField] private float spawnDelay = 0.5f;

    private Transform _spawnPos;
    private Collider2D _enemySpawnTrigger;

    private void Awake()
    {
        _spawnPos = GetComponentsInChildren<Transform>()[1];
        _enemySpawnTrigger = GetComponent<Collider2D>();
        Debug.Log(_spawnPos.position);
    }

    void Start()
    {
       
    }

    void Update()
    {

    }

    internal IEnumerator HandleSpawn(int amount, float spawnDelay) 
    {
        for (int i = 0; i < amount; i++) 
        {
            GameObject newObj = EnemyPoolingManager.Instance.GetEnemyFromPool();
            if (!newObj)
            {
                Debug.LogError("Cannot Find Object From EnemyPool");
                continue;
            }
            newObj.transform.position = _spawnPos.position;
            newObj.SetActive(true);
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != 8) return;
        StartCoroutine(HandleSpawn(amount: enemyAmount, spawnDelay: spawnDelay));
        _enemySpawnTrigger.enabled = false; 
    }
}
