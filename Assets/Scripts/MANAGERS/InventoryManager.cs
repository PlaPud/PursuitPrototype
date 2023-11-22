using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    public int inventoryCapacity = 3; 

    public List<KeyItemController> Inventory { get; private set; } = new List<KeyItemController>();

    public bool IsFull => inventoryCapacity == Inventory.Count;
    public bool IsEmpty => Inventory.Count <= 0;

    public int ItemCount => Inventory.Count;

    public delegate void PickUpItemHandler(KeyItemController item);
    public event PickUpItemHandler OnPickupItem;

    public delegate void UseItemHandle(KeyItemController item);
    public event UseItemHandle OnUseItem;   

    private void Awake() => instance = this;

    public void AddItem(KeyItemController itemCtrl)
    {
        if (IsFull) return;
        Inventory.Add(itemCtrl);

        OnPickupItem?.Invoke(itemCtrl);
    }

    public bool TryRemoveItem(KeyItemController itemCtrl)
    {
        if (IsEmpty) return false;
        bool isExistToRemove = Inventory.Remove(itemCtrl);

        OnUseItem?.Invoke(itemCtrl);
        
        return isExistToRemove;

    }
}
