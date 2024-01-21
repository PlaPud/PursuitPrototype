using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraLevelSelect : MonoBehaviour
{
    public static ExtraLevelSelect Instance { get; private set; }

    public int SelectedLevel;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("ExtraLevelSelect already exists!");
            return;
        }

        Instance = this;
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

}
