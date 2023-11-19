using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "KeyItemSO", menuName = "ScriptableObjects/KeyItemSO")]
public class KeyItemSO : ScriptableObject
{
    [SerializeField] public string itemGuid;
    [SerializeField] public string itemName;
    [SerializeField] public Sprite itemSprite;
    [SerializeField] public string targetGuid;

    [ContextMenu("Generate Item's GUID")]
    private void GenerateGuidForItem() 
    {
        itemGuid = System.Guid.NewGuid().ToString();
    }
}
