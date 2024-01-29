using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("EditMode")]
[assembly: InternalsVisibleTo("PlayMode")]

public class EnemyController : MonoBehaviour
{
    public bool DEBUG = false;

    public enum EnemyState
    {
        Despawn,
        Patrol,
        Chase,
        TeleportOut,
        TeleportHide,
        TeleportIn,
        Attack,
        CoolDown,
        Defeated
    }


    [Header("Patrol Movement")]
    [SerializeField] private int walkSpeedLower;
    [SerializeField] private int walkSpeedUpper;

    [Header("Chasing Movement")]
    [SerializeField] private int chaseSpeedLower;
    [SerializeField] private int chaseSpeedUpper;
    [SerializeField] private float changeChaseSpeedTime = 2f;
    [SerializeField] private float jumpForce;
    [SerializeField] private float waitForTeleportTimeLower;
    [SerializeField] private float waitForTeleportTimeUpper;

    [Header("Attacking")]
    [SerializeField] private float rangeToAttack;
    [SerializeField] private float damageRadius;
    [SerializeField] private float chargeAttackTime;
    [SerializeField] private float attackLeapForce;
    [SerializeField] private float coolDownTime;
    [SerializeField] private int avgAttackDamage;

    [Header("Ray Check")]
    [SerializeField] private float groundDistance;
    [SerializeField] private Vector2 groundCheckBox;
    [SerializeField] private float playerCheckRadius;
    [SerializeField] private float enemyCheckRadius;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask enemyMask;

    private int _walkSpeed;
    private int _chaseSpeed;

    private float _changeDirTimer = 2f;
    private float _changeChaseSpeedTimer;
    private float _confirmTeleportTimer;
    private float _currentEnemySpeed;
    private float _chargeToAttackTimer;
    private float _coolDownTimer;

    private bool _isAttacking;
    private bool _isTeleportOut;
    private bool _isTeleportIn;
    private bool _isChangingWalkoutDir;
    private bool _isSetExplosion;

    private Vector2 _playerHighGroundPos;

    private Rigidbody2D _enemyRB;
    private PlayerController _target;
    private RaycastHit2D _playerLayerHit;

    private List<RaycastHit2D> _enemiesHit;

    private RaycastHit2D _groundHit;
    private RaycastHit2D _attackRangeHit;

    private SpriteRenderer _enemySR;
    private Animator _enemyAnim;

    public bool IsPlayerFound { get; private set; }
    public bool IsPlayerInRange { get; private set; }
    public bool IsChangingDir { get; private set; }
    public bool IsGrounded => _groundHit;

    public bool IsPlayerOnHigherGround => _target.IsGrounded && _target.transform.position.y - transform.position.y > .5f;

    public EnemyState CurrentState = EnemyState.Patrol;

    private string _animState = ENEMY_IDLE;

    private const string ENEMY_IDLE = "EnemyBotIdle";
    private const string ENEMY_WALKING = "EnemyBotWalking";
    private const string ENEMY_CHASING = "EnemyBotChasing";
    private const string ENEMY_JUMP = "EnemyBotJump";
    private const string ENEMY_TELEIN = "EnemyBotTeleIn";
    private const string ENEMY_TELEOUT = "EnemyBotTeleOut";

    public Action OnDefeated;

    private void Awake()
    {
        _target = GameObject
            .FindGameObjectsWithTag("PlayerCat")[0]
            .GetComponent<PlayerController>();

        _enemyRB = GetComponent<Rigidbody2D>();
        _enemyAnim = GetComponent<Animator>();
        _enemySR = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        _Initialize();
    }

    void Update()
    {
        _RayCheck();
        _AnimationStateMachine();
    }

    private void FixedUpdate()
    {
        LogicStateMachine();
    }

    public void KillEnemy() 
    {
        CurrentState = EnemyState.Defeated;
    }

    private void OnEnable()
    {
        _Initialize();
        CurrentState = EnemyState.Patrol;
    }

    private void OnDisable()
    {
        CurrentState = EnemyState.Despawn;
    }

