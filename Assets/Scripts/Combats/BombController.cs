using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : MonoBehaviour
{
    [SerializeField] LayerMask enemyMask;
    [SerializeField] private float explodeTime;
    [SerializeField] private float explodeRadius;

    private float _timer;

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

        foreach (RaycastHit2D hitEnemy in hitEnemies) 
        {
            EnemyController enemyScript = hitEnemy.transform.gameObject.GetComponent<EnemyController>();
            enemyScript.KillEnemy();
        }
        
        gameObject.SetActive(false);
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
        _timer = explodeTime;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, explodeRadius);
    }
}
