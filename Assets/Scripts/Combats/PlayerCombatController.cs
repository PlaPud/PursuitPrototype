using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    [SerializeField] private AimingController _aiming;

    [SerializeField] private KeyCode shootKey = KeyCode.Mouse0;
    [SerializeField] private float shootCoolDown;
    [SerializeField] private float firingForce = 1;
    [field: SerializeField] public int MaxInField { get; private set; } = 3;

    private bool _toShoot;
    private bool _isInCoolDown;

    public List<GameObject> CurrentInField { get; private set; }

    public Action OnPlayerShootBomb;
    public Action OnCheckReload;

    private void Awake()
    {

    }
    private void Start()
    {
        CurrentInField = new List<GameObject>();
    }

    private void Update()
    {
        GetInputShoot();
    }

    private void GetInputShoot() 
    {
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
        if (!_toShoot || CurrentInField.Count == MaxInField || _isInCoolDown) return;

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

        bombRB.AddForce(
                force: _aiming.AimingCircleDirection * firingForce,
                mode: ForceMode2D.Impulse
            );

        StartCoroutine(_EnableCoolDown());

        CurrentInField.Add(bomb);
        _toShoot = false;
    }

    private void _ClearDisabledBomb() 
    {
        if (CurrentInField.Count <= 0f) return;
        CurrentInField = CurrentInField.Where((go) => go.activeSelf).ToList();
        OnCheckReload?.Invoke(); 
    }

    private IEnumerator _EnableCoolDown() 
    {
        _isInCoolDown = true;
        yield return new WaitForSeconds(shootCoolDown);
        _isInCoolDown = false;
    }
}
