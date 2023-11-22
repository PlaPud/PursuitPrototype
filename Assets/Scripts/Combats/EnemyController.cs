using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("EditMode")]
[assembly: InternalsVisibleTo("PlayMode")]

public class EnemyController : MonoBehaviour
{
    public enum EnemyState 
    {
        Patrol,
        Chase,
        RandomFind,
        Attack,
        Defeated
    }

    [SerializeField] private PlayerController target;

    [Header("Patrol Movement")]
    [SerializeField] private float walkSpeed;

    [Header("Chasing Movement")]
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float jumpForce;

    [Header("Attacking")]
    [SerializeField] private float attackRadius;
    [SerializeField] private float attackLeapForce;
    [SerializeField] private float avgAttackDamage;

    [Header("Finding")]
    [SerializeField] private float findingPatrolSpeed;

    [Header("Ray Check")]
    [SerializeField] private float groundDistance;
    [SerializeField] private Vector2 groundCheckBox;
    [SerializeField] private float playerCheckRadius;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask playerMask;

    private Rigidbody2D _enemyRB;
    private RaycastHit2D _playerLayerHit;
    private RaycastHit2D _groundHit;

    public bool IsFoundPlayer { get; private set; } = false;
    public bool IsGrounded => _groundHit;

    public EnemyState CurrentState = EnemyState.Patrol;

    private void Awake()
    {
        _enemyRB = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        _RayCheck();

        LogicStateMachine();
        _AnimationStateMachine();
    }

    internal void HandlePatrol() 
    {
        
    }

    internal void HandleChase() 
    {
        float targetDir = Mathf.Sign(target.transform.localScale.x);
        Vector2 chaseForce = targetDir * chaseSpeed * Vector2.right;
        _enemyRB.AddForce(chaseForce, ForceMode2D.Force);

    }

    internal void HandleRandomFind() 
    {
    
    }

    internal void HandleAttack() 
    {
    
        
    }

    internal void HandleDefeated() 
    {
        gameObject.SetActive(false);
    }

    internal void _ChangeState() 
    {   
        switch (CurrentState) 
        {
            case EnemyState.Patrol:
                break;
            case EnemyState.Chase:
                break;
            case EnemyState.RandomFind:
                break;
            case EnemyState.Attack:
                break;
            case EnemyState.Defeated:
                break;
        }
    }


    private void LogicStateMachine()
    {
        switch (CurrentState)
        {
            case EnemyState.Patrol:
                HandlePatrol();
                _ChangeState();
                break;
            case EnemyState.Chase:
                HandleChase();
                _ChangeState();
                break;
            case EnemyState.RandomFind:
                HandleRandomFind();
                _ChangeState();
                break;
            case EnemyState.Attack:
                HandleAttack();
                _ChangeState();
                break;
            case EnemyState.Defeated:
                HandleDefeated();
                _ChangeState();
                break;
        }
    }

    private void _AnimationStateMachine() 
    {
        
    }

    private void _RayCheck()
    {
        _groundHit = Physics2D.BoxCast(
                origin: transform.position + Vector3.down * groundDistance,
                size: groundCheckBox,
                angle: 0f,
                direction: Vector2.down,
                distance: 0f,
                layerMask: groundMask
            );

        _playerLayerHit = Physics2D.CircleCast(
                origin: transform.position,
                radius: playerCheckRadius,
                distance: 0f,
                direction: Vector2.zero,
                layerMask: playerMask
            );

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + groundDistance * Vector3.down, groundCheckBox);
        Gizmos.DrawWireSphere(transform.position, playerCheckRadius);
    }
}


