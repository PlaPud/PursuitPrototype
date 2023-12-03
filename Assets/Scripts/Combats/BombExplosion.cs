using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExplosion : MonoBehaviour
{
    private Vector3 SPRITE_POS_OFFSET = Vector3.down * 1.5f;

    [SerializeField] private GameObject explodeEffectPrefab;

    private BombController _bomb;

    void Start()
    {
        _bomb = GetComponent<BombController>(); 
        _bomb.OnExplosion += SetExplosionEffect;
    }

    void Update()
    {
        
    }

    private void OnEnable()
    {
        if (!_bomb) return;
        _bomb.OnExplosion += SetExplosionEffect;
    }

    private void OnDisable()
    {
        if (!_bomb) return;
        _bomb.OnExplosion -= SetExplosionEffect;
    }

    private void SetExplosionEffect() 
    {
        Instantiate(
                explodeEffectPrefab, 
                position: _bomb.transform.position + SPRITE_POS_OFFSET,
                rotation: Quaternion.identity
            );
    }
}
