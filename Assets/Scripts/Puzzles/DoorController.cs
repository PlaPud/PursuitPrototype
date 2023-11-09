using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Start()
    {
        _doorCD = GetComponentInChildren<BoxCollider2D>();

        DoorClosePos = DoorBody.position;
        DoorOpenPos = (Vector2)DoorBody.position + _GetDoorOpenDirection(doorOpenDirection);
    }

    private void Update()
    {
        if (_isTransitioning) return;

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
        IsOpen = true;
    }

    public void SetCloseDoor()
    {
        _doorCD.enabled = true;
        IsOpen = false;
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
