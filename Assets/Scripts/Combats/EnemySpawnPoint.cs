using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{

    public bool IsEnemyInAreaClear;

    [SerializeField] bool isSpawnTrigger;

    [field: Header("Enemy Spawning")]
    [field: SerializeField] public int enemyAmount { get; private set; } = 1;
    [SerializeField] private float secPerSpawn = 0.5f;
    [SerializeField] private float spawnDelay = 1f;
    [SerializeField] private float spawnWarningDelay = 0.25f;

    public bool HasSpawnedAll { get; private set; } = false;
    public bool IsSpawned { get; private set; } = false;

    private Transform _spawnPos;
    private Collider2D _enemySpawnTrigger;

    private List<GameObject> _enemiesInField = new List<GameObject>();

    public bool IsTriggerGoesOff => !_enemySpawnTrigger.enabled;

    public static Action OnEnterCombat;

    public Action OnBeforeSpawn;

    private void Awake()
    {
        _spawnPos = GetComponentsInChildren<Transform>()[1];
        _enemySpawnTrigger = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (IsEnemyInAreaClear)
        {
            _OverrideAreaClear();
            return;
        }

        _ClearDisableEnemiesRef();
        _CheckClear();
    }

    private void _OverrideAreaClear()
    {
        _enemySpawnTrigger.enabled = false;
        IsSpawned = true;
        HasSpawnedAll = true;
        _DespawnAll();
    }

    private void _DespawnAll() 
    {
        if (!_enemySpawnTrigger.enabled || _enemiesInField.Count <= 0) return;
        _enemiesInField.ForEach((enemy) => { enemy.SetActive(false); });
        _enemiesInField.Clear();
    }

    private void _ClearDisableEnemiesRef() 
    {
        _enemiesInField = _enemiesInField.Where((enemy) => enemy.activeSelf).ToList();
    }

    private void _CheckClear() 
    {
        bool isCleared = HasSpawnedAll && !_enemySpawnTrigger.enabled && _enemiesInField.Count <= 0;
        IsEnemyInAreaClear = isCleared;
    }

    internal IEnumerator HandleSpawn(int amount, float secPerSpawn, float spawnDelay) 
    {
        yield return new WaitForSeconds(spawnDelay);

        for (int i = 0; i < amount; i++) 
        {
            OnBeforeSpawn?.Invoke();
            
            yield return new WaitForSeconds(spawnWarningDelay);
            
            GameObject newObj = EnemyPoolingManager.Instance.GetEnemyFromPool();

            if (!newObj)
            {
                Debug.LogError("Cannot Find Object From EnemyPool.");
                continue;
            }

            _enemiesInField.Add(newObj);
            newObj.transform.position = _spawnPos.position;
            newObj.SetActive(true);

            yield return new WaitForSeconds(secPerSpawn);
        }

        HasSpawnedAll = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != 8) return;
        StartCoroutine(
            HandleSpawn(
                amount: enemyAmount, 
                secPerSpawn: secPerSpawn,
                spawnDelay: spawnDelay
            )
        );
        IsSpawned = true;
        _enemySpawnTrigger.enabled = false;
        OnEnterCombat?.Invoke();
    }
}
