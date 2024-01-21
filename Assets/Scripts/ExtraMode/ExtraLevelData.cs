using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraLevelData : MonoBehaviour
{
    public static ExtraLevelData Instance { get; private set; }

    [field: SerializeField] public List<Transform> levelSpawn { get; private set; } = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("ExtraLevelData already exists!");
            return;
        }

        Instance = this;

        //Debug.Log(PlayerPrefs.GetInt("ExtraModeLevel", 0));

    }
}
