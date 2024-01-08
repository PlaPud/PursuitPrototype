using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("EditMode")]
[assembly: InternalsVisibleTo("PlayMode")]

public class PlateController : MonoBehaviour
{

    private const int PLAYER_LAYER = 8;

    private const string PLATE_EMPTY = "PressurePlateEmpty";
    private const string PLATE_PRESSED = "PressurePlatePressed";

    public bool IsPuzzleComplete;

    public List<GameObject> OnPlateObjs { get; private set; } = new List<GameObject>();

    [SerializeField] internal DoorController targetDoor;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool isPressing =
            collision.gameObject.layer == PLAYER_LAYER || collision.CompareTag("Moveable");
        
        if (!isPressing) return;

        OnPlateObjs.Add(collision.gameObject);
        
        IsPressing = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        bool isObjectExit =
            collision.gameObject.layer == PLAYER_LAYER || collision.CompareTag("Moveable");

        if (!isObjectExit) return;

        OnPlateObjs.Remove(collision.gameObject);
        
        if (OnPlateObjs.Count <= 0) IsPressing = false;
    }

    private void _ChangeAnimationState(string newState) 
    {
        if (!_plateAnim) return;

        if (newState == _plateAnim.name) return;
        _plateAnim.Play(newState);
    }
}
