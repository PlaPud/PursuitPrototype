using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("EditMode")]
[assembly: InternalsVisibleTo("PlayMode")]

public class EnemyController : MonoBehaviour
{
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

    [SerializeField] private PlayerController target;

    [Header("Patrol Movement")]
    [SerializeField] private float walkSpeed;

    [Header("Chasing Movement")]
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float waitForTeleportTimeLower;
    [SerializeField] private float waitForTeleportTimeUpper;

    [Header("Attacking")]
    [SerializeField] private float attackRange;
    [SerializeField] private float chargeAttackTime;
    [SerializeField] private float attackLeapForce;
    [SerializeField] private float coolDownTime;
    [SerializeField] private float avgAttackDamage;

    [Header("Ray Check")]
    [SerializeField] private float groundDistance;
    [SerializeField] private Vector2 groundCheckBox;
    [SerializeField] private float playerCheckRadius;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask playerMask;

    private float _changeDirTimer = 2f;
    private float _confirmTeleportTimer;
    private float _currentEnemySpeed;
    private float _chargeToAttackTimer;
    private float _coolDownTimer;

    private bool _isAttacking = false;
    private bool _isTeleportOut = false;
    private bool _isTeleportIn = false;
    private bool _isTeleportingAnim = false;

    private Rigidbody2D _enemyRB;
    private RaycastHit2D _playerLayerHit;
    private RaycastHit2D _groundHit;
    private RaycastHit2D _attackRangeHit;

    private SpriteRenderer _enemySR;
    private Animator _enemyAnim;

    public bool IsPlayerFound { get; private set; } = false;
    public bool IsPlayerInRange { get; private set; } = false;
    public bool IsChangingDir { get; private set; } = false;
    public bool IsGrounded => _groundHit;

    public bool IsPlayerOnHigherGround => target.IsGrounded && target.transform.position.y - transform.position.y > .5f;

    public EnemyState CurrentState = EnemyState.Patrol;

    private string _animState = ENEMY_IDLE;

    private const string ENEMY_IDLE = "EnemyBotIdle";
    private const string ENEMY_WALKING = "EnemyBotWalking";
    private const string ENEMY_CHASING = "EnemyBotChasing";
    private const string ENEMY_JUMP = "EnemyBotJump";
    private const string ENEMY_TELEIN = "EnemyBotTeleIn";
    private const string ENEMY_TELEOUT = "EnemyBotTeleOut";

    private void Awake()
    {
        _enemyRB = GetComponent<Rigidbody2D>();
        _enemyAnim = GetComponent<Animator>();
        _enemySR = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        _currentEnemySpeed = walkSpeed;
        _confirmTeleportTimer = Random.Range(waitForTeleportTimeLower, waitForTeleportTimeUpper);
        _chargeToAttackTimer = chargeAttackTime;
        _coolDownTimer = coolDownTime;
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

    internal void HandlePatrol()
    {
        float newChangeDirTime = Random.Range(3f, 5f);
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
        Vector2 diffDist = target.transform.position - transform.position;
        if (Mathf.Abs(diffDist.x) > .5f)
        {
            float targetDir = Mathf.Sign(diffDist.x);
            Vector2 chaseForce = targetDir * chaseSpeed * Vector2.right;
            _enemyRB.velocity = new Vector2(targetDir * chaseSpeed, _enemyRB.velocity.y);
        }
        else
        {
            _enemyRB.velocity = new Vector2(0f, _enemyRB.velocity.y);
        }

        if (!IsPlayerOnHigherGround) return;

        _confirmTeleportTimer -= _confirmTeleportTimer > 0f ? Time.deltaTime : 0f;

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
        transform.position = target.transform.position;
        _enemyRB.velocity = Vector2.zero;

        yield return new WaitForSeconds(.5f);

        _confirmTeleportTimer = Random.Range(waitForTeleportTimeLower, waitForTeleportTimeUpper);
        CurrentState = EnemyState.Chase;
        _isTeleportIn = false;
    }

    internal void HandleAttack()
    {
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
        yield return new WaitForSeconds(.5f);
        CurrentState = EnemyState.TeleportHide;
        _isTeleportOut = false;

        //yield return new WaitUntil(() => IsPlayerOnHigherGround);
        
        //transform.position = target.transform.position;
        //_enemyRB.velocity = Vector2.zero;
        
        //yield return new WaitForSeconds(.5f);

        //_confirmTeleportTimer = Random.Range(waitForTeleportTimeLower, waitForTeleportTimeUpper);
        //_isTeleporting = false;
        //CurrentState = EnemyState.Chase;
    }
    private IEnumerator _JumpAttack()
    {
        _isAttacking = true;
        Vector2 diffDist = target.transform.position - transform.position;
        float targetDir = Mathf.Sign(diffDist.x);
        _enemyRB.AddForce(new Vector2(attackLeapForce * targetDir, attackLeapForce), ForceMode2D.Impulse);

        yield return new WaitForSeconds(.2f);

        yield return new WaitUntil(() => IsGrounded);

        _isAttacking = false;
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

                if (_confirmTeleportTimer <= 0f && target.IsGrounded)
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
                if (!IsPlayerOnHigherGround) return;
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

                if (Mathf.Abs(_enemyRB.velocity.x) <= walkSpeed && IsGrounded) return;

                if (IsGrounded)
                {
                    _ChangeAnimationState(ENEMY_CHASING);
                    return;
                }

                _ChangeAnimationState(ENEMY_JUMP);

                break;
            case ENEMY_CHASING:

                if (Mathf.Abs(_enemyRB.velocity.x) <= walkSpeed && IsGrounded)
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
    IEnumerator _TeleportAnim()
    {
        _isTeleportingAnim = true;
        _ChangeAnimationState(ENEMY_TELEOUT);
        yield return new WaitForSeconds(.5f);

        _enemySR.enabled = false;
        yield return new WaitUntil(() => IsPlayerOnHigherGround);
        _enemySR.enabled = true;

        _ChangeAnimationState(ENEMY_TELEIN);
        yield return new WaitForSeconds(.5f);
        _ChangeAnimationState(ENEMY_IDLE);
        _isTeleportingAnim = false;
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

        _attackRangeHit = Physics2D.CircleCast(
                origin: transform.position,
                radius: attackRange,
                distance: 0f,
                direction: Vector2.zero,
                layerMask: playerMask
            );

        IsPlayerFound = _playerLayerHit && _playerLayerHit.collider.CompareTag("PlayerCat");
        IsPlayerInRange = _attackRangeHit && _attackRangeHit.collider.CompareTag("PlayerCat");

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + groundDistance * Vector3.down, groundCheckBox);
        Gizmos.DrawWireSphere(transform.position, playerCheckRadius);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}


