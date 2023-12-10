using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("EditMode")]
[assembly: InternalsVisibleTo("PlayMode")]

public class ItemDetectorController : Interactable, IDataPersistence
{
    [field: SerializeField] public string guid { get; internal set; }
    [field: SerializeField] public KeyItemController reqItem { get; internal set; }

    [SerializeField] private Sprite LockSprite;
    [SerializeField] private Sprite UnlockedSprite;

    public delegate void UnlockHandler();
    public event UnlockHandler OnUnlock;

    private SpriteRenderer _detectorSR;

    public bool IsUnlocked = false;

    public bool IsCheckItem { get; internal set; } = false;

    void Start()
    {
        _detectorSR = GetComponent<SpriteRenderer>();    
    }

    protected override void Update()
    {
        if (IsUnlocked) {
            _UpdateSprite();
            return;
        }

        base.Update();

        TryCheckItemToUnlock(reqItem);
        _UpdateSprite();
    }

    public override void HandleInteract() => IsCheckItem = true;
    public void TryCheckItemToUnlock(KeyItemController item) 
    {
        if (!IsCheckItem) return;

        IsCheckItem = false;

        bool isExistToRemove = InventoryManager.Instance.TryRemoveItem(reqItem);
        IsUnlocked = isExistToRemove ? true : IsUnlocked;

        if (!IsUnlocked) return;
        OnUnlock?.Invoke();

    }

    [ContextMenu("Generate GUID for This Key Item")]
    private void _GenerateItemGuid() => guid = "ITD-" + System.Guid.NewGuid().ToString();

    private void _UpdateSprite() 
    {
        if (!_detectorSR) return;
        _detectorSR.sprite = IsUnlocked ? UnlockedSprite : LockSprite;
    }

    public void LoadData(GameData data)
    {
        if (!data.SavedItemDetectors.ContainsKey(guid)) return;
        IsUnlocked = data.SavedItemDetectors[guid];
    }

    public void SaveData(GameData data)
    {
        if (guid == "") return;

        if (data.SavedItemDetectors.ContainsKey(guid))
        {
            data.SavedItemDetectors[guid] = IsUnlocked;
            return;
        }

        data.SavedItemDetectors.Add(guid, IsUnlocked);
    }
}
