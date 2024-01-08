using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestPlayDetector
{
    [Test]
    [TestCase(3)]
    [TestCase(1)]
    public void TryCheckItemToUnlock_ItemExist_IsUnlocked(int totalItems)
    {
        GameObject testInv = _GetInventory(5);
        InventoryManager inv = testInv.GetComponent<InventoryManager>();
        List<GameObject> items = _GetMultipleMockKeyItems(totalItems);

        KeyItemController targetItem = items[0].GetComponent<KeyItemController>();
        GameObject detector = _GetItemDetector(_GeneratedRandomGUID, targetItem);

        _MoveItemsToInv(inv, items);
        Assert.AreEqual(inv.ItemCount, totalItems);

        ItemDetectorController detectorScript = detector.GetComponent<ItemDetectorController>();

        detectorScript.IsCheckItem = true;
        detectorScript.TryCheckItemToUnlock(targetItem);

        Assert.IsTrue(detectorScript.IsUnlocked);
        Assert.AreEqual(totalItems - 1, inv.ItemCount);

        Object.Destroy(testInv);
        items.ForEach(item => Object.Destroy(item));    
    }

    [Test]
    [TestCase(3)]
    [TestCase(1)]
    [TestCase(0)]
    public void TryCheckItemToUnlock_NoItemMatch_NotUnlocked(int totalItems)
    {
        GameObject testInv = _GetInventory(5);
        InventoryManager inv = testInv.GetComponent<InventoryManager>();
        List<GameObject> items = _GetMultipleMockKeyItems(totalItems);

        GameObject targetObject = _GetMockItemObject(_GeneratedRandomGUID);
        KeyItemController targetItem = targetObject.GetComponent<KeyItemController>();
        GameObject detector = _GetItemDetector(_GeneratedRandomGUID, targetItem);

        _MoveItemsToInv(inv, items);
        Assert.AreEqual(inv.ItemCount, totalItems);

        ItemDetectorController detectorScript = detector.GetComponent<ItemDetectorController>();

        detectorScript.IsCheckItem = true;
        detectorScript.TryCheckItemToUnlock(targetItem);

        Assert.IsFalse(detectorScript.IsUnlocked);
        Assert.AreEqual(totalItems, inv.ItemCount);

        // clean up all GameObjects
        Object.Destroy(testInv);
        items.ForEach(item => Object.Destroy(item));
        Object.Destroy(targetObject);
        Object.Destroy(detector);
    }


    private GameObject _GetItemDetector(string GUID, KeyItemController reqItem)
    {
        GameObject detectorObject = new();
        detectorObject.AddComponent<BoxCollider2D>();
        detectorObject.AddComponent<SpriteRenderer>();

        detectorObject.AddComponent<ItemDetectorController>();

        detectorObject.GetComponent<ItemDetectorController>().guid = GUID;
        detectorObject.GetComponent<ItemDetectorController>().reqItem = reqItem;

        return detectorObject;
    }

    private GameObject _GetInventory(int capacity)
    {
        GameObject testInv = new GameObject();
        testInv.AddComponent<InventoryManager>();
        InventoryManager inv = testInv.GetComponent<InventoryManager>();
        inv.inventoryCapacity = 3;
        return testInv;
    }

    private List<GameObject> _GetMultipleMockKeyItems(int amount)
    {
        List<GameObject> items = new();

        for (int i = 0; i < amount; i++)
        {
            string newGUID = _GeneratedRandomGUID;
            GameObject newItem = _GetMockItemObject(newGUID);
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
            inv.AddItem(item);
        }
    }

    private string _GeneratedRandomGUID => System.Guid.NewGuid().ToString();

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        foreach (GameObject obj in Object.FindObjectsOfType<GameObject>())
        {
            Object.Destroy(obj);
        }
        yield return null;
    }

}
