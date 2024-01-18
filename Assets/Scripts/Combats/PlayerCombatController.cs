using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    [SerializeField] private CombatAiming _aiming;

    [SerializeField] private KeyCode shootKey = KeyCode.Mouse0;
    [SerializeField] private float shootCoolDown;
    [SerializeField] private float firingForce = 1;
    [field: SerializeField] public int MaxInField { get; private set; } = 3;

    private bool _toShoot;
    private bool _isInCoolDown;
    private bool _isInCombat;

    public List<GameObject> CurrentInField { get; private set; }

    public Action OnPlayerShootBomb;
    public Action OnCheckReload;

    private Rigidbody2D _playerRB;

    private void Awake()
    {
        _playerRB = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        CurrentInField = new List<GameObject>();
        EnemyAreaController.OnCombatStart += EnableCombat;
        EnemyAreaController.OnCombatEnd += DisableCombat;
    }

    private void Update()
    {
        if (!_isInCombat) return;
        GetInputShoot();
    }

    private void GetInputShoot() 
    {
        if (ControllingManager.Instance.CurrentControl != ControllingManager.Control.PlayerMain) return;

        if (Input.GetKeyDown(shootKey) && CurrentInField.Count < MaxInField) 
        {
            _toShoot = true;
        }
    }

    private void FixedUpdate()
    {
        HandleShoot();
        _ClearDisabledBomb();
    }

    private void HandleShoot() 
    {
        if (!_isInCombat || !_toShoot || CurrentInField.Count == MaxInField || _isInCoolDown) return;

        GameObject bomb = BombPoolingManager.Instance.GetBombFromPool();

        if (!bomb) 
        {
            Debug.LogError("Cannot Find Object From BombPool.");
            _toShoot = false;
            return;
        }

        bomb.transform.position = transform.position;
        Rigidbody2D bombRB = bomb.GetComponent<Rigidbody2D>();
        bomb.SetActive(true);

        OnPlayerShootBomb?.Invoke();
       
        StartCoroutine(_ShootBomb(bombRB));

        StartCoroutine(_EnableCoolDown());

        CurrentInField.Add(bomb);
        _toShoot = false;
    }

    private IEnumerator _ShootBomb(Rigidbody2D bombRB) 
    {
        
        yield return new WaitForSeconds(0.05f);

        bombRB.velocity = Vector2.zero;
        bombRB.velocity = _aiming.AimingCircleDirection.normalized * firingForce + (Vector3) _playerRB.velocity * 0.5f;

        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerShoot, transform.position);
    }

    private void _ClearDisabledBomb() 
    {
        if (CurrentInField.Count <= 0f) return;
        CurrentInField = CurrentInField.Where((go) => go.activeSelf).ToList();
        OnCheckReload?.Invoke(); 
    }

    private void EnableCombat() 
    {
        _isInCombat = true;
    }

    private void DisableCombat() 
    {
        _isInCombat = false;
        _toShoot = false ;
        _isInCoolDown = false;
    }

    private IEnumerator _EnableCoolDown() 
    {
        _isInCoolDown = true;
        yield return new WaitForSeconds(shootCoolDown);
        _isInCoolDown = false;
    }
}
