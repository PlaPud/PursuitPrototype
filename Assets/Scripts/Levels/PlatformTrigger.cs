using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    [SerializeField] LayerMask playerMask;

    private Collider2D _platformCD;

    private void Awake()
    {
        _platformCD = transform.parent.GetComponent<Collider2D>();
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer != 8) return;
        transform.parent.GetComponent<Collider2D>().enabled = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer != 8) return;
        _platformCD.enabled = true;
    }


}
