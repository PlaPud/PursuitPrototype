using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private EventInstance _ambienceInstance;
    private EventInstance _musicInstance;

    private void Awake()
    {
        if (Instance != null) 
        {
            Debug.LogError("More than one instance of AudioManager found!");
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        InitializeAmbience(FMODEvents.Instance.Ambience);
        InitializeMusic(FMODEvents.Instance.MainMusic);

    }

    public void SetAmbienceByArea(StageAudioAreaType areaType) 
    {
        _ambienceInstance.setParameterByName("stageArea", (float) areaType);
    } 

    public void InitializeAmbience(EventReference ambienceEventRef) 
    {
        if (ambienceEventRef.IsNull) return;

        _ambienceInstance = RuntimeManager.CreateInstance(ambienceEventRef);
        _ambienceInstance.start();
    }

    public void InitializeMusic(EventReference musicEventRef)
    {
        if (musicEventRef.IsNull) return;

        _musicInstance = RuntimeManager.CreateInstance(musicEventRef);
        _musicInstance.start();
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos) 
    {
        if (sound.IsNull) return;

        RuntimeManager.PlayOneShot(sound, worldPos);
    } 

    private void OnDestroy()
    {
        _ambienceInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        _ambienceInstance.release();

        _musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        _musicInstance.release();
    }
}
