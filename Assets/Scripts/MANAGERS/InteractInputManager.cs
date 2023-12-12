using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractInputManager : MonoBehaviour
{
    public bool IsInteractHold => Input.GetKey(KeyCode.E);
    public bool IsInteracted => Input.GetKeyDown(KeyCode.E);
    public bool IsControlSwitch => Input.GetKey(KeyCode.R);

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
