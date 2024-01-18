using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public enum VolumeType {
        MASTER,
        MUSIC,
        SFX,
        AMBIENCE
    }

    [SerializeField] private VolumeType volumeType;

    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponentInChildren<Slider>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        switch (volumeType) 
        {
            case VolumeType.MASTER:
                _slider.value = AudioManager.Instance.MasterVolume;
                break;
            case VolumeType.MUSIC:
                _slider.value = AudioManager.Instance.MusicVolume;
                break;
            case VolumeType.SFX:
                _slider.value = AudioManager.Instance.SFXVolume;
                break;
            case VolumeType.AMBIENCE:
                _slider.value = AudioManager.Instance.AmbienceVolume;
                break;

        }
    }

    public void OnSliderValueChanged() 
    {
        switch (volumeType)
        {
            case VolumeType.MASTER:
                AudioManager.Instance.MasterVolume = _slider.value;
                break;
            case VolumeType.MUSIC:
                AudioManager.Instance.MusicVolume = _slider.value;
                break;
            case VolumeType.SFX:
                AudioManager.Instance.SFXVolume = _slider.value;
                break;
            case VolumeType.AMBIENCE:
                AudioManager.Instance.AmbienceVolume = _slider.value;
                break;

        }
    }
}
