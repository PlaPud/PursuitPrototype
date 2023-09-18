using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RopePoint : MonoBehaviour
{
    private LineRenderer _ropeLR;
    private SpringJoint2D _ropeDJ;
    private PlayerController _playerController;

    private Collider2D _playerNear;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] float detectRadius;
    [SerializeField] PlayerController target;

    private bool _isUsing;

    private void Awake()
    {
        _ropeLR = GetComponent<LineRenderer>();
        _ropeDJ = GetComponent<SpringJoint2D>();
    }

    void Start()
    {
        _ropeDJ.enabled = false;
        _ropeLR.enabled = false;
    }

    void Update()
    {
        _playerNear = Physics2D.OverlapCircle(
                point: transform.position,
                radius: detectRadius,
                layerMask: playerLayer
            );
        HandleRopePoint();
    }

    private void HandleRopePoint()
    {
        bool canShootRope = _playerNear
                            && !_isUsing;

        bool isPlayerFacingThis = target.FrontRopePointHit &&
            target.FrontRopePointHit.transform.position == transform.position;

        if (target.IsSwingPressed)
        {
            if (canShootRope && isPlayerFacingThis || _isUsing) 
            {
                _isUsing = true;
                _ropeDJ.connectedAnchor = target.transform.position;
                _ropeDJ.enabled = true;
                _ropeLR.SetPosition(1, transform.position);
                _ropeLR.SetPosition(0, target.transform.position);
                _ropeLR.enabled = true;
                return;
            }
        }
        else
        {
            _isUsing = false;
            _ropeDJ.enabled = false;
            _ropeLR.enabled = false;
        }

    }
    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireCube(transform.position - transform.up * castDistance, boxSize);
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
