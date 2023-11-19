using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestPlayKeyItem
{
    [Test]
    public void EnterTrigger_PlayerLayer_PickUpItem()
    {
        GameObject testItem = GetTestItem();
        GameObject mockPLayer = GetMockPlayer();
        GameObject inventory = GetInventory();

        KeyItemController script = testItem.GetComponent<KeyItemController>();
        BoxCollider2D playerCD = mockPLayer.GetComponent<BoxCollider2D>();

        script.OnTriggerEnter2D(playerCD);
        script.Update();

        Assert.IsFalse(testItem.activeSelf);
        
    }

    [Test]
    public void EnterTrigger_OtherLayer_NotPickup()
    {
        GameObject testItem = GetTestItem();
        GameObject mockPLayer = GetMockPlayer();
        GameObject inventory = GetInventory();

        KeyItemController script = testItem.GetComponent<KeyItemController>();
        BoxCollider2D playerCD = mockPLayer.GetComponent<BoxCollider2D>();

        mockPLayer.layer = 1;

        script.OnTriggerEnter2D(playerCD);
        script.Update();

        Assert.IsTrue(testItem.activeSelf);
    }

    private GameObject GetInventory() 
    {
        GameObject testInventory = new GameObject();
        testInventory.AddComponent<InventoryManager>();
        return testInventory;   
    }

    private GameObject GetTestItem()
    {
        GameObject testItem = new GameObject();
        testItem.AddComponent<BoxCollider2D>();
        testItem.GetComponent<BoxCollider2D>().isTrigger = true;
        testItem.AddComponent<KeyItemController>();
        return testItem;
    }

    private GameObject GetMockPlayer()
    {
        GameObject mockPlayer = new GameObject();
        mockPlayer.AddComponent<BoxCollider2D>();
        mockPlayer.layer = 8;
        return mockPlayer;
    }
}
