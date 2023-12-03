using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAreaController : MonoBehaviour
{
    public bool IsEnemyAllCleared;


    private List<EnemySpawnPoint> _enemySpawns;
    private List<DoorController> _lockedDoors;

    public Action OnEnemyAllCleared;

    private void Awake()
    {
        _enemySpawns = GetComponentsInChildren<EnemySpawnPoint>().ToList();
        _lockedDoors = GetComponentsInChildren<DoorController>().ToList();
    }

    void Start()
    {

    }

    void Update()
    {
        CheckLockDown();
        CheckEnemyAreaClear();
    }

    private void CheckLockDown()
    {
        bool anyTriggerGoesOff = _enemySpawns.Any(
                (enemySp) => !enemySp.IsEnemyInAreaClear && enemySp.IsTriggerGoesOff
            );

        if (!anyTriggerGoesOff) return;
        _LockDownDoors();
    }

    private void _LockDownDoors() 
    {
        _lockedDoors.ForEach((door) => {
            door.SetCloseDoor();
        });
    }

    private void CheckEnemyAreaClear() 
    {
        if (IsEnemyAllCleared) return;

        bool anyAreaNotCleared = _enemySpawns.Any((enemySp) => !enemySp.IsEnemyInAreaClear);

        IsEnemyAllCleared = !anyAreaNotCleared;

        if (!IsEnemyAllCleared) return;
        _UnlockAllDoors();
    }
    private void _UnlockAllDoors()
    {
        _lockedDoors.ForEach((door) => {
            door.SetOpenDoor();
        });
    }

}
