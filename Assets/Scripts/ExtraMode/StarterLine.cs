using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarterLine : DisableActiveTrigger
{
    
    void Start()
    {

    }

    void Update()
    {
        
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (!collision.CompareTag("PlayerCat")) return;

        AudioManager.Instance?.PlayOneShot(FMODEvents.Instance.ExtraModeStart, transform.position);
        AudioManager.Instance?.SetInteruptMusic(StageAudioAreaType.COMBAT);
    }
}
