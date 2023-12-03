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
    [SerializeField] private float secToSpawn = 0.5f;
    [SerializeField] private float spawnDelay = 1f;

    private bool _hasSpawned = false;

    private Transform _spawnPos;
    private Collider2D _enemySpawnTrigger;

    private List<GameObject> _enemiesInField = new List<GameObject>();

    public bool IsTriggerGoesOff => !_enemySpawnTrigger.enabled;

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
        _hasSpawned = true;
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
        bool isCleared = _hasSpawned && !_enemySpawnTrigger.enabled && _enemiesInField.Count <= 0;
        IsEnemyInAreaClear = isCleared;
    }

    internal IEnumerator HandleSpawn(int amount, float secPerSpawn, float spawnDelay) 
    {
        yield return new WaitForSeconds(spawnDelay);

        for (int i = 0; i < amount; i++) 
        {
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

        _hasSpawned = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != 8) return;
        StartCoroutine(
            HandleSpawn(
                amount: enemyAmount, 
                secPerSpawn: secToSpawn,
                spawnDelay: spawnDelay
            )
        );
        _enemySpawnTrigger.enabled = false; 
    }
}
