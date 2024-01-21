using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageLoadBtn : MonoBehaviour
{
    [SerializeField] private GameMode forGameMode;
    private Button _btn;

    public enum GameMode 
    {
        Normal,
        Extra
    }

    private void Awake()
    {
        _btn = GetComponent<Button>();
    }

    void Start()
    {
        _btn.onClick.AddListener(AttachStagePref);
    }

    private void AttachStagePref() 
    {
        switch (forGameMode)
        {
            case GameMode.Normal:
                PlayerPrefs.SetString("StageToLoad", "Stage1");
                break;
            case GameMode.Extra:
                PlayerPrefs.SetString("StageToLoad", "Stage1Extra");
                break;
        }

    }

    void Update()
    {
        
    }
}
