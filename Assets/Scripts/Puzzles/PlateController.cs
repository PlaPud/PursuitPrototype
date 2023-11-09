using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateController : MonoBehaviour
{

    private const int PLAYER_LAYER = 8;

    private const string PLATE_EMPTY = "PressurePlateEmpty";
    private const string PLATE_PRESSED = "PressurePlatePressed";

    public bool IsPuzzleComplete;

    [SerializeField] private DoorController targetDoor;

    public bool IsPressing { get; internal set; } = false;

    private Animator _plateAnim;

    void Start()
    {
        _plateAnim = GetComponent<Animator>();  
    }

    void Update()
    {
        if (IsPuzzleComplete) 
        {
            targetDoor.SetOpenDoor();
            return;
        };

        if (IsPressing)
            HandlePress();
        else 
            HandleEmpty();
    }

    private void HandleEmpty() 
    {
        _ChangeAnimationState(PLATE_EMPTY);
        if (!targetDoor.IsOpen) return;
        targetDoor.SetCloseDoor();
    }

    private void HandlePress() 
    {
        _ChangeAnimationState(PLATE_PRESSED);
        if (targetDoor.IsOpen) return;
        targetDoor.SetOpenDoor();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        bool isPressing = 
            collision.gameObject.layer == PLAYER_LAYER || collision.CompareTag("Moveable");
        if (!isPressing) return;
        IsPressing = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        bool isObjectExit =
            collision.gameObject.layer == PLAYER_LAYER || collision.CompareTag("Moveable");
        if (!isObjectExit) return;
        IsPressing = false;
    }

    private void _ChangeAnimationState(string newState) 
    {
        if (newState == _plateAnim.name) return;
        _plateAnim.Play(newState);
    }
}
