using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ClawMachineController : MonoBehaviour
{
    [Header("Control Panel")]
    [SerializeField] private ClawPanelController panel;

    [Header("Claw")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float detectionRadius;
    [SerializeField] private float railCheckDistance;
    [SerializeField] private Vector2 sideCheckSize;
    [SerializeField] private float sideCheckDistance;
    [SerializeField] private float clawEndOffset;
    [SerializeField] private GameObject clawBody;
    [SerializeField] private Transform  holdingPoint;
    [SerializeField] private Transform  wirePoint;
    [SerializeField] private LayerMask  layerToGrab;
    [SerializeField] private LayerMask  groundLayer;


    private LineRenderer _wireLR;

    private float _moveX;
    private float _moveY;
    private float _objectGravityScale;
    private bool  _isHolding;

    private GameObject _holdingObject;

    private bool _toToggleHold;

    private Rigidbody2D  _clawBodyRB;
    private Collider2D   _clawBodyCD;
    [NonSerialized] private List<Collider2D> _allEnteredCollider = new List<Collider2D>();
    [NonSerialized] private List<Collider2D> _filteredMoveables = new List<Collider2D>();

    private RaycastHit2D _hitRailMid;
    private RaycastHit2D _hitRailLeft;
    private RaycastHit2D _hitRailRight;

    private Collider2D _holdingSideHitLeft;
    private Collider2D _holdingSideHitRight;

    private Animator _clawBodyAnimator;

    string _currentAnimationState = CLAW_EMPTY;

    const string CLAW_EMPTY = "ClawEmpty";
    const string CLAW_HOLD = "ClawHold";

    public bool IsControlling => panel.IsControllingThis;

    private bool _leftStuckOnHold => _holdingObject && _holdingSideHitLeft;
    private bool _rightStuckOnHold => _holdingObject && _holdingSideHitRight;


    private void Awake()
    {
        _wireLR = clawBody.GetComponent<LineRenderer>();
        _clawBodyRB = clawBody.GetComponent<Rigidbody2D>();
        _clawBodyAnimator = clawBody.GetComponent<Animator>();
        _clawBodyCD = clawBody.GetComponent<Collider2D>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        RayCheck();
        FilteringMoveable();

        if (ClawMachineManager.Instance.IsControlClaw && panel.IsControllingThis) 
        {
            OnMove();
            OnToggleHold();
        }

        SetWireLength();
    }

    private void FixedUpdate()
    {
        HandleIdle();
        HandleMove();
        HandleHolding();
    }

    private void OnMove() 
    {
        _moveY = Input.GetAxisRaw("Vertical");
        _moveX = Input.GetAxisRaw("Horizontal");
    }

    private void OnToggleHold() 
    {
        bool _isToggleHold = Input.GetKeyDown(KeyCode.Mouse0);
        if (_isToggleHold && _filteredMoveables.Count > 0) _toToggleHold = true;
    }

    private void HandleMove()
    {
        bool cantMoveRight = (_hitRailRight && !_hitRailRight.collider.CompareTag("ClawMachineRail") || _rightStuckOnHold) && _moveX > 0;
        bool cantMoveLeft = (_hitRailLeft && !_hitRailLeft.collider.CompareTag("ClawMachineRail") || _leftStuckOnHold) && _moveX < 0;
        bool cantMoveDown = _holdingObject && _holdingObject.GetComponent<BoxController>().IsGrounded && _moveY < 00;

        float appliedX = (cantMoveLeft || cantMoveRight) ? 0 : _moveX * moveSpeed;
        float appliedY =  (cantMoveDown) ? 0 : _moveY * moveSpeed;

        Vector2 appliedVelocity = new Vector2(appliedX, appliedY);
       
        _clawBodyRB.velocity = appliedVelocity;
    }

    private void HandleIdle() 
    {
        
    }

    private void HandleHolding() 
    {
        if (_toToggleHold && _filteredMoveables.Count > 0 && !_holdingObject)
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
    private void RayCheck()
    {
        _allEnteredCollider = Physics2D.OverlapBoxAll(
                point: holdingPoint.position,
                size: Vector2.one * detectionRadius,
                angle: 0,
                layerMask: layerToGrab
            ).ToList();

        _hitRailMid = Physics2D.Raycast(
                            clawBody.transform.position + Vector3.up * clawEndOffset,
                            Vector2.up,
                            Mathf.Infinity,
                            groundLayer
                        );
        _hitRailLeft = Physics2D.Raycast(
                            clawBody.transform.position - Vector3.right * railCheckDistance + Vector3.up * clawEndOffset,
                            Vector2.up,
                            Mathf.Infinity,
                            groundLayer
                        );
        _hitRailRight = Physics2D.Raycast(
                            clawBody.transform.position + Vector3.right * railCheckDistance + Vector3.up * clawEndOffset,
                            Vector2.up,
                            Mathf.Infinity,
                            groundLayer
                        );

        if (!_holdingObject) return;

        _holdingSideHitLeft = Physics2D.OverlapBox(
                    clawBody.transform.position + Vector3.left * sideCheckDistance + Vector3.up * 1f,
                    sideCheckSize,
                    0f,
                    groundLayer
                );

        _holdingSideHitRight = Physics2D.OverlapBox(
                    clawBody.transform.position + Vector3.right * sideCheckDistance + Vector3.up * 1f,
                    sideCheckSize,
                    0f,
                    groundLayer
                );

    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(clawBody.transform.position + Vector3.right * sideCheckDistance + Vector3.up * 1f, sideCheckSize);
        Gizmos.DrawWireCube(clawBody.transform.position + Vector3.left * sideCheckDistance + Vector3.up * 1f, sideCheckSize);
        Gizmos.DrawWireCube(holdingPoint.position, Vector2.one * detectionRadius);
        Gizmos.DrawLine(clawBody.transform.position + Vector3.up * clawEndOffset, clawBody.transform.position + Vector3.up * clawEndOffset + Vector3.up * 10f);
        Gizmos.DrawLine(clawBody.transform.position + Vector3.right * railCheckDistance + Vector3.up * clawEndOffset, clawBody.transform.position + Vector3.right * railCheckDistance + Vector3.up * 10f);
        Gizmos.DrawLine(clawBody.transform.position - Vector3.right * railCheckDistance + Vector3.up * clawEndOffset, clawBody.transform.position - Vector3.right * railCheckDistance + Vector3.up * 10f);
    }

    private void FilteringMoveable()
    {
        if (_allEnteredCollider.Count <= 0) 
        {
            if (_filteredMoveables.Count > 0) _filteredMoveables.Clear();
            return;
        }
        _filteredMoveables = (
            from collision in _allEnteredCollider
            where collision.CompareTag("Moveable")
            select collision
        ).ToList();

    }

    private Collider2D _NearestMovable() 
    {
        Collider2D closestMoveable = _filteredMoveables[0];
        float minDist = Vector2.Distance(
                holdingPoint.transform.position,
                _filteredMoveables[0].transform.position
            );
        float dist;
        foreach (Collider2D collision in _filteredMoveables) 
        {
            dist = Vector2.Distance(
                    holdingPoint.transform.position,
                    collision.transform.position
                );
            if (dist < minDist) 
            {
                closestMoveable = collision;
                minDist = dist;
            }
        }
        return closestMoveable;
    }
    private void _ReleaseMovable()
    {
        _clawBodyCD.enabled = true;
        Rigidbody2D holdingRB = _holdingObject.GetComponent<Rigidbody2D>();
        holdingRB.gravityScale = _objectGravityScale;
        _holdingObject = null;
        _filteredMoveables.Clear(); 
    }

    private void _GrabMovable(Collider2D closestMovable)
    {
        _clawBodyCD.enabled = false;
        closestMovable.transform.position = holdingPoint.position;
        _holdingObject = closestMovable.gameObject;
        Rigidbody2D holdingRB = closestMovable.GetComponent<Rigidbody2D>();
        _objectGravityScale = holdingRB.gravityScale;
        holdingRB.gravityScale = 0;
    }
    private void SetWireLength()
    {
        if (_hitRailMid.collider.CompareTag("ClawMachineRail"))
        {
            _wireLR.SetPosition(0, wirePoint.position);
            _wireLR.SetPosition(1, new Vector2 (
                    wirePoint.position.x,
                    _hitRailMid.point.y
                ));
            return;
        }
        _wireLR.enabled = false;
    }
}
