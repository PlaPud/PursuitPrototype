using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class KillPlane : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("PlayerCat")) return;
        PlayerHealth.Instance.DamagePlayer(PlayerHealth.Instance.CurrentHealth);
    }
}
