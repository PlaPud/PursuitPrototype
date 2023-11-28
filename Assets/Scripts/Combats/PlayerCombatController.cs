using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    [SerializeField] private AimingController _aiming;

    [SerializeField] private KeyCode shootKey = KeyCode.Mouse0;
    [SerializeField] private float shootDelay;
    [SerializeField] private float firingForce = 1;
    [SerializeField] private float tillExplodeTime;
    [SerializeField] private int maxInField = 3;

    private bool _toShoot;
    private int _currentInField;


    private void Awake()
    {

    }
    private void Start()
    {
        
    }

    private void Update()
    {
        GetInputShoot();
    }

    private void GetInputShoot() 
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) 
        {
            _toShoot = true;
        }
    }

    private void FixedUpdate()
    {
        HandleShoot();
    }

    private void HandleShoot() 
    {
        if (!_toShoot || _currentInField == maxInField) return;

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

        bombRB.AddForce(
                force: _aiming.AimingCircleDirection * firingForce,
                mode: ForceMode2D.Impulse
            );

        _currentInField += 1;
        _toShoot = false;
    }
}
