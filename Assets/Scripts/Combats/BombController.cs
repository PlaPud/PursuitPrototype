using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : MonoBehaviour
{
    [SerializeField] LayerMask enemyMask;
    [SerializeField] private float explodeTime;
    [SerializeField] private float explodeRadius;

    private float _timer;
    private bool _isExploding;
    private SpriteRenderer _bombSR;

    public Action OnExplosion;

    private void Awake()
    {
        _bombSR = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        _Initialize();
    }

    void Update()
    {
        HandleExplosion();
    }

    private void HandleExplosion() 
    {
        _timer -= _timer - Time.deltaTime > 0f ? Time.deltaTime : _timer;
        if (_timer > 0f) return;
        RaycastHit2D[] hitEnemies = Physics2D.CircleCastAll(
                origin: transform.position,
                radius: explodeRadius,
                direction: Vector2.zero,
                distance: 0f,
                layerMask: enemyMask
            );

        if (_isExploding) return;

        foreach (RaycastHit2D hitEnemy in hitEnemies) 
        {
            EnemyController enemyScript = hitEnemy.transform.gameObject.GetComponent<EnemyController>();
            enemyScript.KillEnemy();
        }

        StartCoroutine(_WaitForExplosion());
    }

    private IEnumerator _WaitForExplosion() 
    {
        _isExploding = true;
        _bombSR.enabled = false;
        OnExplosion?.Invoke();
        yield return new WaitForSeconds(0.25f);
        gameObject.SetActive(false);
        _isExploding = false;
    }

    private void OnEnable() 
    {
        _Initialize();
    }

    private void OnDisable()
    {
       
    }

    private void _Initialize() 
    {
        _bombSR.enabled = true;
        _timer = explodeTime;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, explodeRadius);
    }
}
