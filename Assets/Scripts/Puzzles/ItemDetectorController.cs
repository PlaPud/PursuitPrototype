using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDetectorController : Interactable
{
    [field: SerializeField] public string guid { get; private set; }
    [field: SerializeField] public string reqKeyGuid { get; internal set; }
    
    [SerializeField] private Sprite LockSprite;
    [SerializeField] private Sprite UnlockedSprite;

    private SpriteRenderer _detectorSR;

    public bool IsOpen { get; private set; } = false;

    bool _IsCheckItem = false;

    void Start()
    {
        _detectorSR = GetComponent<SpriteRenderer>();    
    }

    protected override void Update()
    {
        base.Update();

        TryCheckItemToUnlock(reqKeyGuid);
        _UpdateSprite();
    }

    public override void HandleInteract() => _IsCheckItem = true;
    public void TryCheckItemToUnlock(string expectedGuid) 
    {
        if (!_IsCheckItem) return;

        _IsCheckItem = false;

        bool isExistToRemove = InventoryManager.instance.TryRemoveItem(reqKeyGuid);
        IsOpen = isExistToRemove;
    }

    [ContextMenu("Generate GUID for This Key Item")]
    private void _GenerateItemGuid() => guid = System.Guid.NewGuid().ToString();

    private void _UpdateSprite() 
    {
        _detectorSR.sprite = IsOpen ? UnlockedSprite : LockSprite;
    }
}