    internal void HandlePatrol()
    {
        float newChangeDirTime = UnityEngine.Random.Range(3f, 5f);
        _enemyRB.velocity = new Vector2(_currentEnemySpeed, _enemyRB.velocity.y);

        if (_changeDirTimer > 0f)
        {
            _changeDirTimer -= Time.deltaTime;
            return;
        }

        if (IsChangingDir) return;
        StartCoroutine(_ChangeTimeAndDir(newChangeDirTime));
    }
    internal void HandleChase()
    {

        _changeChaseSpeedTimer -= _changeChaseSpeedTimer - Time.deltaTime > 0f 
            ? Time.deltaTime 
            : _changeChaseSpeedTimer;

        if (_changeChaseSpeedTimer <= 0f) _GetNewChaseSpeed();

        _FollowPlayer();

        if (!IsPlayerOnHigherGround) return;

        _confirmTeleportTimer -= _confirmTeleportTimer > 0f ? Time.deltaTime : 0f;

    }

    private void _GetNewChaseSpeed()
    {
        _chaseSpeed = UnityEngine.Random.Range(chaseSpeedLower, chaseSpeedUpper);
        _changeChaseSpeedTimer = changeChaseSpeedTime;
    }

    private void _FollowPlayer()
    {
        Vector2 diffDist = _target.transform.position - transform.position;
        bool isNotNearPlayerX = Mathf.Abs(diffDist.x) > .5f;

        if (_enemiesHit.Count > 1) 
        {
            if (_isChangingWalkoutDir) return;
            StartCoroutine(_ChangeCrowdWalkOutDir());
            return;
        }

        if (isNotNearPlayerX)
        {
            float targetDir = Mathf.Sign(diffDist.x);
            _enemyRB.velocity = new Vector2(targetDir * _chaseSpeed, _enemyRB.velocity.y);
        }
        else
        {   
            _enemyRB.velocity = new Vector2(0f, _enemyRB.velocity.y);
        }
    }

    IEnumerator _ChangeCrowdWalkOutDir() 
    {
        _isChangingWalkoutDir = true;

        float newSpeed = UnityEngine.Random.Range(chaseSpeedLower, chaseSpeedUpper);
        int newDir = UnityEngine.Random.Range(-1, 2);

        _enemyRB.velocity = new Vector2(newSpeed * newDir, _enemyRB.velocity.y);
        yield return new WaitForSeconds(0.5f);

        _isChangingWalkoutDir = false;
    }

    internal void HandleTeleportOut()
    {
        if (_isTeleportOut) return;
        StartCoroutine(_TeleportOut());
    }

    internal void HandleTeleportHide()
    {
        _enemySR.enabled = false;
    }

    internal void HandleTeleportIn()
    {
        _enemySR.enabled = true;
        if (_isTeleportIn) return;
        StartCoroutine(_TeleportIn());
    }

    private IEnumerator _TeleportIn() 
    {
        _isTeleportIn = true;

        if (DEBUG) 
        { 
            Debug.DrawLine(
                    _playerHighGroundPos, _playerHighGroundPos + Vector2.down * 2f, 
                    color: Color.cyan, duration: 1f
                );
        }
        transform.position = _playerHighGroundPos;
        _enemyRB.velocity = Vector2.zero;

        yield return new WaitForSeconds(.5f);

        _confirmTeleportTimer = UnityEngine.Random.Range(waitForTeleportTimeLower, waitForTeleportTimeUpper);
        CurrentState = EnemyState.Chase;
        _isTeleportIn = false;
    }

    internal void HandleAttack()
    {
        if (_isAttacking) 
        {
            _CheckAttackPlayer();
        }

        if (!IsGrounded || _isTeleportOut) return;

        if (_chargeToAttackTimer > 0f)
        {
            _enemyRB.velocity = new Vector2(0f, _enemyRB.velocity.y);
            _chargeToAttackTimer -= Time.deltaTime;
            return;
        }

        if (_isAttacking) return;
        StartCoroutine(_JumpAttack());
    }

