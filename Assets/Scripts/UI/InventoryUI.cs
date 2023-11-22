using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject emptySlotPrefab;
    void Start()
    {
        InventoryManager.instance.OnPickupItem += AddItemToDisplay;
        InventoryManager.instance.OnUseItem += RemoveItemFromSlot;
        RenderInventory();
    }

    void Update()
    {

    }

    private void RenderInventory() 
    {
        foreach (KeyItemController item in InventoryManager.instance.Inventory) 
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
