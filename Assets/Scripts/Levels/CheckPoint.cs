using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour, IDataPersistence
{
    [SerializeField] string checkpointId;

    public Action OnBeforeSave;
    public static Action OnSaveAny;

    private Collider2D _trigger;

    private void Awake()
    {
        _trigger = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("PlayerCat")) return;
        _trigger.enabled = false;
        OnBeforeSave?.Invoke();
        OnSaveAny?.Invoke();
        DataPersistenceManager.Instance.SaveGameData();
        AudioManager.Instance?.PlayOneShot(FMODEvents.Instance.SavedLevelComplete, transform.position);
    }

    public void LoadData(GameData data)
    {
        if (!data.SavedCheckpoints.ContainsKey(checkpointId)) return;
        _trigger.enabled = data.SavedCheckpoints[checkpointId];
    }

    public void SaveData(GameData data)
    {
        if (checkpointId == "") return;

        if (data.SavedCheckpoints.ContainsKey(checkpointId))
        {
            data.SavedCheckpoints[checkpointId] = _trigger.enabled;
            return;
        }

        data.SavedCheckpoints.Add(checkpointId, _trigger.enabled);
    }

    [ContextMenu("Generate GUID For This GameObject")]
    private void _GenerateItemGuid() => checkpointId = "CP-" + System.Guid.NewGuid().ToString();
}
