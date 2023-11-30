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
    [SerializeField] private int maxInField = 3;

    private bool _toShoot;
    private bool _isInCoolDown;

    private List<GameObject> _currentInField;


    private void Awake()
    {

    }
    private void Start()
    {
        _currentInField = new List<GameObject>();
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
        _ClearDisabledBomb();
    }

    private void HandleShoot() 
    {
        if (!_toShoot || _currentInField.Count == maxInField || _isInCoolDown) return;

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

        StartCoroutine(_EnableCoolDown());

        _currentInField.Add(bomb);
        _toShoot = false;
    }

    private void _ClearDisabledBomb() 
    {
        if (_currentInField.Count <= 0f) return;
        _currentInField = _currentInField.Where((go) => go.activeSelf).ToList();

    }

    private IEnumerator _EnableCoolDown() 
    {
        _isInCoolDown = true;
        yield return new WaitForSeconds(shootCoolDown);
        _isInCoolDown = false;
    }
}
