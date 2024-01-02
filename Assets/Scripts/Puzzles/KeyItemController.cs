using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

[assembly: InternalsVisibleTo("EditMode")]
[assembly: InternalsVisibleTo("PlayMode")]

[RequireComponent(typeof(Collider2D))]
public class KeyItemController : MonoBehaviour, IDataPersistence
{
    [field : SerializeField] public string guid { get; internal set; }
    [field: SerializeField] public Sprite displaySprite { get; internal set; }

    private bool _isCollected = false;
    private SpriteRenderer _itemSR;
    private Collider2D _itemTrigger;

    private void Awake()
    {
        _itemSR = GetComponent<SpriteRenderer>();
        _itemTrigger = GetComponent<Collider2D>();
    }

    private void Start()
    {
        _itemTrigger.enabled = false;
    }

    internal void Update()
    {
        CheckOnCollect();
    }

    private void CheckOnCollect() 
    {
        gameObject.SetActive(!_isCollected);
        if (gameObject.activeSelf) _itemTrigger.enabled = true;
    }

    [ContextMenu("Generate GUID for This Key Item")]
    private void _GenerateItemGuid() => guid = "KIT-" + System.Guid.NewGuid().ToString();

    internal void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != 8 || InventoryManager.Instance.IsFull) return;
        
        Debug.Log("Collected");
        InventoryManager.Instance.AddItem(this);
        _isCollected = true;
    }

    public void LoadData(GameData data)
    {
        if (!data.SavedCollectedItem.ContainsKey(guid)) return;

        _isCollected = data.SavedCollectedItem[guid];
    }

    public void SaveData(GameData data)
    {
        if (guid == "") return;

        if (data.SavedCollectedItem.ContainsKey(guid)) 
        {
            data.SavedCollectedItem[guid] = _isCollected;
            return;
        }

        data.SavedCollectedItem.Add(guid, _isCollected);
    }
}
