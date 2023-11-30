using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;
    [field: SerializeField] public int MaxHealth { get; private set; } = 5;
    [SerializeField] private int regenAmount = 1;
    [SerializeField] private float invincibleTime = 1f;
    [SerializeField] private float avoidToRegenTime = 10f;
    [SerializeField] private float regenTime = 5f;

    public int CurrentHealth = 5;

    private GameObject _playerGO;
    private PlayerController _player;
    private float _regenTimer;
    private float _avoidDmgTimer;
    private float _invincibleTimer = 0;

    public Action<int> OnDamageTaken;
    public Action<int> OnHealthRegen;
    public Action<float> OnCountDownRegen;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        _playerGO = GameObject.FindGameObjectsWithTag("PlayerCat")[0];
        _player = _playerGO.GetComponent<PlayerController>();
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
        _regenTimer = regenTime;
        CurrentHealth += regenAmount;
    }

    private void CheckAndHandleDeath() 
    {
        if (CurrentHealth > 0) return;
        _playerGO.gameObject.SetActive(false);
    }

    public void DamagePlayer(int damage) 
    {
        if (_invincibleTimer > 0f) return;

        OnDamageTaken?.Invoke(damage);
        CurrentHealth -= CurrentHealth - damage > 0 ? damage : CurrentHealth;
        _avoidDmgTimer = avoidToRegenTime;
        _invincibleTimer = invincibleTime;
        _regenTimer = regenTime;
    }

}
