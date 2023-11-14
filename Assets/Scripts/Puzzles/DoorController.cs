using PlasticPipe.Client;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("EditMode")]
[assembly: InternalsVisibleTo("PlayMode")]

public class DoorController : MonoBehaviour
{
    public enum DoorOpenDir
    {
        Upward, Downward, Horizontal
    }

    [SerializeField] private DoorOpenDir doorOpenDirection = DoorOpenDir.Upward;

    public Transform DoorBody;
    public bool IsOpen = false;

    private bool _isTransitioning = false;

    private BoxCollider2D _doorCD;
    public Vector2 DoorOpenPos { get; private set; }
    public Vector2 DoorClosePos { get; private set; }

    internal void Start()
    {
        _doorCD = GetComponentInChildren<BoxCollider2D>();

        DoorClosePos = DoorBody.position;
        DoorOpenPos = (Vector2)DoorBody.position + _GetDoorOpenDirection(doorOpenDirection);
    }

    private void Update()
    {
        if (_isTransitioning)
        {
            HandleDoorTransitioning();
            return;
        }

        SetDoorPositionIdle();
    }

    private void HandleDoorTransitioning()
    {
        if (IsOpen)
        {
            DoorBody.transform.position = Vector2.MoveTowards(
                    current: DoorBody.transform.position,
                    target: DoorOpenPos,
                    maxDistanceDelta: Time.deltaTime * 25f
                );
        }
        else
        {
            DoorBody.transform.position = Vector2.MoveTowards(
                current: DoorBody.transform.position,
                target: DoorClosePos,
                maxDistanceDelta: Time.deltaTime * 25f
            );
        }
    }

    internal void SetDoorPositionIdle()
    {
        if (IsOpen)
        {
            DoorBody.position = DoorOpenPos;
            _doorCD.enabled = false;
        }
        else
        {
            DoorBody.position = DoorClosePos;
            _doorCD.enabled = true;
        }
    }

    public void SetOpenDoor()
    {
        _doorCD.enabled = false;
        if (!_isTransitioning) StartCoroutine(SetDoorTransitionTimer());
        IsOpen = true;
    }

    public void SetCloseDoor()
    {
        _doorCD.enabled = true;
        if (!_isTransitioning) StartCoroutine(SetDoorTransitionTimer());
        IsOpen = false;
    }

    private IEnumerator SetDoorTransitionTimer() 
    {
        _isTransitioning = true;
        yield return new WaitForSeconds(.5f);
        _isTransitioning = false;
    }

    private Vector2 _GetDoorOpenDirection(DoorOpenDir dir)
    {
        switch (dir)
        {
            case DoorOpenDir.Upward:
                return Vector2.up * _doorCD.size.y;
            case DoorOpenDir.Downward:
                return Vector2.down * _doorCD.size.y;
            case DoorOpenDir.Horizontal:
                return Vector2.right * _doorCD.size.x;
            default:
                return Vector2.up * _doorCD.size.y;
        }
    }

}
