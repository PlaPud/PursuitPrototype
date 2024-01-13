using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class AmbienceChangeTrigger : MonoBehaviour
{
    [Header("Area Type")]
    [SerializeField] private StageAudioAreaType areaType;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("PlayerCat")) return;

        AudioManager.Instance.SetAmbienceByArea(areaType);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }
}
