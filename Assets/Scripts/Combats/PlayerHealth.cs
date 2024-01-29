using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("PlayMode")]
[assembly: InternalsVisibleTo("EditMode")]

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;
    [field: SerializeField] public int MaxHealth { get; internal set; } = 5;
    [SerializeField] internal int regenAmount = 1;
    [SerializeField] internal float invincibleTime = 1f;
    [SerializeField] internal float avoidToRegenTime = 10f;
    [SerializeField] internal float regenTime = 5f;

    public int CurrentHealth = 5;
    public int DeathCount { get; private set; } = 0;

    private GameObject _playerGO;
    private float _regenTimer;
    private float _avoidDmgTimer;
    private float _invincibleTimer = 0;

    public Action<int> OnDamageTaken;
    public Action<int> OnHealthRegen;
    public Action<float> OnCountDownRegen;
    public Action OnPlayerDied;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        _playerGO = GameObject.FindGameObjectsWithTag("PlayerCat")[0];
    }

    void Start()
    {
        _regenTimer = regenTime;
        _avoidDmgTimer = avoidToRegenTime;
        CurrentHealth = MaxHealth;
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
        if (CurrentHealth >= MaxHealth) return;

        if (_avoidDmgTimer > 0f)
        {
            _avoidDmgTimer -= Time.deltaTime;
            return;
        }

        if (_regenTimer > 0f)
        {
            _regenTimer -= Time.deltaTime;
            OnCountDownRegen?.Invoke(_regenTimer / regenTime);
            return;
        }

        OnHealthRegen?.Invoke(regenAmount);
        HealPlayer(regenAmount);
    }

    

    private void CheckAndHandleDeath() 
    {
        if (CurrentHealth > 0) return;
        _playerGO.gameObject.SetActive(false);
        OnPlayerDied?.Invoke();
    }

    public void HealPlayer(int regenAmount)
    {
        _regenTimer = regenTime;
        CurrentHealth += CurrentHealth + regenAmount > MaxHealth ? MaxHealth - CurrentHealth : regenAmount;
    }
     
    public void DamagePlayer(int damage)
    {
        if (_invincibleTimer > 0f) return;

        OnDamageTaken?.Invoke(damage);
        CurrentHealth -= CurrentHealth - damage > 0 ? damage : CurrentHealth;

        _avoidDmgTimer = avoidToRegenTime;
        _invincibleTimer = invincibleTime;
        _regenTimer = regenTime;
        OnCountDownRegen?.Invoke(_regenTimer / regenTime);
    }
}
