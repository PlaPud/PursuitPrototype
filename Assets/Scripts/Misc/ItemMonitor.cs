using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMonitor : MonoBehaviour
{
    [SerializeField] private ItemDetectorController _detector;
    [SerializeField] private Sprite _monitorLocked;
    [SerializeField] private Sprite _monitorUnlocked;
    [SerializeField] private Sprite _monitorDeny;

    private SpriteRenderer _monitorSR;

    private bool _isFlashing = false;   

    private void Awake()
    {
        _monitorSR = GetComponent<SpriteRenderer>();
        _detector.OnDenyItem += FlashDenySprite;
    }

    void Start()
    {
        
    }

    void Update()
    {
        DisplaySprite();
    }

    private void DisplaySprite() 
    {
        if (_isFlashing) return;
        _monitorSR.sprite = _detector.IsUnlocked ?
            _monitorUnlocked : _monitorLocked;
    }

    private void FlashDenySprite()
    {
        StartCoroutine(_FlashDenySprite());
    }

    private IEnumerator _FlashDenySprite()
    {
        _isFlashing = true;
        _monitorSR.sprite = _monitorDeny;
        yield return new WaitForSeconds(0.1f);
        _monitorSR.sprite = _monitorLocked;
        yield return new WaitForSeconds(0.1f);
        _monitorSR.sprite = _monitorDeny;
        yield return new WaitForSeconds(0.1f);
        _monitorSR.sprite = _monitorLocked;
        _isFlashing = false;
    }
}
