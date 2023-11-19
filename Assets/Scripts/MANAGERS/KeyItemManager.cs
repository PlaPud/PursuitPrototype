using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyItemManager : MonoBehaviour
{

    public static KeyItemManager instance;

    [SerializeField] List<KeyItemController> keyItems = new List<KeyItemController>();

    private void Awake() => instance = this;

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public KeyItemController findKeyItem(string guid) 
    { 
        foreach (KeyItemController item in keyItems) 
        {
            if (guid == item.guid) return item;
        }
        return null;
    }
}
