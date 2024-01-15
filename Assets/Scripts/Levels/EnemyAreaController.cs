using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("EditMode")]
[assembly: InternalsVisibleTo("PlayMode")]

public class EnemyAreaController : MonoBehaviour
{
    public bool IsAllSpawnsCleared;

    private List<EnemySpawnPoint> _enemySpawns = new();
    private List<DoorController> _lockedDoors = new();

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
        if (IsAllSpawnsCleared) 
        {
            _enemySpawns.ForEach((enemySp) => enemySp.IsEnemyInAreaClear = true);
            return;
        }

        CheckLockDown();
        CheckEnemyAreaClear();
    }

    private void CheckLockDown()
    {
        bool anyTriggerGoesOff = _enemySpawns.Any(
                (enemySp) => enemySp.IsTriggerGoesOff && enemySp.IsSpawned
            );

        if (!anyTriggerGoesOff || IsLockedDown) return;
        _LockDownDoors();
        AudioManager.Instance?.SetInteruptMusic(StageAudioAreaType.COMBAT);
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
        bool anyAreaNotCleared = _enemySpawns.Any(
                (enemySp) => !enemySp.IsEnemyInAreaClear
            );

        IsAllSpawnsCleared = !anyAreaNotCleared;

        if (!IsAllSpawnsCleared || !IsLockedDown) return;

        _UnlockAllDoors();
        AudioManager.Instance?.SetMusicByArea(AudioManager.Instance.CurrentAreaType);

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
