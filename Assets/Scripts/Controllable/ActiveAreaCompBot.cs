using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAreaCompBot : MonoBehaviour
{
    [SerializeField] private CompBotController compBot;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Transform spawnPoint;

    private Vector2 latestPos;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerCat")) 
        {
            //TODO: if first time enter THIS area set latestPos = spawnPoint
            //TODO: if not first time latestPos = compBot.transform.position
            CompBotManager.instance.IsInActiveArea = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerCat"))
        {
            latestPos = spawnPoint.position;
            CompBotManager.instance.IsInActiveArea = false;
        }
    }

}
