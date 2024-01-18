using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Volume")]

    [Range(0, 1)]
    [SerializeField] public float MasterVolume = 0.8f;

    [Range(0, 1)]
    [SerializeField] public float MusicVolume = 0.5f;

    [Range(0, 1)]
    [SerializeField] public float AmbienceVolume = 0.5f;

    [Range(0, 1)]
    [SerializeField] public float SFXVolume = 0.75f;

    private Bus _masterBus;
    private Bus _musicBus;
    private Bus _ambienceBus;
    private Bus _sfxBus;

    private EventInstance _ambienceInstance;
    private EventInstance _musicInstance;

    private bool _isMuteAtStart = true;

    public StageAudioAreaType CurrentMusic { get; private set; }

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

    private void OnEnable()
    {
        SceneManager.sceneUnloaded += OnSceneUnLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneUnloaded -= OnSceneUnLoaded;
    }

    private void Start()
    {
        StartCoroutine(_MuteForSeconds(2f));
        _LoadVolumeSettingPref();
        InitializeAmbience(FMODEvents.Instance.Ambience);
        InitializeMusic(FMODEvents.Instance.MainMusic);
    }

    
    private IEnumerator _MuteForSeconds(float seconds)
    {
        _isMuteAtStart = true;
        _masterBus.setVolume(0f);
        yield return new WaitForSeconds(seconds);
        _isMuteAtStart = false;
    }

    private void Update()
    {
        if (_isMuteAtStart) return;
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
    }

    public void SetMusicByArea(StageAudioAreaType areaType, bool isRandomDelay = false)
    {
        CurrentMusic = areaType;
        
        if (isRandomDelay)
        {
            StartCoroutine(_RandomDelayStartMusic(areaType));
            return;
        }

        _musicInstance.setParameterByName("stageArea", (float) areaType);
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

    private void OnSceneUnLoaded(Scene scene)
    {
        Debug.Log("Scene Unloaded. Save Setting To Prefs");
    }

    private void OnApplicationQuit()
    {
        _SaveVolumeSettingPref();
    }

    private void _SetVolumeBuses()
    {
        _masterBus.setVolume(MasterVolume);
        _musicBus.setVolume(MusicVolume);
        _ambienceBus.setVolume(AmbienceVolume);
        _sfxBus.setVolume(SFXVolume);
    }

    private void _SaveVolumeSettingPref()
    {
        float masterVol, musicVol, ambienceVol, sfxVol;

        _masterBus.getVolume(out masterVol);
        _musicBus.getVolume(out musicVol);
        _ambienceBus.getVolume(out ambienceVol);
        _sfxBus.getVolume(out sfxVol);

        PlayerPrefs.SetFloat("MasterVolume", masterVol);
        PlayerPrefs.SetFloat("MusicVolume", musicVol);
        PlayerPrefs.SetFloat("AmbienceVolume", ambienceVol);
        PlayerPrefs.SetFloat("SFXVolume", sfxVol);
    }

    private void _LoadVolumeSettingPref()
    {
        MasterVolume = PlayerPrefs.GetFloat("MasterVolume", MasterVolume);
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", MusicVolume);
        AmbienceVolume = PlayerPrefs.GetFloat("AmbienceVolume", AmbienceVolume);
        SFXVolume = PlayerPrefs.GetFloat("SFXVolume", SFXVolume);
    }
    private IEnumerator _RandomDelayStartMusic(StageAudioAreaType areaType)
    {
        yield return new WaitForSeconds(Random.Range(0, 5));
        _musicInstance.setParameterByName("stageArea", (float)areaType);
        _musicInstance.start();
    }

    private void OnDestroy()
    {
        _SaveVolumeSettingPref();

        _ambienceInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        _ambienceInstance.release();

        _musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        _musicInstance.release();
    }
}
