﻿using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace CozyChaosSpring2024
{
    public class VolumeManager : MonoBehaviour

    {
    public AudioMixer masterMixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    public const string MasterVolumeKey = "MasterVolume";
    public const string MusicVolumeKey = "MusicVolume";
    public const string SfxVolumeKey = "SfxVolume";

    private bool volumeCanvasActive;

    private void Awake()
    {
        masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
    }

    private void Start()
    {
        LoadDefaultVolume();
        // AudioManager.i.PlayMusic(Music.MainTheme);
    }

    private void LoadDefaultVolume()
    {
        masterSlider.value = PlayerPrefs.GetFloat(MasterVolumeKey, 0.8f);
        musicSlider.value = PlayerPrefs.GetFloat(MusicVolumeKey, 0.6f);
        sfxSlider.value = PlayerPrefs.GetFloat(SfxVolumeKey, 0.8f);
    }

    public void OnDisable()
    {
        PlayerPrefs.SetFloat(MasterVolumeKey, masterSlider.value);
        PlayerPrefs.SetFloat(MusicVolumeKey, musicSlider.value);
        PlayerPrefs.SetFloat(SfxVolumeKey, sfxSlider.value);
    }


    private void OnMasterVolumeChanged(float value)
    {
        Debug.Log($"Master Volume {value}");
        masterMixer.SetFloat(MasterVolumeKey, ConvertToDecibel(value));
    }

    private void OnMusicVolumeChanged(float value)
    {
        Debug.Log($"Music Volume {value}");
        masterMixer.SetFloat(MusicVolumeKey, ConvertToDecibel(value));
    }

    private void OnSfxVolumeChanged(float value)
    {
        Debug.Log($"SFX Volume {value}");
        masterMixer.SetFloat(SfxVolumeKey, ConvertToDecibel(value));
    }
    public static float ConvertToDecibel(float volume)
         {
             var clamped = Mathf.Clamp(volume, 0.0001f, float.MaxValue);
             return Mathf.Log10(clamped) * 20.0f;
         }
    }
}