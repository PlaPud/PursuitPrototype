using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMonitor : MonoBehaviour
{
    [SerializeField] private ItemDetectorController _detector;
    [SerializeField] private Sprite _monitorLocked;
    [SerializeField] private Sprite _monitorUnlocked;

    private SpriteRenderer _monitorSR;

    private void Awake()
    {
        _monitorSR = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        _monitorSR.sprite = _detector.IsUnlocked ? 
            _monitorUnlocked : _monitorLocked;
    }
}
