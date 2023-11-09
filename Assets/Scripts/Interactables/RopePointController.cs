using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopePoint : MonoBehaviour
{
    
    private PlayerController _playerController;

    private Collider2D _playerNear;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] float detectRadius;
    [SerializeField] PlayerController target;

    private bool _isUsing;

    private void Awake()
    {
        
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
        
        HandleRopePoint();
    }

    private void HandleRopePoint()
    {
        bool canShootRope = _playerNear
                            && target.PlayerRopeJoint
                            && !target.PlayerRopeJoint.enabled;

        bool isPlayerFacingThis = target.FrontRopePointHit &&
            target.FrontRopePointHit.transform.position == transform.position;

        if (target.IsSwingPressed && !target.IsGrounded)
        {
            if (canShootRope && isPlayerFacingThis || _isUsing) 
            {
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
    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireCube(transform.position - transform.up * castDistance, boxSize);
        Gizmos.DrawWireSphere(transform.position, target.PlayerRopeRadius);
    }
}
