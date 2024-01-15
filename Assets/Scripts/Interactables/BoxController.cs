using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour, IDataPersistence
{
    [Header("BoxID")]
    [SerializeField] private string boxid;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float boxCastDistance;
    [SerializeField] private Vector2 boxCastSize;

    private Rigidbody2D _boxRB;
    private FixedJoint2D _boxFJ;
    private RaycastHit2D _hitGround;

    public bool IsGrounded => _hitGround;

    private EventInstance _boxMoveSound;

    private void Start()
    {
        _boxRB = GetComponent<Rigidbody2D>();
        _boxFJ = GetComponent<FixedJoint2D>();
        _boxMoveSound = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.BoxMove);
    }

    private void Update()
    {
        RayGroundCheck();
        HandleFreezePosition();
        HandleOffGround();
    }

    private void FixedUpdate()
    {
        UpdateSound();
    }

    private void HandleFreezePosition()
    {
        if (!_boxFJ.enabled && IsGrounded)
        {
            _boxRB.constraints =
                  RigidbodyConstraints2D.FreezePositionX
                | RigidbodyConstraints2D.FreezeRotation;
            return;
        }

        _boxRB.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void RayGroundCheck()
    {
        _hitGround = Physics2D.BoxCast(
                origin: transform.position + Vector3.down * boxCastDistance,
                size: boxCastSize,
                angle: 0,
                direction: Vector2.down,
                distance: 0f,
                layerMask: groundLayer
            );
    }

    private void HandleOffGround()
    {
        if (_hitGround) return;
        _boxFJ.enabled = false;
    }

    private void UpdateSound() 
    {
        if (!_boxFJ.enabled) 
        {
            PLAYBACK_STATE playbackState;
            _boxMoveSound.getPlaybackState(out playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.PLAYING) || playbackState.Equals(PLAYBACK_STATE.STARTING))
            {
                _boxMoveSound.stop(STOP_MODE.ALLOWFADEOUT);
            }
            return;
        }

        if (Mathf.Abs(_boxRB.velocity.x) > 0.5f && IsGrounded) 
        {
            PLAYBACK_STATE playbackState;
            _boxMoveSound.getPlaybackState(out playbackState);
            
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                _boxMoveSound.start();
            }

            return;
        }

        _boxMoveSound.stop(STOP_MODE.ALLOWFADEOUT);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(
            center: transform.position + Vector3.down * boxCastDistance, 
            size: boxCastSize
        );
    }

    public void LoadData(GameData data)
    {
        if (!data.SavedMoveablePos.ContainsKey(boxid)) return;
        transform.position = data.SavedMoveablePos[boxid].ToUnityVector3();
    }

    public void SaveData(GameData data)
    {
        if (boxid == "") return;

        if (data.SavedMoveablePos.ContainsKey(boxid)) 
        {
            data.SavedMoveablePos[boxid] = new Vector3Serialize(transform.position);
            return;
        }

        data.SavedMoveablePos.Add(boxid, new Vector3Serialize(transform.position));
    }

    [ContextMenu("Generate GUID for This Key Item")]
    private void _GenerateItemGuid() => boxid = "B-" + System.Guid.NewGuid().ToString();
}
