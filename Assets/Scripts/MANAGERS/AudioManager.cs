using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Volume")]

    [Range(0, 1)]
    [SerializeField] public float MasterVolume = 1;

    [Range(0, 1)]
    [SerializeField] public float MusicVolume = 1;

    [Range(0, 1)]
    [SerializeField] public float AmbienceVolume = 1;

    [Range(0, 1)]
    [SerializeField] public float SFXVolume = 1;

    private Bus _masterBus;
    private Bus _musicBus;
    private Bus _ambienceBus;
    private Bus _sfxBus;

    private EventInstance _ambienceInstance;
    private EventInstance _musicInstance;

    public StageAudioAreaType CurrentAreaType { get; private set; }

    private void Awake()
    {
        if (Instance != null) 
        {
            Debug.LogError("More than one instance of AudioManager found!");
            return;
        }

        Instance = this;

        _masterBus = RuntimeManager.GetBus("bus:/");
        _musicBus = RuntimeManager.GetBus("bus:/Music");
        _ambienceBus = RuntimeManager.GetBus("bus:/Ambience");
        _sfxBus = RuntimeManager.GetBus("bus:/SFX");

    }

    private void Start()
    {
        InitializeAmbience(FMODEvents.Instance.Ambience);
        InitializeMusic(FMODEvents.Instance.MainMusic);
    }

    private void Update()
    {
        _SetVolumeBuses();
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        if (sound.IsNull) return;

        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public void SetAmbienceByArea(StageAudioAreaType areaType) 
    {
        _ambienceInstance.setParameterByName("stageArea", (float) areaType);
        CurrentAreaType = areaType;
    } 

    public void SetMusicByArea(StageAudioAreaType areaType)
    {
        _musicInstance.setParameterByName("stageArea", (float) areaType);
        CurrentAreaType = areaType;
        _musicInstance.start();
    }

    public void SetInteruptMusic(StageAudioAreaType audioType)
    {
        _musicInstance.setParameterByName("stageArea", (float) audioType);
        _musicInstance.start();
    }

    public void StopMusic() 
    {
        _musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
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

    public EventInstance CreateEventInstance(EventReference eventRef)
    {
        EventInstance eInstance = RuntimeManager.CreateInstance(eventRef);
        return eInstance;
    }

    private void _SetVolumeBuses()
    {
        _masterBus.setVolume(MasterVolume);
        _musicBus.setVolume(MusicVolume);
        _ambienceBus.setVolume(AmbienceVolume);
        _sfxBus.setVolume(SFXVolume);
    }

    private void OnDestroy()
    {
        _ambienceInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        _ambienceInstance.release();

        _musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        _musicInstance.release();
    }
}
