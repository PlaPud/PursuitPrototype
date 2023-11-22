using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("EditMode")]
[assembly: InternalsVisibleTo("PlayMode")]

public class ItemDetectorController : Interactable
{
    [field: SerializeField] public string guid { get; internal set; }
    [field: SerializeField] public KeyItemController reqItem { get; internal set; }

    [SerializeField] private Sprite LockSprite;
    [SerializeField] private Sprite UnlockedSprite;

    public delegate void UnlockHandler();
    public event UnlockHandler OnUnlock;

    private SpriteRenderer _detectorSR;

    public bool IsOpen { get; private set; } = false;

    public bool IsCheckItem { get; internal set; } = false;

    void Start()
    {
        _detectorSR = GetComponent<SpriteRenderer>();    
    }

    protected override void Update()
    {
        base.Update();

        TryCheckItemToUnlock(reqItem);
        _UpdateSprite();
    }

    public override void HandleInteract() => IsCheckItem = true;
    public void TryCheckItemToUnlock(KeyItemController item) 
    {
        if (!IsCheckItem) return;

        IsCheckItem = false;

        bool isExistToRemove = InventoryManager.instance.TryRemoveItem(reqItem);
        IsOpen = isExistToRemove ? true : IsOpen;

        if (!IsOpen) return;
        OnUnlock?.Invoke();

    }

    [ContextMenu("Generate GUID for This Key Item")]
    private void _GenerateItemGuid() => guid = System.Guid.NewGuid().ToString();

    private void _UpdateSprite() 
    {
        _detectorSR.sprite = IsOpen ? UnlockedSprite : LockSprite;
    }
}
