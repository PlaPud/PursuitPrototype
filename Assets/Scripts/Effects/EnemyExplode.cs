using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyExplode : MonoBehaviour
{
    private EnemyController _enemy;

    private void Awake()
    {
        _enemy = GetComponent<EnemyController>();
    }
 
    void Start()
    {
        _enemy.OnDefeated += SetExplosionEffect;
    }

    private void SetExplosionEffect()
    {
        GameObject explodeEff = EnemyExplodeEffManager.Instance.GetExplodeEffectFromPool();

        if (!explodeEff)
        {
            Debug.LogError("EnemyExplodeEffManager.Instance.GetExplodeEffectFromPool() returns null");
            return;
        }

        explodeEff.transform.position = transform.position;
        explodeEff.SetActive(true);
        AudioManager.Instance?.PlayOneShot(FMODEvents.Instance.EnemyDestroyed, transform.position);
    }
}
