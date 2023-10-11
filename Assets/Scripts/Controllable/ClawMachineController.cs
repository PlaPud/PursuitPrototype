using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClawMachineController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float detectionRadius;
    [SerializeField] private GameObject clawBody;
    [SerializeField] private GameObject clawTop;
    [SerializeField] private Transform holdingPoint;
    [SerializeField] private Transform wirePoint;
    [SerializeField] private LayerMask movableLayer;
    [SerializeField] private LayerMask groundLayer;

    private LineRenderer _wireLR;

    private float _moveX;
    private float _moveY;
    private float _objectGravityScale;
    private bool _isHolding;

    private GameObject _holdingObject;

    private bool _toToggleHold;

    private Rigidbody2D _clawBodyRB;
    private Rigidbody2D _clawTopRB;
    private Animator _clawBodyAnimator;
    private Collider2D _clawBodyCD;
    private Collider2D[] _allEnteredMovables;
    private RaycastHit2D _hitCeiling;

    string _currentAnimationState = CLAW_EMPTY;

    const string CLAW_EMPTY = "ClawEmpty";
    const string CLAW_HOLD = "ClawHold";

    private void Awake()
    {
        _wireLR = clawBody.GetComponent<LineRenderer>();
        _clawBodyRB = clawBody.GetComponent<Rigidbody2D>();
        _clawBodyAnimator = clawBody.GetComponent<Animator>();
        _clawTopRB = clawTop.GetComponent<Rigidbody2D>();
    }

    void Start()
    {
     
    }

    void Update()
    {
        RayCheck();
        OnMove();
        OnToggleHold();
        SetWireLength();
    }

    private void FixedUpdate()
    {
        HandleIdle();
        HandleMove();
        HandleHolding();
    }

    private void RayCheck() 
    {
        _allEnteredMovables = Physics2D.OverlapBoxAll(
                point: holdingPoint.position,
                size: Vector2.one * detectionRadius,
                angle: 0,
                layerMask: movableLayer
            );

        _hitCeiling = Physics2D.Raycast(
                        transform.position,
                        Vector2.up,
                        Mathf.Infinity,
                        groundLayer
                    );
    }

    private void OnMove() 
    {
        _moveX = Input.GetAxisRaw("Horizontal");
        _moveY = Input.GetAxisRaw("Vertical");
    }

    private void OnToggleHold() 
    {
        bool _isToggleHold = Input.GetKeyDown(KeyCode.E);
        if (_isToggleHold) _toToggleHold = true;
    }

    private void HandleMove() 
    {
        //if (_holdingObject && _holdingObject.GetComponent<Collider2D>()) 

        Vector2 appliedVelocity = new Vector2(
                    _moveX * moveSpeed,
                    _moveY * moveSpeed
                );
        
        _clawTopRB.velocity = Vector2.right * appliedVelocity.x;
        _clawBodyRB.velocity = Vector2.up * appliedVelocity.y;
        clawBody.transform.position =
            new Vector3(
                    clawTop.transform.position.x,
                    clawBody.transform.position.y,
                    0f
                );

    }

    private void HandleIdle() 
    {
        
    }

    private void HandleHolding() 
    {
        if (_toToggleHold && _allEnteredMovables.Length > 0 && !_holdingObject)
        {
            Collider2D closestMovable = _NearestMovable();
            _toToggleHold = false;
            _isHolding = true;
            _GrabMovable(closestMovable);
            ChangeAnimationState(CLAW_HOLD);
            return;
        }

        if (_toToggleHold && _holdingObject)
        {
            _toToggleHold = false;
            _isHolding = false;
            _ReleaseMovable();
            ChangeAnimationState(CLAW_EMPTY);
            return;
        }

        if (_isHolding) 
        {
            _holdingObject.transform.position = holdingPoint.position;
        }
    }

    void ChangeAnimationState(string newState) 
    {
        if (_currentAnimationState == newState) return;
        _currentAnimationState = newState;
        _clawBodyAnimator.Play(newState);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(holdingPoint.position, Vector2.one * detectionRadius);
    }

    private Collider2D _NearestMovable() 
    {
        Collider2D closestCollision = _allEnteredMovables[0];
        float minDist = Vector2.Distance(
                holdingPoint.transform.position,
                _allEnteredMovables[0].transform.position
            );
        float dist;
        foreach (Collider2D collision in _allEnteredMovables) 
        {
            dist = Vector2.Distance(
                    holdingPoint.transform.position,
                    collision.transform.position
                );
            if (dist < minDist) 
            {
                closestCollision = collision;
                minDist = dist;
            }
        }
        return closestCollision;
    }
    private void _ReleaseMovable()
    {
        Rigidbody2D holdingRB = _holdingObject.GetComponent<Rigidbody2D>();
        holdingRB.gravityScale = _objectGravityScale;
        _holdingObject = null;
    }

    private void _GrabMovable(Collider2D closestMovable)
    {
        closestMovable.transform.position = holdingPoint.position;
        _holdingObject = closestMovable.gameObject;
        Rigidbody2D holdingRB = closestMovable.GetComponent<Rigidbody2D>();
        _objectGravityScale = holdingRB.gravityScale;
        holdingRB.gravityScale = 0;
    }
    private void SetWireLength()
    {
        if (_hitCeiling)
        {
            _wireLR.SetPosition(0, wirePoint.position);
            _wireLR.SetPosition(1, new Vector2 (
                    wirePoint.position.x,
                    _hitCeiling.point.y
                ));
            return;
        }
        _wireLR.enabled = false;
    }
}
