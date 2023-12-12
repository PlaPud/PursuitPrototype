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
    public static Action OnCombatEnd;
    public static Action OnCombatStart;

    public bool IsLockedDown { get; private set; } = false;

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

        if (!anyTriggerGoesOff || IsLockedDown) return;
        _LockDownDoors();
    }

    private void _LockDownDoors() 
    {
        IsLockedDown = true;
        _lockedDoors.ForEach((door) => {
            door.SetCloseDoor();
        });
        OnCombatStart?.Invoke();
    }

    private void CheckEnemyAreaClear() 
    {
        if (IsEnemyAllCleared) return;

        bool anyAreaNotCleared = _enemySpawns.Any((enemySp) => !enemySp.IsEnemyInAreaClear);

        IsEnemyAllCleared = !anyAreaNotCleared;

        if (!IsEnemyAllCleared || !IsLockedDown) return;
        _UnlockAllDoors();
    }
    private void _UnlockAllDoors()
    {
        IsLockedDown = false;
        _lockedDoors.ForEach((door) => {
            door.SetOpenDoor();
        });
        OnCombatEnd?.Invoke();
    }

}
