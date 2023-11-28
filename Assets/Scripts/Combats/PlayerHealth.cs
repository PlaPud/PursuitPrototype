using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;
    [field: SerializeField] public int maxHealth { get; private set; } = 5;
    [SerializeField] private int regenAmount = 1;
    [SerializeField] private float invincibleTime = 1f;
    [SerializeField] private float avoidToRegenTime = 10f;
    [SerializeField] private float regenDelayTime = 5f;

    public int CurrentHealth = 5;

    private GameObject _playerGO;
    private PlayerController _player;
    private float _regenTimer;
    private float _avoidDmgTimer;
    private float _invincibleTimer = 0;

    public Action OnDamageTaken;
    public Action OnHealthRegen;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        _playerGO = GameObject.FindGameObjectsWithTag("PlayerCat")[0];
        _player = _playerGO.GetComponent<PlayerController>();
    }

    void Start()
    {
        _regenTimer = regenDelayTime;
        _avoidDmgTimer = avoidToRegenTime;
        CurrentHealth = maxHealth;
    }

    void Update()
    {
        CheckInvincible();
        CheckHealthRegen();
        CheckAndHandleDeath();
    }

    private void CheckInvincible() 
    {
       _invincibleTimer -= _invincibleTimer > 0f ? Time.deltaTime : 0;
    }
    private void CheckHealthRegen() 
    {
        if (CurrentHealth >= maxHealth) return;

        if (_avoidDmgTimer > 0f) 
        {
            _avoidDmgTimer -= Time.deltaTime;
            return;
        }

        if (_regenTimer > 0f) 
        {
            _regenTimer -= Time.deltaTime;
            return;
        }

        _regenTimer = regenDelayTime;
        CurrentHealth += regenAmount;
        OnHealthRegen?.Invoke();
    }

    private void CheckAndHandleDeath() 
    {
        if (CurrentHealth > 0) return;
        _playerGO.gameObject.SetActive(false);
    }

    public void DamagePlayer(int damage) 
    {
        if (_invincibleTimer > 0f) return;

        CurrentHealth -= CurrentHealth - damage > 0 ? damage : CurrentHealth;
        _avoidDmgTimer = avoidToRegenTime;
        _invincibleTimer = invincibleTime;
        OnDamageTaken?.Invoke();
    }

}
