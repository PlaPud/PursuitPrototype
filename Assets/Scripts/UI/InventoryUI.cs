using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject emptySlotPrefab;
    void Start()
    {
        InventoryManager.Instance.OnPickupItem += AddItemToDisplay;
        InventoryManager.Instance.OnUseItem += RemoveItemFromSlot;
        InventoryManager.Instance.OnLoadData += RenderInventory;
        RenderInventory();
    }

    void Update()
    {

    }

    private void RenderInventory() 
    {
        foreach (KeyItemController item in InventoryManager.Instance.Inventory) 
        {
            AddItemToDisplay(item);
        }
    }

    private void AddItemToDisplay(KeyItemController item)
    {
        if (!item) return;
        GameObject slot = Instantiate(emptySlotPrefab);
        slot.transform.Find("ItemImage").GetComponent<Image>().sprite = item.displaySprite;
        slot.name = item.guid;
        slot.transform.SetParent(transform);
    }

    private void RemoveItemFromSlot(KeyItemController item)
    {
        if (!item) return;
        Transform removeItem = transform.Find(item.guid);
        Destroy(removeItem?.gameObject);
    }

}
