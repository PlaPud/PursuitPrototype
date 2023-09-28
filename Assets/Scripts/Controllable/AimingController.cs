using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingController : MonoBehaviour
{
    [SerializeField] Transform followingTarget;
    [SerializeField] Transform arrow;

    private Vector3 _aimingPositionGlobal;
    private Vector3 _aimingCircleDirection;
    private float _aimingAngle;


    public float AimingAngle { get { return _aimingAngle; } }
    public Vector3 AimingPositionGlobal { get { return _aimingPositionGlobal; } }
    public Vector3 AimingCircleDirection { get { return _aimingCircleDirection; } }

    void Start()
    {

    }

    void Update()
    {
        transform.position = followingTarget.position;
        _aimingPositionGlobal = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _aimingCircleDirection = _aimingPositionGlobal - transform.position;
        _aimingAngle = Mathf.Atan2(_aimingCircleDirection.y, _aimingCircleDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, _aimingAngle);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawLine(transform.position, _aimingPositionGlobal);
        //Gizmos.DrawLine(Vector2.zero, _aimingPositionGlobal);
    }


}
