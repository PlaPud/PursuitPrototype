using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour
{

    public bool IsPuzzleComplete;

    private const int PLAYER_LAYER = 8;

    [SerializeField] List<DoorController> toggleDoorsTarget = new List<DoorController>();

    public bool IsPressed { get; private set; } = false;
    private bool _canPress;
    void Start()
    {
        
    }

    void Update()
    {
        if (IsPuzzleComplete) 
        {
            foreach (DoorController target in toggleDoorsTarget) 
            {
                target.SetOpenDoor();
                //StartCoroutine(target.SetOpenDoor());
            }
            return;
        };

        OnPressed();

        HandlePress();
    }

    void OnPressed() 
    {
        IsPressed = _canPress && Input.GetKeyDown(KeyCode.E);
    }

    void HandlePress() 
    {
        if (!IsPressed) return;

        foreach (DoorController target in toggleDoorsTarget) 
        {
            _ToggleDoor(target);
        }
    }

    private void _ToggleDoor(DoorController targetDoor) 
    {
        if (targetDoor.IsOpen) 
        {
            targetDoor.SetCloseDoor();
        } 
        else 
        {
            targetDoor.SetOpenDoor();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        bool isPlayer =
            collision.gameObject.layer == PLAYER_LAYER;
        if (!isPlayer) return;
        _canPress = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        bool isObjectExit =
            collision.gameObject.layer == PLAYER_LAYER;
        if (!isObjectExit) return;
        _canPress = false;
    }

}
