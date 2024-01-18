using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int DeathCount;
    public Vector3Serialize PlayerPos;
    public Dictionary<string, Vector3Serialize> SavedMoveablePos;
    public Dictionary<string, bool> SavedLevelComplete;
    public Dictionary<string, bool> SavedCollectedItem;
    public Dictionary<string, bool> SavedItemDetectors;
    public Dictionary<string, bool> SavedCheckpoints;
    public List<string> InventoryItems;

    public GameData() 
    {
        DeathCount = 0;
        PlayerPos = new Vector3Serialize(Vector3.zero);
        SavedMoveablePos = new Dictionary<string, Vector3Serialize>();
        SavedLevelComplete = new Dictionary<string, bool>();
        SavedCollectedItem = new Dictionary<string, bool>();
        SavedItemDetectors = new Dictionary<string, bool>();
        SavedCheckpoints = new Dictionary<string, bool>();
        InventoryItems = new List<string>();
    }
}
