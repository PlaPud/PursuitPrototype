using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

[assembly: InternalsVisibleTo("EditMode")]
[assembly: InternalsVisibleTo("PlayMode")]

[RequireComponent(typeof(Collider2D))]
public class KeyItemController : MonoBehaviour
{
    [field : SerializeField] public string guid { get; internal set; }
    [field: SerializeField] public Sprite displaySprite { get; internal set; }

    private bool _isCollected = false;

    private void Awake()
    {

    }

    private void Start()
    {

    }

    internal void Update()
    {
        gameObject.SetActive(!_isCollected);
    }

    [ContextMenu("Generate GUID for This Key Item")]
    private void _GenerateItemGuid() => guid = System.Guid.NewGuid().ToString();

    internal void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != 8 || InventoryManager.instance.IsFull) return;
        
        if (!gameObject.activeSelf) return;

        Debug.Log("Collected");
        InventoryManager.instance.AddItem(this);
        _isCollected = true;
    }


}
