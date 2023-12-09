using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour, IDataPersistence
{
    public static InventoryManager Instance;
    public int inventoryCapacity = 3; 

    public List<KeyItemController> Inventory { get; private set; } = new List<KeyItemController>();
    public bool IsFull => inventoryCapacity == Inventory.Count;
    public bool IsEmpty => Inventory.Count <= 0;

    private List<KeyItemController> _allKeyItem;

    public int ItemCount => Inventory.Count;

    public delegate void PickUpItemHandler(KeyItemController item);
    public event PickUpItemHandler OnPickupItem;

    public delegate void UseItemHandle(KeyItemController item);
    public event UseItemHandle OnUseItem;

    public Action OnLoadData;

    private void Awake() 
    {
        Instance = this;
        _allKeyItem = FindObjectsOfType<KeyItemController>().ToList();
    }

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

    public void LoadData(GameData data)
    {
        data.InventoryItems.ForEach((guid) => {
            KeyItemController item = _FindItemByGuid(guid);
            if (item == null) return;
            Inventory.Add(item);
        });
        OnLoadData?.Invoke();
    }

    public void SaveData(GameData data)
    {
        data.InventoryItems.Clear();
        Inventory.ForEach((item) => {
            string id = item.guid;
            if (id != "")
            {
                data.InventoryItems.Add(id);                    
            }
        });
    }
    private KeyItemController _FindItemByGuid(string guid) 
    {
        KeyItemController target =
            _allKeyItem.Find((item) => item.guid == guid);
        return target;
    }
}
