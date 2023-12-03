using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompleteTrigger : MonoBehaviour
{

    public bool IsPlayerReached = false;

    private Collider2D _triggerCD;

    private void Start()
    {
        _triggerCD = GetComponent<Collider2D>();    
    }

    private void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("PlayerCat")) return;
        IsPlayerReached = true;
        _triggerCD.enabled = false;
    }
}
