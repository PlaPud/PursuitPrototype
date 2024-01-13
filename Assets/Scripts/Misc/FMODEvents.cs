using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Ambience")]
    [field: SerializeField] public EventReference Ambience { get; private set; }


    [field: Header("Switches SFX")]
    [field: SerializeField] public EventReference SwitchPressed { get; private set; }
    [field: SerializeField] public EventReference PressureDoorMoved { get; private set; }
    [field: SerializeField] public EventReference ToggleDoorMoved { get; private set; }

    [field: Header("Level Controls SFX")]
    [field: SerializeField] public EventReference SavedLevelComplete { get; private set; }


    [field: Header("Music")]
    [field: SerializeField] public EventReference MainMusic { get; private set; }
    [field: SerializeField] public EventReference CombatMusic { get; private set; }

    public static FMODEvents Instance { get; private set; }

    private List<EventInstance> _eventInstances = new List<EventInstance>();

    private void Awake()
    {
        if (Instance != null) 
        {
            Debug.LogError("More than one FMODEvents in the scene!");
            return;
        }

        Instance = this;
    }

    private void CleanUpEvents() 
    {
        _eventInstances.ForEach(
                (eInstance) =>
                {
                    eInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                    eInstance.release();
                }
            );
    }

    private void OnDestroy()
    {
        CleanUpEvents();
    }
}