    private void _CheckAttackPlayer() 
    {
        bool isAttacked = Physics2D.CircleCast(
                origin: transform.position,
                radius: damageRadius,
                direction: Vector2.zero,
                distance: 0f,
                layerMask: playerMask
            );

        if (!isAttacked) return;
        PlayerHealth.Instance.DamagePlayer(damage: avgAttackDamage);
    }

    internal void HandleCoolDown()
    {
        _enemyRB.velocity = new Vector2(0f, _enemyRB.velocity.y);

        if (_coolDownTimer > 0)
        {
            _coolDownTimer -= Time.deltaTime;
            return;
        }

        _coolDownTimer = coolDownTime;
        CurrentState = EnemyState.Patrol;
    }
    internal void HandleDefeated()
    {
        if (!gameObject.activeSelf) return;

        OnDefeated?.Invoke();
        gameObject.SetActive(false);
    }

    

    private IEnumerator _ChangeTimeAndDir(float newChangeDirTime)
    {
        float tmpWalkSpeed = _currentEnemySpeed;
        IsChangingDir = true;
        _currentEnemySpeed = 0;
        yield return new WaitForSeconds(1f);
        _changeDirTimer = newChangeDirTime;
        _currentEnemySpeed = -tmpWalkSpeed;
        IsChangingDir = false;
    }
    private IEnumerator _TeleportOut() 
    {
        _isTeleportOut = true;
        _enemyRB.velocity = Vector2.zero;
        _playerHighGroundPos = _target.transform.position;

        if (DEBUG) 
        {
            Debug.DrawLine(
                    _playerHighGroundPos, _playerHighGroundPos + Vector2.down * 2f, 
                    color: Color.blue, duration: 1f
                );
        }

        yield return new WaitForSeconds(.5f);

        CurrentState = EnemyState.TeleportHide;
        _isTeleportOut = false;
    }
    private IEnumerator _JumpAttack()
    {
        _isAttacking = true;
        Vector2 diffDist = _target.transform.position - transform.position;
        float targetDir = Mathf.Sign(diffDist.x);
        _enemySR.color = Color.red;
        _enemyRB.AddForce(new Vector2(attackLeapForce * targetDir, attackLeapForce), ForceMode2D.Impulse);

        AudioManager.Instance?.PlayOneShot(FMODEvents.Instance.EnemyAttack, transform.position); 

        yield return new WaitForSeconds(.2f);

        yield return new WaitUntil(() => IsGrounded);

        _isAttacking = false;
        _enemySR.color = Color.gray;
        CurrentState = EnemyState.CoolDown;
        _chargeToAttackTimer = chargeAttackTime;
    }

    private void LogicStateMachine()
    {
        switch (CurrentState)
        {
            case EnemyState.Patrol:
                HandlePatrol();
                break;
            case EnemyState.Chase:
                HandleChase();
                break;
            case EnemyState.TeleportOut:
                HandleTeleportOut();
                break;
            case EnemyState.TeleportHide:
                HandleTeleportHide();
                break;
            case EnemyState.TeleportIn:
                HandleTeleportIn();
                break;
            case EnemyState.Attack:
                HandleAttack();
                break;
            case EnemyState.CoolDown:
                HandleCoolDown();
                break;
            case EnemyState.Defeated:
                HandleDefeated();
                break;
        }

        _CheckChangeLogicState();
    }
    internal void _CheckChangeLogicState()
    {
        switch (CurrentState)
        {
            case EnemyState.Patrol:
                if (!IsPlayerFound) return;
                CurrentState = EnemyState.Chase;
                break;
            case EnemyState.Chase:

                if (_confirmTeleportTimer <= 0f && IsPlayerOnHigherGround)
                {
                    CurrentState = EnemyState.TeleportOut;
                    return;
                }

                if (!IsPlayerInRange || _isTeleportOut) return;
                CurrentState = EnemyState.Attack;

                break;
            case EnemyState.TeleportOut:
                break;
            case EnemyState.TeleportHide:
                CurrentState = EnemyState.TeleportIn;
                break;
            case EnemyState.TeleportIn:

                break;
            case EnemyState.Attack:
                
                break;
            case EnemyState.Defeated:
                
                break;
        }
    }

