using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableActiveTrigger : MonoBehaviour
{
    [SerializeField] private string tagToDisable;

    virtual protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(tagToDisable)) return;
        gameObject.SetActive(false);
    }
}
