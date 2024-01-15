using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopePoint : MonoBehaviour
{

    [SerializeField] LayerMask playerLayer;

    private PlayerController target;
    private Collider2D _playerNear;
    private SpriteRenderer _outline;

    private bool _isUsing;
    private bool _isPlayerAboveRopePoint => target.transform.position.y > transform.position.y;
    private bool _isPlayerFacingThis => target.FrontRopePointHit && target.FrontRopePointHit.transform.position == transform.position;
    private void Awake()
    {
        target = GameObject.FindGameObjectsWithTag("PlayerCat")[0].GetComponent<PlayerController>();
        _outline = GetComponentsInChildren<SpriteRenderer>()[1];
    }

    void Start()
    {

    }

    void Update()
    {
        _playerNear = Physics2D.OverlapCircle(
                point: transform.position,
                radius: target.PlayerRopeRadius,
                layerMask: playerLayer
            );
        
        CheckEnableOutline();
        HandleRopePoint();
    }

    private void HandleRopePoint()
    {
        bool canShootRope = _playerNear
                            && target.PlayerRopeJoint
                            && !target.PlayerRopeJoint.enabled;

        if (target.IsSwingPressed && !target.IsGrounded)
        {
            if (!_isUsing && _isPlayerAboveRopePoint) return;

            if (canShootRope && _isPlayerFacingThis || _isUsing) 
            {
                if (!_isUsing) AudioManager.Instance?.PlayOneShot(FMODEvents.Instance.PlayerSwingEnable, transform.position);
                _isUsing = true;
                target.PlayerRopeJoint.connectedAnchor = transform.position;
                target.PlayerRopeJoint.enabled = true;
                target.PlayerRopeRenderer.SetPosition(0, transform.position);
                target.PlayerRopeRenderer.SetPosition(1, target.transform.position);
                target.PlayerRopeRenderer.enabled = true;
                return;
            }
        }
        else
        {
            _isUsing = false;
            target.PlayerRopeJoint.enabled = false;
            target.PlayerRopeRenderer.enabled = false;
        }

    }

    private void CheckEnableOutline() 
    {
        if (!_isPlayerAboveRopePoint && _isPlayerFacingThis || _isUsing)
        {
            _outline.enabled = true;
            return;
        }

        _outline.enabled = false;   
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireCube(transform.position - transform.up * castDistance, boxSize);
        //Gizmos.DrawWireSphere(transform.position, target.PlayerRopeRadius);
    }
}
