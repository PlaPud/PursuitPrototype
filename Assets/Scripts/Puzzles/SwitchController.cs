using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("EditMode")]
[assembly: InternalsVisibleTo("PlayMode")]

public class SwitchController : Interactable
{

    [Header("Switch Control")]

    [field: SerializeField] public bool IsPuzzleComplete;
    [SerializeField] internal List<DoorController> toggleDoorsTarget = new List<DoorController>();

    [Header("Sound")]
    [SerializeField] private EventReference switchPressed;

    private SpriteRenderer _switchSR;

    void Awake()
    {
        _switchSR = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        
    }

    protected override void Update()
    {
        if (IsPuzzleComplete) 
        {
            foreach (DoorController target in toggleDoorsTarget) 
            {
                target.SetOpenDoor();
            }
            return;
        };

        base.Update();
    }

    public override void HandleInteract()
    {
        foreach (DoorController target in toggleDoorsTarget)
        {
            _ToggleDoor(target);
        }


        if (_switchSR) StartCoroutine(_ToggleSwitchColor());

        if (!FMODEvents.Instance.SwitchPressed.IsNull) AudioManager.Instance.PlayOneShot(FMODEvents.Instance.SwitchPressed, transform.position);
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

    private IEnumerator _ToggleSwitchColor()
    {
        _switchSR.color = Color.gray;
        yield return new WaitForSeconds(.25f);
        _switchSR.color = Color.white;

    }
}
