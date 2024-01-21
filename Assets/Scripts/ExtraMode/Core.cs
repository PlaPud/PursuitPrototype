using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("PlayerCat") || ExtraModeManager.Instance.IsTimeOver) return;

        ExtraModeManager.Instance?.CollectCore();

        AudioManager.Instance?.PlayOneShot(FMODEvents.Instance.CoreCollect, transform.position);

        gameObject.SetActive(false);
    }
}
