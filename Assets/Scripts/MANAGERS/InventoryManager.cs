using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    public int inventoryCapacity = 3; 

    private List<string> _inventory = new List<string>();

    public bool IsFull => inventoryCapacity == _inventory.Count;
    public bool IsEmpty => _inventory.Count <= 0;

    public int ItemCount => _inventory.Count;  

    private void Awake() => instance = this;

    public void AddItem(string guid, KeyItemController itemCtrl)
    {
        if (itemCtrl.guid != guid || IsFull) return;
        _inventory.Add(guid);
    }

    public bool TryRemoveItem(string guid)
    {
        if (IsEmpty) return false;
        bool isExistToRemove = _inventory.Remove(guid);
        return isExistToRemove;
    }
}
