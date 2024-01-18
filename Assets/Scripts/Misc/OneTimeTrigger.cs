using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneTimeTrigger : MonoBehaviour
{
    [SerializeField] private string tagToDisable;

    private Collider2D _triggerCD;

    public Action OnTriggerDisabled;

    private void Awake()
    {
        _triggerCD = GetComponent<Collider2D>();
        _triggerCD.enabled = true;
        _triggerCD.isTrigger = true;
    }

    virtual protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(tagToDisable)) return;
        _triggerCD.enabled = false;
        OnTriggerDisabled?.Invoke();    
    }
   
}
