using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableByTagCollider : MonoBehaviour
{
    [SerializeField] private string tagToDisable;
    private Collider2D _edgeCD;

    private void Awake()
    {
        _edgeCD = GetComponent<Collider2D>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(tagToDisable)) return;
        _edgeCD.enabled = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag(tagToDisable)) return;
        _edgeCD.enabled = true;
    }

}
