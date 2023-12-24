using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyLockedEntry : Entry
{
    [SerializeField] ItemDetectorController detector;

    Collider2D _entryCD;

    private void Awake()
    {
        _entryCD = GetComponent<Collider2D>();
    }

    protected override void Update()
    {
        if (!detector.IsUnlocked) 
        {
            _entryCD.enabled = false;
            return;
        }

        if (!_entryCD.enabled) _entryCD.enabled = true;

        base.Update();
    }
}