    private void _AnimationStateMachine() 
    {
        if (!_enemyAnim) return;

        switch (_animState)
        {
            case ENEMY_IDLE:

                if (CurrentState == EnemyState.TeleportOut)
                {
                    _ChangeAnimationState(ENEMY_TELEOUT);
                }

                if (Mathf.Abs(_enemyRB.velocity.x) <= .05f && IsGrounded) return;

                if (IsGrounded)
                {
                    _ChangeAnimationState(ENEMY_WALKING);
                    return;
                }

                _ChangeAnimationState(ENEMY_JUMP);

                break;
            case ENEMY_WALKING:

                if (Mathf.Abs(_enemyRB.velocity.x) <= .5f && IsGrounded)
                {
                    _ChangeAnimationState(ENEMY_IDLE);
                    return;
                }

                if (Mathf.Abs(_enemyRB.velocity.x) <= _walkSpeed && IsGrounded) return;

                if (IsGrounded)
                {
                    _ChangeAnimationState(ENEMY_CHASING);
                    return;
                }

                _ChangeAnimationState(ENEMY_JUMP);

                break;
            case ENEMY_CHASING:

                if (Mathf.Abs(_enemyRB.velocity.x) <= _walkSpeed && IsGrounded)
                {
                    _ChangeAnimationState(ENEMY_WALKING);
                    return;
                }

                if (CurrentState == EnemyState.TeleportOut) 
                {
                    _ChangeAnimationState(ENEMY_TELEOUT);
                }

                if (IsGrounded) return;
                _ChangeAnimationState(ENEMY_JUMP);
                break;
            case ENEMY_TELEOUT:
                if (CurrentState == EnemyState.TeleportIn) 
                {
                    _ChangeAnimationState(ENEMY_TELEIN);
                }
                break;
            case ENEMY_TELEIN:
                if (CurrentState == EnemyState.Chase) 
                {
                    _ChangeAnimationState(ENEMY_CHASING);
                }
                break;
            case ENEMY_JUMP:
                if (!IsGrounded) return;
                _ChangeAnimationState(ENEMY_IDLE);
                break;
        }
    }
    private void _ChangeAnimationState(string newState) 
    {
        if (newState == _animState) return;
        _animState = newState;
        _enemyAnim.Play(_animState);
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

        _enemiesHit = Physics2D.CircleCastAll(
                origin: transform.position,
                radius: enemyCheckRadius,
                distance: 0f,
                direction: Vector2.zero,
                layerMask: enemyMask
            ).ToList();

        _attackRangeHit = Physics2D.CircleCast(
                origin: transform.position,
                radius: rangeToAttack,
                distance: 0f,
                direction: Vector2.zero,
                layerMask: playerMask
            );

        IsPlayerFound = _playerLayerHit && _playerLayerHit.collider.CompareTag("PlayerCat");
        IsPlayerInRange = _attackRangeHit && _attackRangeHit.collider.CompareTag("PlayerCat");

    }

    private void _Initialize()
    {
        _isAttacking = false;
        _isTeleportOut = false;
        _isTeleportIn = false;
        _isChangingWalkoutDir = false;

        IsPlayerFound = false;
        IsPlayerInRange = false;
        IsChangingDir = false;

        _walkSpeed = UnityEngine.Random.Range(walkSpeedLower, walkSpeedUpper);
        _chaseSpeed = UnityEngine.Random.Range(chaseSpeedLower, chaseSpeedUpper);
        _confirmTeleportTimer = UnityEngine.Random.Range(waitForTeleportTimeLower, waitForTeleportTimeUpper);

        _enemySR.color = Color.gray;
        _changeChaseSpeedTimer = changeChaseSpeedTime;
        _chargeToAttackTimer = chargeAttackTime;
        _coolDownTimer = coolDownTime;

        _currentEnemySpeed = _walkSpeed;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + groundDistance * Vector3.down, groundCheckBox);
        Gizmos.DrawWireSphere(transform.position, playerCheckRadius);
        Gizmos.DrawWireSphere(transform.position, enemyCheckRadius);
        Gizmos.DrawWireSphere(transform.position, rangeToAttack);
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}


