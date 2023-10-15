using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAreaCompBot : MonoBehaviour
{
    [SerializeField] LayerMask playerLayer;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerCat")) 
        {
            CompBotManager.instance.IsInActiveArea = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerCat"))
        {
            CompBotManager.instance.IsInActiveArea = false;
        }
    }

}
