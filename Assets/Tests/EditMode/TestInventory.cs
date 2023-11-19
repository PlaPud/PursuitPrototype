using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static UnityEditor.Experimental.GraphView.Port;

public class TestInventory
{
    [Test]
    [TestCase(0, 3)]
    [TestCase(3, 3)]
    [TestCase(1, 3)]
    [TestCase(0, 0)]
    public void AddItem_NotFull_ListAdded(int amountAdded, int capacity)
    {
        GameObject testInv = new GameObject();
        testInv.AddComponent<InventoryManager>();
        InventoryManager inv = testInv.GetComponent<InventoryManager>();

        inv.inventoryCapacity = capacity;

        List<GameObject> items = _GetMultipleMockKeyItems(amountAdded);
        _MoveItemsToInv(inv, items);

        Assert.AreEqual(amountAdded, inv.ItemCount);
    }

    [Test]
    [TestCase(10, 5)]
    [TestCase(5, 3)]
    [TestCase(2, 0)]
    public void AddItem_Full_ItemCountEqualToCap(int amountAdded, int capacity) 
    {
        GameObject testInv = new GameObject();
        testInv.AddComponent<InventoryManager>();
        InventoryManager inv = testInv.GetComponent<InventoryManager>();

        inv.inventoryCapacity = capacity;

        List<GameObject> items = _GetMultipleMockKeyItems(amountAdded);
        _MoveItemsToInv(inv, items);

        Assert.AreEqual(inv.ItemCount, capacity);
    }

    [Test]
    [TestCase(0)]
    [TestCase(3)]
    public void TryRemoveItem_EmptyInv_ReturnFalse_ItemCountZero(int capacity) 
    {
        GameObject testInv = new GameObject();
        testInv.AddComponent<InventoryManager>();
        InventoryManager inv = testInv.GetComponent<InventoryManager>();    

        inv.inventoryCapacity = capacity;

        bool isExistToRemove = inv.TryRemoveItem(_GeneratedRandomGUID);

        Assert.IsFalse(isExistToRemove);
        Assert.AreEqual(0, inv.ItemCount);
    }

    [Test]
    public void TryRemoveItem_GuidNotExist_ReturnFalse_ItemsRemain()
    {
        GameObject testInv = new GameObject();
        testInv.AddComponent<InventoryManager>();
        InventoryManager inv = testInv.GetComponent<InventoryManager>();

        inv.inventoryCapacity = 3;

        List<GameObject> items = _GetMultipleMockKeyItems(2);
        _MoveItemsToInv(inv, items);

        bool isExistToRemove = inv.TryRemoveItem(_GeneratedRandomGUID);
        
        Assert.IsFalse(isExistToRemove);    
        Assert.AreEqual(2, inv.ItemCount);
    }

    [Test]
    public void TryRemoveItem_ExistedGuid_ReturnTrue_ItemRemoved() 
    {
        GameObject testInv = new GameObject();
        testInv.AddComponent<InventoryManager>();
        InventoryManager inv = testInv.GetComponent<InventoryManager>();

        inv.inventoryCapacity = 3;

        List<GameObject> items = _GetMultipleMockKeyItems(2);
        KeyItemController topItem = items[items.Count - 1].GetComponent<KeyItemController>();
        KeyItemController firstItem = items[0].GetComponent<KeyItemController>();
        _MoveItemsToInv(inv, items);

        bool isExistToRemove = inv.TryRemoveItem(topItem.guid);
        Assert.IsTrue(isExistToRemove);
        Assert.AreEqual(1, inv.ItemCount);

        isExistToRemove = inv.TryRemoveItem(firstItem.guid);
        Assert.IsTrue(isExistToRemove);
        Assert.AreEqual(0, inv.ItemCount);
    }

    private List<GameObject> _GetMultipleMockKeyItems(int amount) 
    {
        List<GameObject> items = new();  

        for (int i = 0; i < amount; i++) 
        {
            string newGUID = _GeneratedRandomGUID;
            GameObject newItem = _GetMockItemObject(newGUID) ;
            items.Add(newItem);    
        }

        return items;
    }

    private GameObject _GetMockItemObject(string GUID) 
    {
        GameObject itemObject = new();
        itemObject.AddComponent<BoxCollider2D>();
        itemObject.AddComponent<KeyItemController>();
        itemObject.GetComponent<KeyItemController>().guid = GUID;

        return itemObject; 
    }

    private static void _MoveItemsToInv(InventoryManager inv, List<GameObject> items)
    {
        foreach (GameObject itemObj in items)
        {
            KeyItemController item = itemObj.GetComponent<KeyItemController>(); 
            inv.AddItem(item.guid, item);
        }
    }
    private string _GeneratedRandomGUID => System.Guid.NewGuid().ToString();

}
