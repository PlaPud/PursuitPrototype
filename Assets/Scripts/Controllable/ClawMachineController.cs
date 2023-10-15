using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private float clawEndOffset;
    [SerializeField] private GameObject clawBody;
    [SerializeField] private GameObject clawMiddle;
    [SerializeField] private Transform  holdingPoint;
    [SerializeField] private Transform  wirePoint;
    [SerializeField] private LayerMask  movableLayer;
    [SerializeField] private LayerMask  railLayer;


    private LineRenderer _wireLR;

    private float _moveX;
    private float _moveY;
    private float _objectGravityScale;
    private bool  _isHolding;

    private GameObject _holdingObject;

    private bool _toToggleHold;

    private Rigidbody2D  _clawBodyRB;
    private Collider2D   _clawBodyCD;
    private Collider2D[] _allEnteredMovables;
    private RaycastHit2D _hitRailMid;
    private RaycastHit2D _hitRailLeft;
    private RaycastHit2D _hitRailRight;

    private Animator _clawBodyAnimator;

    string _currentAnimationState = CLAW_EMPTY;

    const string CLAW_EMPTY = "ClawEmpty";
    const string CLAW_HOLD = "ClawHold";

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

        if (ClawMachineManager.instance.IsControlClaw && panel.IsControllingThis) 
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
        if (_isToggleHold && _allEnteredMovables.Length > 0) _toToggleHold = true;
    }

    private void HandleMove() 
    {
        Collider2D clawMiddleCD = clawMiddle.GetComponent<Collider2D>();
        Rigidbody2D clawMiddleRB = clawMiddle.GetComponent<Rigidbody2D>();
        bool cantMoveRight = !_hitRailRight && _moveX > 0;
        bool cantMoveLeft = !_hitRailLeft && _moveX < 0;
        bool cantMoveUp = clawMiddleCD.IsTouchingLayers(railLayer) && _moveY > 0;

        float appliedX = (cantMoveLeft || cantMoveRight) ? 0 : _moveX * moveSpeed;
        float appliedY = (cantMoveUp) ? 0 : _moveY * moveSpeed;

        Vector2 appliedVelocity = new Vector2(appliedX, appliedY);
       
        _clawBodyRB.velocity = appliedVelocity;
        clawMiddle.GetComponent<Rigidbody2D>().velocity = _clawBodyRB.velocity;
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
    private void RayCheck()
    {
        _allEnteredMovables = Physics2D.OverlapBoxAll(
                point: holdingPoint.position,
                size: Vector2.one * detectionRadius,
                angle: 0,
                layerMask: movableLayer
            );

        _hitRailMid = Physics2D.Raycast(
                        clawBody.transform.position + Vector3.up * clawEndOffset,
                        Vector2.up,
                        Mathf.Infinity,
                        railLayer
                    );
        _hitRailLeft = Physics2D.Raycast(
                        clawBody.transform.position - Vector3.right * railCheckDistance,
                        Vector2.up,
                        Mathf.Infinity,
                        railLayer
                    );
        _hitRailRight = Physics2D.Raycast(
                        clawBody.transform.position + Vector3.right * railCheckDistance,
                        Vector2.up,
                        Mathf.Infinity,
                        railLayer
                    );
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(holdingPoint.position, Vector2.one * detectionRadius);
        Gizmos.DrawLine(clawBody.transform.position + Vector3.up * clawEndOffset, clawBody.transform.position + Vector3.up * clawEndOffset + Vector3.up * 10f);
        Gizmos.DrawLine(clawBody.transform.position + Vector3.right * railCheckDistance, clawBody.transform.position + Vector3.right * railCheckDistance + Vector3.up * 10f);
        Gizmos.DrawLine(clawBody.transform.position - Vector3.right * railCheckDistance, clawBody.transform.position - Vector3.right * railCheckDistance + Vector3.up * 10f);
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
        _clawBodyCD.enabled = true;
        Rigidbody2D holdingRB = _holdingObject.GetComponent<Rigidbody2D>();
        holdingRB.gravityScale = _objectGravityScale;
        _holdingObject = null;
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
        if (_hitRailMid)
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
